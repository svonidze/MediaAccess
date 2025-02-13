using Common.Exceptions;
using Common.Http;
using Common.Monads;
using Common.Serialization.Json;
using Common.System.Collections;

using Microsoft.Extensions.Logging;

using Yandex.Music.Integration.Utility;

var allPlaylistIds = new Dictionary<int, string>
    {
        { 1040, "rock.post" },
        { 1044, "hip-hop" },
        { 3, "favorite" },
        { 1038, "jazz" },
        { 1037, "rock.indie" },
        { 1042, "hip-hop.lyrics" },
        { 1015, "rock.russian" },
        /*
           "1004": "детский.сказка",
           "1005": "strange",
           "1006": "rock.vocal",
           "1007": "dub & drum",
           "1009": "beat",
           "1010": "rock.indie.sad",
           "1013": "rap.best",
           "1014": "relax",
           "1015": "rock.russian",
           "1016": "word",
           "1020": "детский",
           "1025": "russian",
           "1027": "jazz.modern",
           "1029": "electronic",
           "1031": "rock.classic",
           "1034": "rock.russian.soft",
           "1035": "rock.dance",
           "1037": "rock.indie",
           "1038": "jazz",
           "1039": "rock.soft",
           "1040": "rock.post",
           "1042": "hip-hop lyrics",
           "1043": "speeches from movies",
           "1044": "hip-hop",
           "1047": "в дорогу",
           "3": "Мне нравится"
         */
    };

var yandex = new YandexMusicIntegrator(stagingDirectoryPath: "/home/sergey/Music/playlists");

//var playlistFiles = await yandex.DownloadMissingPlaylists(playlistIds.Keys).ToListAsync();
var playlistIds = Enumerable.Range(1000, 50).ToList();
playlistIds.Add(3);
var playlistFiles = await yandex
    .DownloadMissingPlaylists(playlistIds: playlistIds, forceReProcessing: true).ToListAsync();

Console.WriteLine(playlistFiles.JoinToString(Environment.NewLine));

// https://qna.habr.com/q/401476
class YandexMusicIntegrator(string stagingDirectoryPath)
{
    // https://music.yandex.ru/users/kirichenkov.sa/playlists/1040
    private const string UrlFormat = "https://music.yandex.ru/handlers/playlist.jsx?owner=kirichenkov.sa&kinds={0}";

    private const string SongNameFormat = "{0} - {1}";

    private const string SongNameDelimiter = " - ";

    public async IAsyncEnumerable<string> DownloadMissingPlaylists(
        IEnumerable<int> playlistIds,
        bool forceReDownloading = false,
        bool forceReProcessing = false)
    {
        var existingPlaylists = new Dictionary<int, string>();

        foreach (var playlistId in playlistIds)
        {
            var fileName = $"yandex-{playlistId}";
            var exportFilePath = Path.Combine(stagingDirectoryPath, $"{fileName}.json");
            if (forceReDownloading || !File.Exists(exportFilePath))
            {
                Console.WriteLine($"File {exportFilePath} does not exist, downloading");
                var downloadResult = await _Download(string.Format(UrlFormat, playlistId), exportFilePath);
                if (downloadResult.IsLeft)
                {
                    Console.WriteLine(downloadResult.Left!.GetShortDescription());
                    continue;
                }
            }

            var convertResult = _ConvertPlaylistToTextFile(exportFilePath);
            if (convertResult.IsLeft)
            {
                Console.WriteLine(convertResult.Left!.GetShortDescription());
                continue;
            }

            var playlist = convertResult.Right!;
            existingPlaylists.Add(playlistId, playlist.Title);

            var importFilePath = Path.Combine(stagingDirectoryPath, $"{fileName}.{playlist.Title}.txt");
            if (!forceReProcessing && File.Exists(importFilePath))
            {
                Console.WriteLine($"File '{importFilePath}' exists");
                continue;
            }

            Console.WriteLine($"Writing {playlist.TrackCount} tracks to '{importFilePath}'");

            await using var fileStream = File.Open(importFilePath, FileMode.Create);
            await using var writer = new StreamWriter(fileStream);

            foreach (var track in playlist.Tracks)
            {
                var artists = track.Artists.Select(a => a.Name).JoinToString(",");
                var trackTitle = track.Title;
                var line = string.Format(SongNameFormat, _WrapFields(artists), _WrapFields(trackTitle));
                await writer.WriteLineAsync(line);
            }

            yield return importFilePath;
        }

        Console.WriteLine(existingPlaylists.ToJsonIndented());
    }

    private static async Task<EitherExceptionOr<string>> _Download(string url, string filePath)
    {
        try
        {
            var logger = _CreateLogger();

            var httpRequestBuilder = new HttpRequestBuilder(logger).SetUrl(url);
            await using var httpStream = await httpRequestBuilder.GetStreamAsync();
            await using var fileStream = new FileStream(filePath, FileMode.OpenOrCreate);
            logger.LogDebug("Saving to {FilePath}", filePath);
            await httpStream.CopyToAsync(fileStream);
            return new EitherExceptionOr<string>(filePath);
        }
        catch (Exception e)
        {
            return new EitherExceptionOr<string>(e);
        }
    }

    private static ILogger<YandexMusicIntegrator> _CreateLogger()
    {
        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Debug));

        return loggerFactory.CreateLogger<YandexMusicIntegrator>();
    }

    private static EitherExceptionOr<Playlist> _ConvertPlaylistToTextFile(string jsonFilePath)
    {
        if (!File.Exists(jsonFilePath))
        {
            return new EitherExceptionOr<Playlist>(new FileNotFoundException(jsonFilePath));
        }

        Console.WriteLine($"Reading {jsonFilePath}");
        using var streamReader = File.OpenText(jsonFilePath);
        Playlist? playlist;
        try
        {
            playlist = streamReader.FromJsonFileTo<Root>()?.Playlist;
        }
        catch (Exception e)
        {
            return new EitherExceptionOr<Playlist>(e);
        }

        return playlist is null
            ? new EitherExceptionOr<Playlist>(new NullReferenceException(nameof(playlist)))
            : new EitherExceptionOr<Playlist>(playlist);
    }

    private static string _WrapFields(string input) =>
        input.Contains(SongNameDelimiter)
            ? $"\"{input}\""
            : input;
}