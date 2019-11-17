namespace MediaServer.Contracts
{
    using Telegram.Bot.Types;

    public interface ITelegramChatListener
    {
        void Handle(string queryData, ITelegramClientAndServerLogger log);

        void Handle(Message message, ITelegramClientAndServerLogger log);
    }
}