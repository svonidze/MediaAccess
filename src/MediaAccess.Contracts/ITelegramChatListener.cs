namespace MediaServer.Contracts
{
    using Telegram.Bot.Types;

    public interface ITelegramChatListener
    {
        void HandleBotInput(string queryData, ITelegramClientAndServerLogger log);

        void HandleUserInput(Message message, ITelegramClientAndServerLogger log);
    }
}