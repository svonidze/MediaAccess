namespace BitTorrent.Contracts
{
    using JetBrains.Annotations;

    public interface IBitTorrentClient
    {
        bool IsSetUp { get; }

        NewTorrentInfo AddTorrent(NewTorrent torrent);

        [CanBeNull]
        string[] ListDownloadLocations();

    }
}