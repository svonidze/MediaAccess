namespace Jackett.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    using global::Common.Http.Contracts;

    [DataContract]
    public enum ManualSearchResultIndexerStatus
    {
        Unknown = 0,

        Error = 1,

        OK = 2
    };

    [DataContract]
    public class ManualSearchResultIndexer
    {
        [DataMember]
        public string ID { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public ManualSearchResultIndexerStatus Status { get; set; }

        [DataMember]
        public int Results { get; set; }

        [DataMember]
        public string Error { get; set; }
    }

    [DataContract]
    public class ManualSearchResult : Response
    {
        [DataMember]
        public IList<TrackerCacheResult> Results { get; set; }

        [DataMember]
        public IList<ManualSearchResultIndexer> Indexers { get; set; }
    }

    public class TrackerCacheResult : ReleaseInfo
    {
        public DateTime FirstSeen { get; set; }

        public string Tracker { get; set; }

        public string TrackerId { get; set; }

        public string CategoryDesc { get; set; }

        public Uri BlackholeLink { get; set; }
    }

    public class ReleaseInfo
    {
        public string Title { get; set; }

        public Uri Guid { get; set; }

        public Uri Link { get; set; }

        public Uri Comments { get; set; }

        public DateTime PublishDate { get; set; }

        public ICollection<int> Category { get; set; }

        public long? Size { get; set; }

        public long? Files { get; set; }

        public long? Grabs { get; set; }

        public string Description { get; set; }

        public long? RageID { get; set; }

        public long? TVDBId { get; set; }

        public long? Imdb { get; set; }

        public long? TMDb { get; set; }

        public int? Seeders { get; set; }

        public int? Peers { get; set; }

        public Uri BannerUrl { get; set; }

        public string InfoHash { get; set; }

        public Uri MagnetUri { get; set; }

        public double? MinimumRatio { get; set; }

        public long? MinimumSeedTime { get; set; }

        public double? DownloadVolumeFactor { get; set; }

        public double? UploadVolumeFactor { get; set; }

        public double? Gain
        {
            get
            {
                var sizeInGb = this.Size / 1024.0 / 1024.0 / 1024.0;
                return this.Seeders * sizeInGb;
            }
        }

        // ex: " 3.5  gb   "

        public static long GetBytes(string unit, float value)
        {
            unit = unit.Replace("i", "").ToLowerInvariant();
            if (unit.Contains("kb"))
                return BytesFromKB(value);
            if (unit.Contains("mb"))
                return BytesFromMB(value);
            if (unit.Contains("gb"))
                return BytesFromGB(value);
            if (unit.Contains("tb"))
                return BytesFromTB(value);
            return (long)value;
        }

        public static long BytesFromTB(float tb) => BytesFromGB(tb * 1024f);

        public static long BytesFromGB(float gb) => BytesFromMB(gb * 1024f);

        public static long BytesFromMB(float mb) => BytesFromKB(mb * 1024f);

        public static long BytesFromKB(float kb) => (long)(kb * 1024f);

        public override string ToString() =>
            $"[ReleaseInfo: Title={this.Title}, Guid={this.Guid}, Link={this.Link}, Comments={this.Comments}, PublishDate={this.PublishDate}, Category={this.Category}, Size={this.Size}, Files={this.Files}, Grabs={this.Grabs}, Description={this.Description}, RageID={this.RageID}, TVDBId={this.TVDBId}, Imdb={this.Imdb}, TMDb={this.TMDb}, Seeders={this.Seeders}, Peers={this.Peers}, BannerUrl={this.BannerUrl}, InfoHash={this.InfoHash}, MagnetUri={this.MagnetUri}, MinimumRatio={this.MinimumRatio}, MinimumSeedTime={this.MinimumSeedTime}, DownloadVolumeFactor={this.DownloadVolumeFactor}, UploadVolumeFactor={this.UploadVolumeFactor}, Gain={this.Gain}]";
    }
}