namespace MediaServer.Contracts
{
    using Jackett.Contracts;

    public class Configuration
    {
        public Proxy Proxy { get; set; }

        public JackettAccessConfiguration Jackett { get; set; }
        
        public TelegramBotConfiguration TelegramBot { get; set; }
    }
}