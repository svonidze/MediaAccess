namespace MediaServer.Workflow
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    using BitTorrent.Contracts;

    using ByteSizeLib;

    using Common.Collections;
    using Common.Cryptography;
    using Common.Exceptions;
    using Common.Serialization.Json;
    using Common.System;
    using Common.Text;

    using Jackett.Contracts;

    using JetBrains.Annotations;

    using MediaServer.Contracts;
    using MediaServer.Workflow.Constants;

    using Telegram.Bot.Types;
    using Telegram.Bot.Types.Enums;
    using Telegram.Bot.Types.ReplyMarkups;

    public class TelegramChatListener : ITelegramChatListener
    {
        private static readonly string NewLine = Environment.NewLine;

        private readonly IJackettIntegration jackett;

        private readonly IBitTorrentClient bitTorrentClient;

        private readonly ViewFilterConfiguration viewFilterConfig;

        private readonly ConcurrentDictionary<string, Uri> hashToUrl = new ConcurrentDictionary<string, Uri>();

        private ManualSearchResult torrents;

        private TrackerCacheResult torrent;

        private bool waitingForUserInput;

        public TelegramChatListener(
            [NotNull] ViewFilterConfiguration viewFilterConfig,
            [NotNull] IJackettIntegration jackett,
            [NotNull] IBitTorrentClient bitTorrentClient)
        {
            this.viewFilterConfig = viewFilterConfig;
            this.jackett = jackett;
            this.bitTorrentClient = bitTorrentClient;
        }

        public void Handle(string queryData, ITelegramClientAndServerLogger log)
        {
            var actions = new Dictionary<Regex, Action<Match>>
                {
                    { BotCommands.PickLocationForTorrent.Regex, PickLocationForTorrent },
                    { BotCommands.StartTorrent.Regex, StartTorrent },
                    { BotCommands.GoToPage.Regex, GoToPage },
                    { BotCommands.SortResults.Regex, SortResults }
                };
            
            void PickLocationForTorrent(Match match)
            {
                if (!this.bitTorrentClient.IsSetUp)
                {
                    log.ReplyBack(
                        $"{nameof(this.bitTorrentClient)} is not set up. The command {nameof(BotCommands.PickLocationForTorrent)} cannot be executed.");
                    log.LogLastMessage();
                    return;
                }

                var downloadLocations = this.bitTorrentClient.ListDownloadLocations();
                if (downloadLocations == null || !downloadLocations.Any())
                {
                    log.ReplyBack(
                        $"No {nameof(downloadLocations)} are set up. The command {nameof(BotCommands.PickLocationForTorrent)} cannot be executed.");
                    log.LogLastMessage();
                    return;
                }

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

                log.ReplyBack(
                    $"Pick download location for torrent #{this.GetResultIndex(this.torrent)}",
                    new InlineKeyboardMarkup(
                        downloadLocations.Select(
                            dl => InlineKeyboardButton.WithCallbackData(dl, string.Format(BotCommands.StartTorrent.Format, dl)))));
            }

            void StartTorrent(Match match)
            {
                if (!this.bitTorrentClient.IsSetUp)
                {
                    log.ReplyBack(
                        $"{nameof(this.bitTorrentClient)} is not ste up. The command {nameof(BotCommands.StartTorrent)} cannot be executed.");
                    log.LogLastMessage();
                    return;
                }

                if (this.torrent == null)
                {
                    log.Text("No torrent is picked");
                    log.LogLastMessage();
                    return;
                }

                var location = match.Groups[BotCommands.StartTorrent.Groups.HashUrl].Value;

                try
                {
                    var newTorrent = this.bitTorrentClient.AddTorrent(
                        new NewTorrent
                            {
                                FileName = this.torrent.Link.AbsoluteUri,
                                DownloadDirectory = location
                            });

                    log.ReplyBack($"Torrent '{newTorrent.Name}' was added");
                    log.Log($"Torrent '{newTorrent.Name}' was added {newTorrent.ToJson()}");
                }
                catch (Exception exception)
                {
                    var text = $"Torrent '{this.torrent.Title}' could not be added";
                    log.ReplyBack(exception.GetShortDescription(text));
                    log.Log(exception.GetFullDescription(text));
                }
            }

            void GoToPage(Match match)
            {
                var pageString = match.Groups[BotCommands.GoToPage.Groups.Page].Value;
                if (!int.TryParse(pageString, out var page))
                {
                    log.Text(
                        $"Could not parse '{queryData}' as a pagination command. Page group could not recognized as {nameof(Int32)}");
                    log.LogLastMessage();
                    return;
                }

                this.ShowResults(log, page);
            }

            void SortResults(Match match)
            {
                var sortingTypeString = match.Groups[BotCommands.SortResults.Groups.SortingType].Value;
                if (!sortingTypeString.TryParseToEnum<SortingType>(out var sortingType))
                {
                    log.Text($"Could not parse '{queryData}' as {nameof(SortingType)}");
                    log.LogLastMessage();
                    return;
                }

                if (this.viewFilterConfig.SortBy == sortingType)
                {
                    this.viewFilterConfig.Ascending = !this.viewFilterConfig.Ascending;
                }
                else
                {
                    this.viewFilterConfig.SortBy = sortingType;
                }

                this.ShowResults(log);
            }

            
            foreach (var regex in actions.Keys)
            {
                if (!regex.TryMath(queryData, out var match))
                    continue;
                
                actions[regex](match);
                return;
            }

            log.Log($"'{queryData}' is not supported");
            log.Text($"Your action '{queryData}' is not supported. Maybe you click old buttons?");
        }

        public void Handle(Message message, ITelegramClientAndServerLogger log)
        {
            var messageText = message.Text;
            if (messageText == null)
            {
                log.Text("I can understand only text");
                return;
            }

            var actions = new Dictionary<Regex, Action<Match>>
                {
                    { UserCommands.StartBotCommunication.Regex, StartBotCommunication },
                    { UserCommands.Kinopoisk.Regex, Kinopoisk },
                    { UserCommands.Film.Regex, Film },
                    { UserCommands.Torrent.Regex, Torrent },
                };
            
            void StartBotCommunication(Match match)
            {
                log.Text(
                    "Greeting! Here you can search for torrents and send them to a favorite BitTorrent client. Lets start with typing /t or /torrent.");
                log.Log(match.Value);
            }

            void Kinopoisk(Match match)
            {
                var searchRequest =
                    $"{match.Groups[UserCommands.Kinopoisk.Groups.RusName]} {match.Groups[UserCommands.Kinopoisk.Groups.EngName]}";
                this.SearchTorrents(searchRequest, log);
            }

            void Film(Match match)
            {
                var searchRequest = match.Groups[UserCommands.Film.Groups.Name].Value;
                this.SearchTorrents(searchRequest, log);
            }

            void Torrent(Match match)
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


            foreach (var regex in actions.Keys)
            {
                if (!regex.TryMath(messageText, out var match))
                    continue;
                
                actions[regex](match);
                return;
            }
                
            if (this.waitingForUserInput)
            {
                this.waitingForUserInput = false;
                this.SearchTorrents(messageText, log);
            }
            else
            {
                log.ReplyBack($"No idea what to do with your request");
                log.Log($"Received a text message in chat {message.Chat.Id}: '{messageText}'");
            }
        }
        
        private int GetResultIndex(TrackerCacheResult t) => this.torrents.Results.IndexOf(t) + 1;

        private void SearchTorrents(string searchRequest, ITelegramClientAndServerLogger log)
        {
            if (searchRequest.IsNullOrEmpty())
            {
                log.ReplyBack($"{nameof(searchRequest)} is recognized as empty");
                log.LogLastMessage();
                return;
            }
            
            var action = $"Searching torrents for {searchRequest}";
            log.TextWithAction(action, ChatAction.Typing);
            log.LogLastMessage();

            try
            {
                this.torrents = null;
                this.torrents = this.jackett.SearchTorrents(searchRequest);

                log.Log($"Done {action}");
            }
            catch (Exception exception)
            {
                var message = $"Search of '{searchRequest}' failed";
                log.Text(exception.GetShortDescription(message));
                log.Log(exception.GetFullDescription(message));
            }

            this.ShowResults(log);
        }

        private void ShowResults(ITelegramClientAndServerLogger log, int pageNumber = 1)
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

            if (pageNumber < 1)
            {
                log.Text($"{nameof(pageNumber)} is less than 1");
                log.LogLastMessage();
                return;
            }

            var resultNumberOnPage = this.viewFilterConfig.ResultNumberOnPage;
            var lasPageNumber =
                (int)Math.Ceiling((double)this.torrents.Results.Count / resultNumberOnPage);

            TrackerCacheResult[] FilterAndSortResults()
            {
                Func<TrackerCacheResult, object> sort;
                var sortingType = this.viewFilterConfig.SortBy;
                switch (sortingType)
                {
                    case SortingType.Default:
                    case SortingType.Size:
                        sort = r => r.Size;
                        break;
                    case SortingType.Date:
                        sort = r => r.PublishDate;
                        break;
                    case SortingType.Title:
                        sort = r => r.Title;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(sortingType), sortingType, message: null);
                }

                var query = this.torrents.Results.AsEnumerable();

                query = this.viewFilterConfig.Ascending
                    ? query.OrderBy(sort)
                    : query.OrderByDescending(sort);

                return query.Skip(resultNumberOnPage * (pageNumber - 1)).Take(resultNumberOnPage).ToArray();
            }

            InlineKeyboardButton PrepareGoToButton(Emoji emoji, int page, params int[] excludePages) =>
                page == pageNumber || page <= 0 || page > lasPageNumber || excludePages.Any() && excludePages.Contains(page)
                    ? null
                    : InlineKeyboardButton.WithCallbackData(emoji.ToUnicode(), string.Format(BotCommands.GoToPage.Format, page));

            Emoji SortingDirectionEmoji(SortingType st) =>
                this.viewFilterConfig.SortBy == st && this.viewFilterConfig.Ascending
                    ? Emojies.UpwardsBlackArrow
                    : Emojies.DownwardsBlackArrow;

            var resultsToShow = FilterAndSortResults();
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
                                        string.Format(BotCommands.PickLocationForTorrent.Format, hashedUrl));
                                }),
                        new[]
                            {
                                PrepareGoToButton(Emojies.LastTrackButton, 1), 
                                PrepareGoToButton(Emojies.ReverseButton, pageNumber - 1, excludePages: new[] { 1, lasPageNumber }),
                                PrepareGoToButton(Emojies.PlayButton, pageNumber + 1, excludePages: new[] { 1, lasPageNumber }),
                                PrepareGoToButton(Emojies.NextTrackButton, lasPageNumber, excludePages: 1)
                            }.Where(b => b != null),
                        EnumUtils.EnumToList<SortingType>()
                            .Except(SortingType.Default)
                            .Select(st => InlineKeyboardButton.WithCallbackData(
                                $"{SortingDirectionEmoji(st).ToUnicode()} {st}",
                                string.Format(BotCommands.SortResults.Format, st)))
                        
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
                        r => $"{this.GetResultIndex(r)}. {GetReadableSize(r.Size)} {r.CategoryDesc} {r.Title}" 
                            + NewLine
                            + $"{r.PublishDate:s}" 
                            + NewLine 
                            + $"{r.Tracker} {nameof(r.Files)}={r.Files} "
                            + $"{nameof(r.Grabs)}={r.Grabs} {nameof(r.Seeders)}={r.Seeders} ")
                    .JoinToString(NewLine + NewLine),
                inlineKeyboardMarkup);
        }
    }
}
