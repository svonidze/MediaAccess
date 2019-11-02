namespace MediaServer.Contracts
{
    public class TelegramBotConfiguration
    {
        public string Token { get; set; }

        public long[]? AllowedChats { get; set; }
    }
}