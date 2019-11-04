namespace MediaServer.Workflow
{
    using System;
    using System.Collections.Concurrent;
    using System.Net;

    using Common.Collections;
    using Common.Serialization.Json;

    using MediaServer.Contracts;

    using Telegram.Bot;
    using Telegram.Bot.Args;
    using Telegram.Bot.Types;

    public class TelegramListener
    {
        private readonly ITelegramBotClient botClient;

        private readonly Configuration configuration;

        private readonly ConcurrentHashSet<long> allowedChats;

        // TODO make cache expiration
        private readonly ConcurrentDictionary<long, TelegramChatListener> chatByListener =
            new ConcurrentDictionary<long, TelegramChatListener>();

        public TelegramListener(Configuration configuration)
        {
            this.configuration = configuration;
            this.allowedChats = this.configuration.TelegramBot.AllowedChats.ToConcurrentHashSet();
            
            var proxy = configuration.Proxy;
            var webProxy = new WebProxy(proxy.Host, proxy.Port)
                {
                    Credentials = new NetworkCredential(proxy.UserName, proxy.Password)
                };
            this.botClient = new TelegramBotClient(configuration.TelegramBot.Token, webProxy)
                {
                    Timeout = this.configuration.TelegramBot.Timeout ?? TimeSpan.FromMinutes(1)
                };
        }

        public void StartReceiving()
        {
            var me = this.botClient.GetMeAsync();

            this.botClient.OnMessage += this.OnMessage;
            this.botClient.OnCallbackQuery += this.OnCallbackQuery;
            this.botClient.StartReceiving();

            Console.WriteLine($"{nameof(this.StartReceiving)} is run for bot {me.Result.ToJson()}");
        }

        public void StopReceiving()
        {
            this.botClient.OnMessage -= this.OnMessage;
            this.botClient.OnCallbackQuery -= this.OnCallbackQuery;
            this.botClient.StopReceiving();
        }

        private TelegramChatListener GetOrAddChatListener(long chatId) =>
            this.chatByListener.GetOrAdd(chatId, ci => new TelegramChatListener(this.botClient, this.configuration));

        private async void OnCallbackQuery(object sender, CallbackQueryEventArgs e)
        {
            this.Handle(e.CallbackQuery.Message, (chatListener, log) => chatListener.Handle(e.CallbackQuery.Data, log));
        }

        private async void OnMessage(object sender, MessageEventArgs e)
        {
            this.Handle(e.Message, (chatListener, log) => chatListener.Handle(e.Message, log));
        }

        private void Handle(Message message, Action<TelegramChatListener, ClientAndServerLogger> action)
        {
            var log = new ClientAndServerLogger(this.botClient, message);
            if (!this.CheckChatIsAllowed(message, log))
                return;
            
            var chatListener = this.GetOrAddChatListener(message.Chat.Id);
            action(chatListener, log);
        }

        private bool CheckChatIsAllowed(Message message, ClientAndServerLogger log)
        {
            if (this.allowedChats.IsNullOrEmpty() || this.allowedChats.Contains(message.Chat.Id))
                return true;
            
            log.ReplyBack("You are not allowed to use this Bot instance. Run your own!");
            log.Log($"{message.Chat.Username} in chat {message.Chat.Id} tried to access the Bot. Chat is not allowed.");
            return false;

        }
    }
}