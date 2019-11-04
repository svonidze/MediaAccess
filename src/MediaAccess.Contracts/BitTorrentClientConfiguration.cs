namespace MediaServer.Contracts
{
    using JetBrains.Annotations;

    public class BitTorrentClientConfiguration
    {
        public BitTorrentType BitTorrentType { get; set; }

        [NotNull]
        public string Host { get; set; }
        
        [NotNull]
        public string[] DownloadLocations { get; set; }
    }
}