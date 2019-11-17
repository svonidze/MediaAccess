namespace BitTorrent.Contracts
{
    public class NewTorrentInfo
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string HashString { get; set; }
    }
}