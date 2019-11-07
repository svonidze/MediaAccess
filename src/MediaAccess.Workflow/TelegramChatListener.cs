namespace MediaServer.Workflow
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Net.Http;

    using ByteSizeLib;

    using Common.Collections;
    using Common.Cryptography;
    using Common.Exceptions;
    using Common.Http;
    using Common.Serialization.Json;
    using Common.Text;

    using Jackett.Contracts;

    using MediaServer.Contracts;
    using MediaServer.Workflow.Constants;

    using Telegram.Bot.Types;
    using Telegram.Bot.Types.Enums;
    using Telegram.Bot.Types.ReplyMarkups;

    using Transmission.API.RPC;
    using Transmission.API.RPC.Entity;

    public class TelegramChatListener
    {
        private static readonly string NewLine = Environment.NewLine;

        private readonly Configuration configuration;

        private readonly ConcurrentDictionary<string, Uri> hashToUrl = new ConcurrentDictionary<string, Uri>();

        private ManualSearchResult torrents;

        private TrackerCacheResult torrent;

        private bool waitingForUserInput;

        public TelegramChatListener(Configuration configuration)
        {
            this.configuration = configuration;
        }

        public void Handle(string queryData, ClientAndServerLogger log)
        {
            var bitTorrentConfig = this.configuration.BitTorrent;
            if (BotCommands.PickLocationForTorrent.Regex.TryMath(queryData, out var match))
            {
                var hashedUri = match.Groups[BotCommands.PickLocationForTorrent.Groups.HashUrl].Value;
                if (!this.hashToUrl.TryGetValue(hashedUri, out var uri))
                {
                    log.Text($"Search results are expired, repeat your search");
                    log.Log($"Uri not found by hash '{hashedUri}'");
                    return;
                }

                var torrentCandidates = this.torrents.Results.Where(t => t.Guid == uri).ToArray();
                if (!torrentCandidates.Any())
                {
                    log.Text("Found no torrents, probably Search results are expired, repeat your search");
                    log.Log(
                        $"Torrent could not be found with URL={uri.AbsoluteUri}, " 
                        + $"last search has {this.torrents?.Results?.Count} results");
                    return;
                }

                if (torrentCandidates.Length > 1)
                {
                    log.Text(
                        "Strange, found more than one torrent with the same characteristics, " 
                        + "probably search results are corrupted, repeat your search");
                    log.Log(
                        $"Found {torrentCandidates.Length} torrents with URL={uri.AbsoluteUri}, " 
                        + $"last search has {this.torrents?.Results?.Count} results");
                    return;
                }

                this.torrent = torrentCandidates.Single();

                if (bitTorrentConfig == null || bitTorrentConfig.ForceUsingBlackHole || !bitTorrentConfig.DownloadLocations.Any())
                {
                    this.SendToBlackHoleLocation(log);
                    return;
                }

                log.ReplyBack(
                    $"Pick download location for torrent #{this.GetResultIndex(this.torrent)}",
                    new InlineKeyboardMarkup(
                        bitTorrentConfig.DownloadLocations.Select(
                            dl => InlineKeyboardButton.WithCallbackData(
                                dl,
                                string.Format(BotCommands.StartTorrent.Format, dl)))));
            }
            else if (BotCommands.StartTorrent.Regex.TryMath(queryData, out match))
            {
                if (this.torrent == null)
                {
                    log.Text("No torrent is picked");
                    log.LogLastMessage();
                    return;
                }

                var location = match.Groups[BotCommands.StartTorrent.Groups.HashUrl].Value;

                if (bitTorrentConfig == null)
                {
                    this.SendToBlackHoleLocation(log);
                    return;
                }

                var transmissionClient = new Client(bitTorrentConfig.Url);
                var newTorrent = transmissionClient.TorrentAddAsync(
                    new NewTorrent
                        {
                            Filename = this.torrent.Link.AbsoluteUri,
                            DownloadDirectory = location
                        });

                newTorrent.Wait();
                if (newTorrent.Exception != null)
                {
                    var text = $"Torrent '{this.torrent.Title}' could not be added";
                    log.ReplyBack(newTorrent.Exception.GetShortDescription(text));
                    log.Log(newTorrent.Exception.GetFullDescription(text));
                    return;
                }
                else if (newTorrent.Result == null)
                {
                    log.ReplyBack($"Adding of torrent '{this.torrent.Title}' returned null with no explanation");
                    log.LogLastMessage();
                    return;
                }
                else if (!newTorrent.IsCompletedSuccessfully)
                {
                    log.ReplyBack($"Torrent '{this.torrent.Title}' could not be added due unknown reasons");
                    log.LogLastMessage();
                    return;
                }

                log.ReplyBack($"Torrent '{newTorrent.Result.Name}' was added");
                log.Log($"Torrent '{newTorrent.Result.Name}' was added {newTorrent.Result.ToJson()}");
            }
            else if (BotCommands.GoToPage.Regex.TryMath(queryData, out match))
            {
                var pageString = match.Groups[BotCommands.GoToPage.Groups.Page].Value;
                if (!int.TryParse(pageString, out var page))
                {
                    log.Text(
                        $"Could not parse '{queryData}' as a pagination command. Page group could not recognized as {nameof(Int32)}");
                    log.LogLastMessage();
                    return;
                }

                this.ShowResults(page, log);
            }
            else
            {
                log.Log($"'{queryData}' is not supported");
                log.Text($"Your action '{queryData}' is not supported. Maybe you click old buttons?");
            }
        }

        public void Handle(Message message, ClientAndServerLogger log)
        {
            var messageText = message.Text;
            if (messageText == null)
            {
                log.Text("I can understand only text");
                return;
            }

            if (UserCommands.StartBotCommunication.Regex.TryMath(messageText, out var match))
            {
                log.Text("Greeting! Here you can search for torrents and send them to a favorite BitTorrent client. Lets start with typing /t or /torrent.");
                log.Log(messageText);
            }
            else if (UserCommands.Kinopoisk.Regex.TryMath(messageText, out match))
            {
                var searchRequest =
                    $"{match.Groups[UserCommands.Kinopoisk.Groups.RusName]} {match.Groups[UserCommands.Kinopoisk.Groups.EngName]}";
                this.SearchTorrents(searchRequest, log);
            }
            else if (UserCommands.Film.Regex.TryMath(messageText, out match))
            {
                var searchRequest = match.Groups[UserCommands.Film.Groups.Name].Value;
                this.SearchTorrents(searchRequest, log);
            }
            else if (UserCommands.Torrent.Regex.TryMath(messageText, out match))
            {
                var searchRequest = match.Groups[UserCommands.Torrent.Groups.SearchRequest];
                if (searchRequest.Value.IsNullOrEmpty())
                {
                    this.waitingForUserInput = true;
                    log.Text($"Send your search request");
                    return;
                }

                this.SearchTorrents(searchRequest.Value, log);
            }
            else if (this.waitingForUserInput)
            {
                this.waitingForUserInput = false;
                this.SearchTorrents(messageText, log);
            }
            else
            {
                log.ReplyBack($"No idea what to do with your request");
                log.Log($"Received a text message in chat {message.Chat.Id}: {messageText}");
            }
        }
        

        private int GetResultIndex(TrackerCacheResult t) => this.torrents.Results.IndexOf(t) + 1;

        private void SearchTorrents(string searchRequest, ClientAndServerLogger log)
        {
            if (searchRequest.IsNullOrEmpty())
            {
                log.ReplyBack("Search request is recognized as empty");
                log.LogLastMessage();
                return;
            }
            
            var action = $"Searching torrents for {searchRequest}";
            log.TextWithAction(action, ChatAction.Typing);
            log.LogLastMessage();

            var jackett = new JackettIntegration(this.configuration.Jackett);
            try
            {
                this.torrents = null;
                this.torrents = jackett.SearchTorrents(searchRequest);
                this.torrents.Results = this.torrents.Results.OrderByDescending(r => r.Size).ToArray();

                log.Log($"Done {action}");
            }
            catch (Exception exception)
            {
                var message = $"Search of '{searchRequest}' failed";
                log.Text(exception.GetShortDescription(message));
                log.Log(exception.GetFullDescription(message));
            }

            this.ShowResults(1, log);
        }

        private void SendToBlackHoleLocation(ClientAndServerLogger log)
        {
            var response = new HttpRequestBuilder()
                .SetUrl(this.torrent.BlackholeLink.AbsoluteUri)
                .RequestAndValidate<ActionDoneResponse>(HttpMethod.Get);

            var success = response.result == "success";

            log.Text(
                $"Sending to Blackhole was{(success ? null : " NOT")} DONE "
                + $"for '{this.torrent.Title}'{(success ? null : response.ToJson())}");
            log.LogLastMessage();
            this.torrent = null;
        }

        private void ShowResults(int pageNumber, ClientAndServerLogger log)
        {
            if (this.torrents == null)
            {
                log.ReplyBack("Search results are expired, try your search again.");
                log.LogLastMessage();
                return;
            }
            if (!this.torrents.Results.Any())
            {
                log.ReplyBack(
                    $"No search results from {this.torrents.Indexers.Count} indexers " 
                    + this.torrents.Indexers.OrderBy(i => i.Name).Select(i => i.Name).JoinToString(", ") 
                    + ". Try to reword your search.");
                log.LogLastMessage();
                return;
            }
                
            var resultNumberOnPage = this.configuration.ViewFilter.ResultNumberOnPage;
            var lasPageNumber =
                (int)Math.Ceiling((double)this.torrents.Results.Count / resultNumberOnPage);
            var resultsToShow = this.torrents.Results
                .Skip(resultNumberOnPage* (pageNumber - 1))
                .Take(resultNumberOnPage).ToArray();

            InlineKeyboardButton PrepareButton(string text, int page, params int[] excludePages) =>
                page != pageNumber && page > 0 && page <= lasPageNumber
                && (!excludePages.Any() || !excludePages.Contains(page))
                    ? InlineKeyboardButton.WithCallbackData(text, $"Go {page} page")
                    : null;

            var inlineKeyboardMarkup = new InlineKeyboardMarkup(
                new[]
                    {
                        // CallbackData should be max 64bytes
                        // Data to be sent in a callback query to the bot when button is pressed, 1-64 bytes
                        resultsToShow.Select(
                            r =>
                                {
                                    var uri = r.Guid;
                                    var hashedUrl = EncryptHelper.GetMD5(uri.AbsoluteUri);
                                    this.hashToUrl.TryAdd(hashedUrl, uri);
                                    return InlineKeyboardButton.WithCallbackData(
                                        this.GetResultIndex(r).ToString(),
                                        String.Format(BotCommands.PickLocationForTorrent.Format, hashedUrl));
                                }),
                        new[]
                            {
                                PrepareButton("<<-", 1), 
                                PrepareButton("<-", pageNumber - 1, excludePages: new[] { 1, lasPageNumber }),
                                PrepareButton("->", pageNumber + 1, excludePages: new[] { 1, lasPageNumber }),
                                PrepareButton("-->", lasPageNumber, excludePages: 1)
                            }.Where(b => b != null)
                    });

            string GetReadableSize(long? size) =>
                size.HasValue
                    ? ByteSize.FromBytes(size.Value).ToString()
                    : null;

            var orderedIndexes = this.torrents.Indexers.OrderByDescending(i => i.Results).ToArray();
            log.ReplyBack(
                $"Page {pageNumber}/{lasPageNumber}" 
                + NewLine
                + $"Found {this.torrents.Results.Count} results from "
                + $"{orderedIndexes.Count(i => i.Results > 0)} indexers: "
                + orderedIndexes.Select(i => $"{i.Name} ({i.Results})").JoinToString(", ") 
                + NewLine + NewLine
                + resultsToShow.Select(
                        r => $"{this.GetResultIndex(r)}. {GetReadableSize(r.Size)} {r.Title}" 
                            + $"{NewLine}"
                            + $"{r.PublishDate:s}" 
                            + $"{NewLine}" 
                            + $"{r.Tracker} {nameof(r.Files)}={r.Files} "
                            + $"{nameof(r.Grabs)}={r.Grabs} {nameof(r.Seeders)}={r.Seeders} ")
                    .JoinToString(NewLine + NewLine),
                inlineKeyboardMarkup);
        }
    }
}