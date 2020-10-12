namespace MediaServer.Workflow
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;

    using Common.Collections;
    using Common.Serialization.Json;

    using MediaServer.Contracts;

    using Telegram.Bot;
    using Telegram.Bot.Args;
    using Telegram.Bot.Types;

    // TODO use log library
    public class TelegramListener : IDisposable, ITelegramListener
    {
        private readonly ITelegramBotClient botClient;

        private readonly ITelegramFactory telegramFactory;

        private readonly IServerLogger serverLogger;

        private readonly ConcurrentHashSet<long> allowedChats;

        // TODO make cache expiration
        private readonly ConcurrentDictionary<long, ITelegramChatListener> chatByListener =
            new ConcurrentDictionary<long, ITelegramChatListener>();

        public TelegramListener(
            ITelegramFactory telegramFactory,
            IServerLogger serverLogger,
            ITelegramBotClient botClient,
            IEnumerable<long> allowedChatIds)
        {
            this.telegramFactory = telegramFactory;
            this.serverLogger = serverLogger;
            this.botClient = botClient;
            this.allowedChats = allowedChatIds.ToConcurrentHashSet();
        }
        
        public void Dispose()
        {
            this.StopReceiving();
        }

        public void StartReceiving()
        {
            var me = this.botClient.GetMeAsync();

            this.botClient.OnMessage += this.OnMessage;
            this.botClient.OnCallbackQuery += this.OnCallbackQuery;
            this.botClient.StartReceiving();

            this.serverLogger.Log($"{nameof(this.StartReceiving)} is run for bot {me.Result.ToJson()}");
        }

        public void StopReceiving()
        {
            this.serverLogger.Log(nameof(this.StopReceiving));
            
            this.botClient.OnMessage -= this.OnMessage;
            this.botClient.OnCallbackQuery -= this.OnCallbackQuery;
            this.botClient.StopReceiving();
        }

        private ITelegramChatListener GetOrAddChatListener(long chatId) =>
            this.chatByListener.GetOrAdd(
                chatId,
                ci => this.telegramFactory.CreateChatListener());

        private void OnCallbackQuery(object sender, CallbackQueryEventArgs e)
        {
            this.Handle(e.CallbackQuery.Message, (chatListener, log) => chatListener.Handle(e.CallbackQuery.Data, log));
        }

        private void OnMessage(object sender, MessageEventArgs e)
        {
            this.Handle(e.Message, (chatListener, log) => chatListener.Handle(e.Message, log));
        }

        private void Handle(Message message, Action<ITelegramChatListener, ITelegramClientAndServerLogger> action)
        {
            var log = this.telegramFactory.CreateClientAndServerLogger(message);
            if (!this.CheckChatIsAllowed(message, log))
                return;
            
            var chatListener = this.GetOrAddChatListener(message.Chat.Id);
            action(chatListener, log);
        }

        private bool CheckChatIsAllowed(Message message, ITelegramClientAndServerLogger log)
        {
            if (this.allowedChats.IsNullOrEmpty() || this.allowedChats.Contains(message.Chat.Id))
                return true;
            
            log.ReplyBack("You are not allowed to use this Bot instance. Run your own!");
            log.Log($"Bot is not allowed for the user {message.From?.Username} {message.Chat.Id}.");
            return false;
        }
    }
}
