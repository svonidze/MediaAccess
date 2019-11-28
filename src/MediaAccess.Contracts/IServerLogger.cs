namespace MediaServer.Contracts
{
    public interface IServerLogger
    {
        void Log(params object[] texts);
    }
}