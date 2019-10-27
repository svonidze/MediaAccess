namespace MediaServer.Contracts
{
    public class Configuration
    {
        public Proxy Proxy { get; set; }

        public string TelegramBotToken { get; set; }

        public string JacketUrl { get; set; }
        public string JacketApiKey { get; set; }
    }
}