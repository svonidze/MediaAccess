namespace MediaServer.Workflow
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Text.RegularExpressions;

    using BitTorrent.Contracts;

    using ByteSizeLib;

    using Common.Collections;
    using Common.Cryptography;
    using Common.Exceptions;
    using Common.Http;
    using Common.Serialization.Json;
    using Common.System;
    using Common.Text;

    using Jackett.Contracts;

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

        private readonly IDictionary<string, Uri> hashToUrl;

        private ManualSearchResult? torrents;

        private TrackerCacheResult? torrent;

        private bool waitingForUserInput;

        public TelegramChatListener(
            ViewFilterConfiguration viewFilterConfig,
            IJackettIntegration jackett,
            IBitTorrentClient bitTorrentClient)
        {
            this.viewFilterConfig = viewFilterConfig.DeepCopy();
            this.jackett = jackett;
            this.bitTorrentClient = bitTorrentClient;
            
            this.hashToUrl = new ConcurrentDictionary<string, Uri>();
        }

        public void HandleBotInput(string queryData, ITelegramClientAndServerLogger log)
        {
            var actions = new Dictionary<Regex, Action<Match>>
                {
                    { BotCommands.PickTorrent.Regex, PickTorrent },
                    { BotCommands.StartTorrent.Regex, StartTorrent },
                    { BotCommands.GoToPage.Regex, GoToPage },
                    { BotCommands.SortResults.Regex, SortResults },
                    { BotCommands.FilterTrackerResults.Regex, FilterTrackerResults },
                    { BotCommands.DownloadTorrentFile.Regex, DownloadTorrentFile },
                };
         
            bool TryFindTorrentInLocalResults(Match match, out string hashedUri, out TrackerCacheResult? torrentCandidate)
            {
                torrentCandidate = default;
                
                hashedUri = match.Groups[BotCommands.PickTorrent.Groups.HashUrl].Value;
                if (!this.hashToUrl.TryGetValue(hashedUri, out var uri))
                {
                    log.Text($"Search results are expired, repeat your search");
                    log.Log($"Uri not found by hash '{hashedUri}'");
                    return false;
                }

                var torrentCandidates = this.torrents.Results.Where(t => t.Guid == uri).ToArray();
                if (!torrentCandidates.Any())
                {
                    log.Text("Found no torrents, probably Search results are expired, repeat your search");
                    log.Log(
                        $"Torrent could not be found with URL={uri.LocalPath}, "
                        + $"last search has {this.torrents?.Results?.Count} results");
                    return false;
                }

                if (torrentCandidates.Length > 1)
                {
                    log.Text(
                        "Strange, found more than one torrent with the same characteristics, "
                        + "probably search results are corrupted, repeat your search");
                    log.Log(
                        $"Found {torrentCandidates.Length} torrents with URL={uri.AbsoluteUri}, "
                        + $"last search has {this.torrents?.Results?.Count} results");
                    return false;
                }

                torrentCandidate = torrentCandidates.Single();
                return true;
            }
            
            void DownloadTorrentFile(Match match)
            {
                if (!TryFindTorrentInLocalResults(match, out _, out this.torrent))
                {
                    return;
                }
                
                log.TrySendDocumentBackAsync(this.torrent.Link);
            }
            
            void PickTorrent(Match match)
            {
                if (!TryFindTorrentInLocalResults(match, out var hashedUri, out this.torrent))
                    return;

                var downloadLocations = (this.bitTorrentClient.IsSetUp
                    ? this.bitTorrentClient.ListDownloadLocations()
                    : default) ?? Array.Empty<string>();

                log.ReplyBack(
                    $"¿What to do with torrent #{this.GetResultIndex(this.torrent)} '{this.torrent.Title}'?",
                    new InlineKeyboardMarkup(
                        new[]
                            {
                                downloadLocations.Select(
                                    dl => InlineKeyboardButton.WithCallbackData(
                                        dl,
                                        string.Format(BotCommands.StartTorrent.Format, dl))),
                                new[]
                                    {
                                        InlineKeyboardButton.WithUrl($"Open {this.torrent.Tracker}", this.torrent.Guid.AbsoluteUri),
                                        InlineKeyboardButton.WithCallbackData("Download locally", string.Format(BotCommands.DownloadTorrentFile.Format, hashedUri)),
                                    }
                            }));
            }

            void StartTorrent(Match match)
            {
                if (!this.bitTorrentClient.IsSetUp)
                {
                    log.ReplyBack(
                        $"{nameof(this.bitTorrentClient)} is not set up. "
                        + $"The command {nameof(BotCommands.StartTorrent)} cannot be executed.");
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

                    string GetMessage(string torrentInfo) => $"Torrent '{torrentInfo}' will be downloaded to {location}.";
                    log.ReplyBack(GetMessage(newTorrent.Name));
                    log.Log(GetMessage(newTorrent.ToJson()));
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
            
            void FilterTrackerResults(Match match)
            {
                var trackerName = match.Groups[BotCommands.FilterTrackerResults.Groups.TrackerName].Value;

                if (!trackerName.IsNullOrEmpty())
                {
                    Console.WriteLine($"Filtering by {trackerName}");
                    this.viewFilterConfig.TrackerName = trackerName;
                }
                
                this.ShowResults(log);
            }

            foreach (var regex in actions.Keys)
            {
                if (!regex.TryMath(queryData, out var match))
                {
                    continue;
                }
                
                actions[regex](match);
                return;
            }

            log.Log($"'{queryData}' is not supported");
            log.Text($"Your action '{queryData}' is not supported. Maybe you click old buttons?");
        }

        public void HandleUserInput(Message message, ITelegramClientAndServerLogger log)
        {
            var messageText = message.Text;
            if (messageText == null)
            {
                log.Text("I can understand only text");
                return;
            }

            this.HandleUserInput(messageText, log);
        }

        private void HandleUserInput(string messageText, ITelegramClientAndServerLogger log)
        {
            var actions = new Dictionary<Regex, Action<Match>>
                {
                    { UserCommands.StartBotCommunication.Regex, HandleStartBotCommunication },
                    { UserCommands.Kinopoisk.Regex, HandleKinopoisk },
                    { UserCommands.Film.Regex, HandleFilm },
                    { UserCommands.Torrent.Regex, HandleTorrent },
                    { Patterns.Http.Regex, HandleHttp },
                };
            
            void HandleStartBotCommunication(Match match)
            {
                log.Text(
                    "Greeting! " + NewLine 
                    + "Here you can search torrents, download and send them to your favorite BitTorrent client " 
                    + "(requires to set up your private BitTorrent client and Telegram bot). " + NewLine 
                    + $"Lets start with typing {UserCommands.Torrent.Commands.JoinToString(" or ")}." + NewLine
                    + "For more details see https://github.com/svonidze/MediaAccess");
                log.Log(match.Value);
            }

            void HandleKinopoisk(Match match)
            {
                var searchRequest =
                    $"{match.Groups[UserCommands.Kinopoisk.Groups.RusName]} {match.Groups[UserCommands.Kinopoisk.Groups.EngName]}";
                this.SearchTorrents(log, searchRequest);
            }
            
            void HandleHttp(Match match)
            {
                var url = match.Value;
                var httpRequestBuilder = new HttpRequestBuilder().SetUrl(url);
                var html = httpRequestBuilder.DownloadString();

                if (!Patterns.Html.TitleRegex.TryMath(html, out var titleMatch))
                {
                    log.ReplyBack($"Could not extract {nameof(Patterns.Html)}");
                    return;
                }

                var title = match.Groups[Patterns.Html.Groups.Title].Value;
                this.HandleUserInput(title, log);
            }

            void HandleFilm(Match match)
            {
                var searchRequest = match.Groups[UserCommands.Film.Groups.Name].Value;
                this.SearchTorrents(log, searchRequest);
            }
            
            void StartTorrentSearch(string input)
            {
                if (UserCommands.SearchRequest.Regex.TryMath(input, out var match))
                {
                    var searchRequest = match.Groups[UserCommands.SearchRequest.Groups.Input];
                    var trackerName = match.Groups[UserCommands.SearchRequest.Groups.TrackerName];
                    this.SearchTorrents(log, searchRequest.Value, trackerName.Value);
                }
                else
                {
                    this.SearchTorrents(log, input);
                }
            }
            
            void HandleTorrent(Match match)
            {
                var searchRequest = match.Groups[UserCommands.Torrent.Groups.Input];
                if (searchRequest.Value.IsNullOrEmpty())
                {
                    this.waitingForUserInput = true;
                    log.Text("Send your search request");
                    return;
                }

                StartTorrentSearch(searchRequest.Value);
            }

            foreach (var regex in actions.Keys)
            {
                if (!regex.TryMath(messageText, out var match))
                {
                    continue;
                }
                
                actions[regex](match);
                return;
            }
                
            if (this.waitingForUserInput)
            {
                this.waitingForUserInput = false;
                StartTorrentSearch(messageText);
            }
            else
            {
                log.ReplyBack($"No idea what to do with your request. Type {UserCommands.StartBotCommunication.Command} for intro.");
                log.Log($"said '{messageText}'");
            }
        }
        
        
        private int GetResultIndex(TrackerCacheResult t) => this.torrents.Results.IndexOf(t) + 1;

        private void SearchTorrents(
            ITelegramClientAndServerLogger log,
            string searchRequest,
            string? trackerName = null)
        {
            if (searchRequest.IsNullOrEmpty())
            {
                log.ReplyBack($"{nameof(searchRequest)} is recognized as empty");
                log.LogLastMessage();
                return;
            }
            
            var action = $"Searching torrents for '{searchRequest}'";
            if (!trackerName.IsNullOrEmpty())
            {
                action += $" in {trackerName}";
            }
            
            log.TextWithAction(action, ChatAction.Typing);
            log.LogLastMessage();

            try
            {
                this.torrents = null;
                this.torrents = this.jackett.SearchTorrents(searchRequest, trackerName);

                log.Log($"Done {action}: " 
                    + $"Found {this.torrents?.Results.Count} results from "
                    + $"{this.torrents?.Indexers.Count(i => i.Results > 0)}/{this.torrents?.Indexers?.Count} indexers.");
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
                    + this.torrents.Indexers
                        .OrderBy(i => i.Name)
                        .Select(i => $"{i.Name}{(i.Error.IsNullOrEmpty()
                            ? null
                            : "(Failed)")}")
                        .JoinToString(", ") 
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
                Func<TrackerCacheResult, object?> sort;
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

                if (!this.viewFilterConfig.TrackerName.IsNullOrEmpty())
                {
                    query = query.Where(q => q.Tracker == this.viewFilterConfig.TrackerName);
                }
                
                query = this.viewFilterConfig.Ascending
                    ? query.OrderBy(sort)
                    : query.OrderByDescending(sort);

                return query.Skip(resultNumberOnPage * (pageNumber - 1)).Take(resultNumberOnPage).ToArray();
            }

            InlineKeyboardButton? PrepareGoToButton(Emoji emoji, int page, params int[] excludePages) =>
                page == pageNumber || page <= 0 || page > lasPageNumber || excludePages.Any() && excludePages.Contains(page)
                    ? null
                    : InlineKeyboardButton.WithCallbackData(emoji.ToUnicode(), string.Format(BotCommands.GoToPage.Format, page));

            Emoji SortingDirectionEmoji(SortingType st) =>
                this.viewFilterConfig.SortBy == st && this.viewFilterConfig.Ascending
                    ? Emojies.UpwardsBlackArrow
                    : Emojies.DownwardsBlackArrow;

            var resultsToShow = FilterAndSortResults();

            string? Format(string key, object? value) =>
                value is null
                    ? null
                    : $"{key}={value}";

            var torrentButtons = resultsToShow.Select(
                r =>
                    {
                        // CallbackData should be max 64bytes
                        // Data to be sent in a callback query to the bot when button is pressed, 1-64 bytes
                        var uri = r.Guid;
                        var hashedUrl = EncryptHelper.GetMD5(uri.AbsoluteUri);
                        this.hashToUrl.TryAdd(hashedUrl, uri);
                        return InlineKeyboardButton.WithCallbackData(
                            GetReadableSize(r.Size),
                            string.Format(BotCommands.PickTorrent.Format, hashedUrl));
                    });
        
            var navigationButtons = new[]
                {
                    PrepareGoToButton(Emojies.LastTrackButton, 1), 
                    PrepareGoToButton(Emojies.ReverseButton, pageNumber - 1, excludePages: new[] { 1, lasPageNumber }),
                    PrepareGoToButton(Emojies.PlayButton, pageNumber + 1, excludePages: new[] { 1, lasPageNumber }),
                    PrepareGoToButton(Emojies.NextTrackButton, lasPageNumber, excludePages: 1)
                }.Where(b => b != null);

            var trackerButtons = new List<InlineKeyboardButton>();
            var trackers = this.torrents.Indexers.Where(i => i.Results > 0).ToArray();
            if (trackers.Length > 1)
            {
                var collection = new NameValueCollection
                    {
                        { "All", string.Empty }
                    };
                trackers.Select(r => r.Name).Distinct().Foreach(t => collection.Add(t, t));

                trackerButtons.AddRange(
                    collection.AllKeys.Select(
                        t => InlineKeyboardButton.WithCallbackData(
                            t,
                            string.Format(BotCommands.FilterTrackerResults.Format, collection[t]))));
            }
            
            var sortingButtons = EnumUtils.EnumToList<SortingType>()
                .Except(SortingType.Default)
                .Select(st => InlineKeyboardButton.WithCallbackData(
                    $"{SortingDirectionEmoji(st).ToUnicode()} {st}",
                    string.Format(BotCommands.SortResults.Format, st)));

            var inlineKeyboardMarkup = new InlineKeyboardMarkup(new []
                {
                    torrentButtons,
                    navigationButtons,
                    trackerButtons,
                    sortingButtons
                });

            string? GetReadableSize(long? size) =>
                size.HasValue
                    ? ByteSize.FromBytes(size.Value).ToString()
                    : null;

            var orderedIndexes = this.torrents.Indexers.OrderByDescending(i => i.Results).ToArray();
            log.ReplyBack(
                $"Page {pageNumber}/{lasPageNumber}" 
                + NewLine 
                + $"Found {this.torrents.Results.Count} results from "
                + $"{orderedIndexes.Count(i => i.Results > 0)}/{orderedIndexes.Length} indexers: " + orderedIndexes
                    .Select(
                        i => $"{i.Name} ({(i.Error.IsNullOrEmpty()
                            ? i.Results
                            : "Failed")})")
                    .JoinToString(", ") 
                + NewLine
                + resultsToShow.Select(
                    r => $"{this.GetResultIndex(r)}. " 
                        + $"{GetReadableSize(r.Size)} " 
                        + $"{r.CategoryDesc} "
                        + $"{r.Title}" 
                        + NewLine 
                        + $"{r.PublishDate:yyyy-MM-dd}" 
                        + NewLine 
                        + $"{r.Tracker} " 
                        + new[]
                            {
                                Format(nameof(r.Files), r.Files), 
                                Format(nameof(r.Grabs), r.Grabs),
                                Format(nameof(r.Seeders), r.Seeders),
                                Format(nameof(r.Peers), r.Peers),
                            }.Where(x => x is not null)
                            .JoinToString(" "))
                    .JoinToString(NewLine + NewLine),
                inlineKeyboardMarkup);
        }
    }
}