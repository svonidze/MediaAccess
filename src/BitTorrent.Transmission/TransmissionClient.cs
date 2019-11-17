namespace BitTorrent.Transmission
{
    using BitTorrent.Contracts;

    using global::Transmission.API.RPC;

    using JetBrains.Annotations;

    using NewTorrentCommand = global::Transmission.API.RPC.Entity.NewTorrent;

    public class TransmissionClient : IBitTorrentClient
    {
        private readonly BitTorrentClientConfiguration bitTorrentConfig;

        private readonly Client transmissionClient;

        public TransmissionClient([NotNull] BitTorrentClientConfiguration bitTorrentConfig)
        {
            this.bitTorrentConfig = bitTorrentConfig;
            this.transmissionClient = new Client(bitTorrentConfig.Url);
        }

        public bool IsSetUp => this.bitTorrentConfig != null;

        public NewTorrentInfo AddTorrent(NewTorrent torrent)
        {
            var newTorrent = this.transmissionClient.TorrentAdd(
                new NewTorrentCommand
                    {
                        Filename = torrent.FileName,
                        DownloadDirectory = torrent.DownloadDirectory
                    });

            return new NewTorrentInfo
                {
                    Id = newTorrent.ID,
                    Name = newTorrent.Name,
                    HashString = newTorrent.HashString
                };
        }

        public string[] ListDownloadLocations()
        {
            return this.bitTorrentConfig.DownloadLocations;
        }
    }
}