using Common.Collections;
using Common.Exceptions;
using Common.Http;
using Common.Monads;
using Common.Serialization.Json;

using Yandex.Music.Integration.Utility;

var playlistIds = new Dictionary<int,string>
    {
        { 1040, "rock.post" },
        { 1044, "hip-hop" },
        { 3, "favorite" },
        { 1038, "jazz" },
        { 1037, "rock.indie" },
        { 1042, "hip-hop.lyrics" },
        { 1015, "rock.russian" },
    };

var yandex = new YandexMusicIntegrator(stagingDirectoryPath: Path.GetTempPath());

//var playlistFiles = await yandex.DownloadMissingPlaylists(playlistIds.Keys).ToListAsync();
var playlistFiles = await yandex.DownloadMissingPlaylists(Enumerable.Range(1000, 50)).ToListAsync();

Console.WriteLine(playlistFiles.JoinToString(Environment.NewLine));

// https://qna.habr.com/q/401476
class YandexMusicIntegrator
{
    // https://music.yandex.ru/users/kirichenkov.sa/playlists/1040
    private const string UrlFormat = "https://music.yandex.ru/handlers/playlist.jsx?owner=kirichenkov.sa&kinds={0}";

    private readonly string stagingDirectoryPath;

    public YandexMusicIntegrator(string stagingDirectoryPath)
    {
        this.stagingDirectoryPath = stagingDirectoryPath;
    }
    
    public async IAsyncEnumerable<string> DownloadMissingPlaylists(IEnumerable<int> playlistIds)
    {
        foreach (var playlistId in playlistIds)
        {
            var fileName = $"yandex-{playlistId}";
            var exportFilePath = Path.Combine(this.stagingDirectoryPath, $"{fileName}.json");
            if (!File.Exists(exportFilePath))
            {
                Console.WriteLine($"File {exportFilePath} does not exist, downloading");
                var downloadResult = await Download(string.Format(UrlFormat, playlistId), exportFilePath);
                if (downloadResult.IsLeft)
                {
                    Console.WriteLine(downloadResult.Left!.GetShortDescription());
                    continue;
                }
            }

            var convertResult = this.ConvertPlaylistToTextFile(exportFilePath);
            if (convertResult.IsLeft)
            {
                Console.WriteLine(convertResult.Left!.GetShortDescription());
                continue;
            }

            var playlist = convertResult.Right!;

            var importFilePath = Path.Combine(this.stagingDirectoryPath, $"{fileName}.{playlist.Title}.txt");
            if (File.Exists(importFilePath))
            {
                Console.WriteLine($"File '{importFilePath}' exists");
                continue;
            }

            Console.WriteLine($"Writing {playlist.TrackCount} tracks to '{importFilePath}'");

            await using var fileStream = File.Open(importFilePath, FileMode.OpenOrCreate);
            await using var writer = new StreamWriter(fileStream);

            foreach (var track in playlist.Tracks)
            {
                var line = $"{track.Artists.Select(a => a.Name).JoinToString(",")} - {track.Title}";
                await writer.WriteLineAsync(line);
            }

            yield return importFilePath;
        }
    }

    private static async Task<EitherExceptionOr<string>> Download(string url, string filePath)
    {
        try
        {
            var httpRequestBuilder = new HttpRequestBuilder().SetUrl(url);
            await using var httpStream = await httpRequestBuilder.GetStreamAsync();
            await using var fileStream = new FileStream(filePath, FileMode.OpenOrCreate);
            Console.WriteLine($"Saving to {filePath}");
            await httpStream.CopyToAsync(fileStream);
            return new EitherExceptionOr<string>(filePath);
        }
        catch (Exception e)
        {
            return new EitherExceptionOr<string>(e);
        }
    }

    private EitherExceptionOr<Playlist> ConvertPlaylistToTextFile(string jsonFilePath)
    {
        if (!File.Exists(jsonFilePath))
        {
            return new EitherExceptionOr<Playlist>(new FileNotFoundException(jsonFilePath));
        }

        using var streamReader = File.OpenText(jsonFilePath);
        var playlist = streamReader.FromJsonTo<Root>()?.Playlist;
        return playlist is null
            ? new EitherExceptionOr<Playlist>(new NullReferenceException(nameof(playlist)))
            : new EitherExceptionOr<Playlist>(playlist);
    }
}