namespace MediaServer.Contracts
{
    public interface ITelegramListener
    {
        void StartReceiving();

        void StopReceiving();
    }
}