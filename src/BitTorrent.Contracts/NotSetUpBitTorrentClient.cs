namespace BitTorrent.Contracts
{
    using System;

    public class NotSetUpBitTorrentClient : IBitTorrentClient
    {
        public bool IsSetUp => false;

        public NewTorrentInfo AddTorrent(NewTorrent torrent)
        {
            throw new NotSupportedException();
        }

        public string[] ListDownloadLocations()
        {
            throw new NotSupportedException();
        }
    }
}
