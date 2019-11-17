namespace MediaServer.Contracts
{
    using Telegram.Bot.Types;

    public interface ITelegramFactory
    {
        ITelegramChatListener CreateChatListener();

        ITelegramClientAndServerLogger CreateClientAndServerLogger(Message message);
    }
}