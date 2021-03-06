namespace BitTorrent.Contracts
{
    using JetBrains.Annotations;

    public class BitTorrentClientConfiguration
    {
        public BitTorrentType BitTorrentType { get; set; }

        public string Url { get; set; }

        [NotNull]
        public string[] DownloadLocations { get; set; }
    }
}