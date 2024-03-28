namespace Yandex.Music.Integration.Utility;

using System.Text.Json.Serialization;

public class Album
{
    [JsonPropertyName("id")]
    public int? Id { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyName("metaType")]
    public string MetaType { get; set; }

    [JsonPropertyName("version")]
    public string Version { get; set; }

    [JsonPropertyName("year")]
    public int? Year { get; set; }

    [JsonPropertyName("releaseDate")]
    public DateTime? ReleaseDate { get; set; }

    [JsonPropertyName("coverUri")]
    public string CoverUri { get; set; }

    [JsonPropertyName("ogImage")]
    public string OgImage { get; set; }

    [JsonPropertyName("genre")]
    public string Genre { get; set; }

    [JsonPropertyName("trackCount")]
    public int? TrackCount { get; set; }

    [JsonPropertyName("likesCount")]
    public int? LikesCount { get; set; }

    [JsonPropertyName("recent")]
    public bool? Recent { get; set; }

    [JsonPropertyName("veryImportant")]
    public bool? VeryImportant { get; set; }

    [JsonPropertyName("artists")]
    public List<Artist> Artists { get; set; }

    [JsonPropertyName("labels")]
    public List<Label> Labels { get; set; }

    [JsonPropertyName("available")]
    public bool? Available { get; set; }

    [JsonPropertyName("availableForPremiumUsers")]
    public bool? AvailableForPremiumUsers { get; set; }

    [JsonPropertyName("disclaimers")]
    public List<object> Disclaimers { get; set; }

    [JsonPropertyName("availableForOptions")]
    public List<string> AvailableForOptions { get; set; }

    [JsonPropertyName("availableForMobile")]
    public bool? AvailableForMobile { get; set; }

    [JsonPropertyName("availablePartially")]
    public bool? AvailablePartially { get; set; }

    [JsonPropertyName("bests")]
    public List<int?> Bests { get; set; }

    [JsonPropertyName("trackPosition")]
    public TrackPosition TrackPosition { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("contentWarning")]
    public string ContentWarning { get; set; }
}

public class Artist
{
    [JsonPropertyName("id")]
    public int? Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("various")]
    public bool? Various { get; set; }

    [JsonPropertyName("composer")]
    public bool? Composer { get; set; }

    [JsonPropertyName("cover")]
    public Cover Cover { get; set; }

    [JsonPropertyName("genres")]
    public List<object> Genres { get; set; }

    [JsonPropertyName("disclaimers")]
    public List<object> Disclaimers { get; set; }
}

public class Cover
{
    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("itemsUri")]
    public List<string> ItemsUri { get; set; }

    [JsonPropertyName("custom")]
    public bool? Custom { get; set; }

    [JsonPropertyName("prefix")]
    public string Prefix { get; set; }

    [JsonPropertyName("uri")]
    public string Uri { get; set; }
}

public class DerivedColors
{
    [JsonPropertyName("average")]
    public string Average { get; set; }

    [JsonPropertyName("waveText")]
    public string WaveText { get; set; }

    [JsonPropertyName("miniPlayer")]
    public string MiniPlayer { get; set; }

    [JsonPropertyName("accent")]
    public string Accent { get; set; }
}

public class Fade
{
    [JsonPropertyName("inStart")]
    public double? InStart { get; set; }

    [JsonPropertyName("inStop")]
    public double? InStop { get; set; }

    [JsonPropertyName("outStart")]
    public double? OutStart { get; set; }

    [JsonPropertyName("outStop")]
    public double? OutStop { get; set; }
}

public class Label
{
    [JsonPropertyName("id")]
    public int? Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }
}

public class LyricsInfo
{
    [JsonPropertyName("hasAvailableSyncLyrics")]
    public bool? HasAvailableSyncLyrics { get; set; }

    [JsonPropertyName("hasAvailableTextLyrics")]
    public bool? HasAvailableTextLyrics { get; set; }
}

public class Major
{
    [JsonPropertyName("id")]
    public int? Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }
}

public class Owner
{
    [JsonPropertyName("uid")]
    public int? Uid { get; set; }

    [JsonPropertyName("login")]
    public string Login { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("sex")]
    public string Sex { get; set; }

    [JsonPropertyName("verified")]
    public bool? Verified { get; set; }

    [JsonPropertyName("avatarHash")]
    public string AvatarHash { get; set; }
}

public class Playlist
{
    [JsonPropertyName("revision")]
    public int? Revision { get; set; }

    [JsonPropertyName("kind")]
    public int? Kind { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonPropertyName("descriptionFormatted")]
    public string DescriptionFormatted { get; set; }

    [JsonPropertyName("trackCount")]
    public int? TrackCount { get; set; }

    [JsonPropertyName("visibility")]
    public string Visibility { get; set; }

    [JsonPropertyName("cover")]
    public Cover Cover { get; set; }

    [JsonPropertyName("owner")]
    public Owner Owner { get; set; }

    [JsonPropertyName("tracks")]
    public List<Track> Tracks { get; set; }

    [JsonPropertyName("created")]
    public DateTime? Created { get; set; }

    [JsonPropertyName("modified")]
    public DateTime? Modified { get; set; }

    [JsonPropertyName("trackIds")]
    public List<string?> TrackIds { get; set; }

    [JsonPropertyName("ogImage")]
    public string OgImage { get; set; }

    [JsonPropertyName("tags")]
    public List<object> Tags { get; set; }

    [JsonPropertyName("likesCount")]
    public int? LikesCount { get; set; }

    [JsonPropertyName("duration")]
    public int? Duration { get; set; }

    [JsonPropertyName("available")]
    public bool? Available { get; set; }

    [JsonPropertyName("doNotIndex")]
    public bool? DoNotIndex { get; set; }
}

public class R128
{
    [JsonPropertyName("i")]
    public double? I { get; set; }

    [JsonPropertyName("tp")]
    public double? Tp { get; set; }
}

public class Root
{
    [JsonPropertyName("playlist")]
    public Playlist Playlist { get; set; }
}

public class Substituted
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("realId")]
    public string RealId { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyName("trackSource")]
    public string TrackSource { get; set; }

    [JsonPropertyName("available")]
    public bool? Available { get; set; }

    [JsonPropertyName("availableForPremiumUsers")]
    public bool? AvailableForPremiumUsers { get; set; }

    [JsonPropertyName("availableFullWithoutPermission")]
    public bool? AvailableFullWithoutPermission { get; set; }

    [JsonPropertyName("disclaimers")]
    public List<object> Disclaimers { get; set; }

    [JsonPropertyName("availableForOptions")]
    public List<object> AvailableForOptions { get; set; }

    [JsonPropertyName("durationMs")]
    public int? DurationMs { get; set; }

    [JsonPropertyName("storageDir")]
    public string StorageDir { get; set; }

    [JsonPropertyName("fileSize")]
    public int? FileSize { get; set; }

    [JsonPropertyName("r128")]
    public R128 R128 { get; set; }

    [JsonPropertyName("fade")]
    public Fade Fade { get; set; }

    [JsonPropertyName("previewDurationMs")]
    public int? PreviewDurationMs { get; set; }

    [JsonPropertyName("artists")]
    public List<Artist> Artists { get; set; }

    [JsonPropertyName("albums")]
    public List<object> Albums { get; set; }

    [JsonPropertyName("ogImage")]
    public string OgImage { get; set; }

    [JsonPropertyName("lyricsAvailable")]
    public bool? LyricsAvailable { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("rememberPosition")]
    public bool? RememberPosition { get; set; }

    [JsonPropertyName("trackSharingFlag")]
    public string TrackSharingFlag { get; set; }

    [JsonPropertyName("error")]
    public string Error { get; set; }
}

public class Track
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("realId")]
    public string RealId { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyName("trackSource")]
    public string TrackSource { get; set; }

    [JsonPropertyName("major")]
    public Major Major { get; set; }

    [JsonPropertyName("available")]
    public bool? Available { get; set; }

    [JsonPropertyName("availableForPremiumUsers")]
    public bool? AvailableForPremiumUsers { get; set; }

    [JsonPropertyName("availableFullWithoutPermission")]
    public bool? AvailableFullWithoutPermission { get; set; }

    [JsonPropertyName("disclaimers")]
    public List<object> Disclaimers { get; set; }

    [JsonPropertyName("availableForOptions")]
    public List<string> AvailableForOptions { get; set; }

    [JsonPropertyName("durationMs")]
    public int? DurationMs { get; set; }

    [JsonPropertyName("storageDir")]
    public string StorageDir { get; set; }

    [JsonPropertyName("fileSize")]
    public int? FileSize { get; set; }

    [JsonPropertyName("r128")]
    public R128 R128 { get; set; }

    [JsonPropertyName("fade")]
    public Fade Fade { get; set; }

    [JsonPropertyName("previewDurationMs")]
    public int? PreviewDurationMs { get; set; }

    [JsonPropertyName("artists")]
    public List<Artist> Artists { get; set; }

    [JsonPropertyName("albums")]
    public List<Album> Albums { get; set; }

    [JsonPropertyName("coverUri")]
    public string CoverUri { get; set; }

    [JsonPropertyName("ogImage")]
    public string OgImage { get; set; }

    [JsonPropertyName("lyricsAvailable")]
    public bool? LyricsAvailable { get; set; }

    [JsonPropertyName("lyricsInfo")]
    public LyricsInfo LyricsInfo { get; set; }

    [JsonPropertyName("derivedColors")]
    public DerivedColors DerivedColors { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("rememberPosition")]
    public bool? RememberPosition { get; set; }

    [JsonPropertyName("trackSharingFlag")]
    public string TrackSharingFlag { get; set; }

    [JsonPropertyName("prefix")]
    public string Prefix { get; set; }

    [JsonPropertyName("substituted")]
    public Substituted Substituted { get; set; }

    [JsonPropertyName("error")]
    public string Error { get; set; }

    [JsonPropertyName("contentWarning")]
    public string ContentWarning { get; set; }

    [JsonPropertyName("version")]
    public string Version { get; set; }
}

public class TrackPosition
{
    [JsonPropertyName("volume")]
    public int? Volume { get; set; }

    [JsonPropertyName("index")]
    public int? Index { get; set; }
}