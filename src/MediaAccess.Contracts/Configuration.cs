namespace MediaServer.Contracts
{
    using BitTorrent.Contracts;

    using Jackett.Contracts;

    using JetBrains.Annotations;

    public class Configuration
    {
        public Proxy Proxy { get; set; }

        public JackettAccessConfiguration Jackett { get; set; }
        
        public TelegramBotConfiguration TelegramBot { get; set; }

        [CanBeNull]
        public BitTorrentClientConfiguration BitTorrent { get; set; }
        
        public ViewFilterConfiguration ViewFilter { get; set; }
    }
}