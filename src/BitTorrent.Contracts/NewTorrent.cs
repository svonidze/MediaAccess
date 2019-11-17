namespace BitTorrent.Contracts
{
    public class NewTorrent
    {
        public string FileName { get; set; }

        public string DownloadDirectory { get; set; }

        public string Name { get; set; }
    }
}