namespace MediaServer.Workflow
{
    using System;
    using System.Collections.Concurrent;
    using System.Net;

    using Common.Serialization.Json;

    using MediaServer.Contracts;

    using Telegram.Bot;
    using Telegram.Bot.Args;

    public class TelegramListener
    {
        private readonly ITelegramBotClient botClient;

        private readonly Configuration configuration;

        // TODO make cache expiration
        private readonly ConcurrentDictionary<long, TelegramChatListener> chatByListener =
            new ConcurrentDictionary<long, TelegramChatListener>();

        public TelegramListener(Configuration configuration)
        {
            this.configuration = configuration;

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
            var message = e.CallbackQuery.Message;
            var chatListener = this.GetOrAddChatListener(message.Chat.Id);
            chatListener.Handle(e.CallbackQuery.Data, message);
        }

        private async void OnMessage(object sender, MessageEventArgs e)
        {
            var message = e.Message;
            var chatListener = this.GetOrAddChatListener(message.Chat.Id);
            chatListener.Handle(message);
        }
    }
}