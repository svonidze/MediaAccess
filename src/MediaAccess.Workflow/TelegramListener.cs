namespace MediaServer.Workflow
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Text.RegularExpressions;
    using System.Threading;

    using ByteSizeLib;

    using Common.Collections;
    using Common.Cryptography;
    using Common.Exceptions;
    using Common.Http;
    using Common.Serialization.Json;

    using Jackett.Contracts;

    using MediaServer.Contracts;

    using Telegram.Bot;
    using Telegram.Bot.Args;
    using Telegram.Bot.Types;
    using Telegram.Bot.Types.Enums;
    using Telegram.Bot.Types.ReplyMarkups;

    // TODO use log library
    public class TelegramListener
    {
        private static readonly string NewLine = Environment.NewLine;

        private readonly ITelegramBotClient botClient;

        private readonly Configuration configuration;
        
        private ManualSearchResult torrents;

        private readonly ConcurrentDictionary<string, Uri> hashToUrl = new ConcurrentDictionary<string, Uri>();

        private static readonly ChatSettings ChatSettings = new ChatSettings
            {
                PageResultNumber = 5,
                OrderBy = "Size",
                SortingDirection = "DESC",
                MaxSize = null,
                MinSize = "1 GB"
            };
        
        public TelegramListener(Configuration configuration)
        {
            this.configuration = configuration;
            
            var proxy = configuration.Proxy;
            this.botClient = new TelegramBotClient(
                configuration.TelegramBot.Token,
                new WebProxy(proxy.Host, proxy.Port)
                    {
                        Credentials = new NetworkCredential(proxy.UserName, proxy.Password)
                    });
        }

        public void StartReceiving()
        {
            var me = this.botClient.GetMeAsync();
                
            this.botClient.OnMessage += this.Bot_OnMessage;
            this.botClient.OnCallbackQuery += this.Bot_OnCallbackQuery;
            this.botClient.StartReceiving();

            Console.WriteLine($"{nameof(this.StartReceiving)} is run for bot {me.Result.ToJson()}");
        }

        public void StopReceiving()
        {
            this.botClient.OnMessage -= this.Bot_OnMessage;
            this.botClient.OnCallbackQuery -= this.Bot_OnCallbackQuery;
            this.botClient.StopReceiving();
        }

        private void Bot_OnCallbackQuery(object sender, CallbackQueryEventArgs e)
        {
            var log = new Logger(this.botClient, e.CallbackQuery.Message);

            var queryData = e.CallbackQuery.Data;
            if (queryData.StartsWith("Call"))
            {
                const string ActionName = "Sending to Blackhole";

                var hashedUri = queryData.Replace("Call ", null);
                if (!this.hashToUrl.TryGetValue(hashedUri, out var uri))
                {
                    log.Log($"Uri not found by hash '{hashedUri}'");
                    log.Text($"Search results are expired, repeat your search");
                    return;
                }

                var result = this.torrents.Results.Single(t => t.Guid == uri);
                var response = new HttpRequestBuilder().SetUrl(result.BlackholeLink.AbsoluteUri)
                    .RequestAndValidate<ActionDoneResponse>(HttpMethod.Get);
                var success = response.result == "success";
                
                log.Text(
                    $"{ActionName} {(success ? "done" : "not done")} " 
                    + $"for '{result.Title}'{(success ? null : response.ToJson())}");
                log.LogLastMessage();
            }
            else if (queryData.StartsWith("Go"))
            {
                var regex = new Regex(@"Go (?<page>\d+) page");
                var match = regex.Match(queryData);
                if (!match.Success)
                {
                    log.Text($"Could not parse {queryData} as a pagination command");
                    log.LogLastMessage();
                    return;
                }

                var matchGroup = match.Groups["page"];
                if (!matchGroup.Success || !int.TryParse(matchGroup.Value, out var page))
                {
                    log.Text($"Could not parse '{queryData}' as a pagination command. Page group could not recognized as {nameof(Int32)}");
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

        private async void Bot_OnMessage(object sender, MessageEventArgs e)
        {
            var log = new Logger(this.botClient, e.Message);

            var allowedChats = this.configuration.TelegramBot.AllowedChats;
            if (allowedChats != null && !allowedChats.Contains(e.Message.Chat.Id))
            {
                log.ReplyBack("You are not allowed to use this Bot instance. Run your own!");
                Console.WriteLine($"{e.Message.Chat.Username} in chat {e.Message.Chat.Id} tried to access the Bot. Chat is not allowed.");
                return;
            }
            var messageText = e.Message.Text;
            if (messageText == null)
            {
                log.Text("I can understand only text");
                return;
            }

            if (messageText.StartsWith("Фильм "))
            {
                Console.WriteLine("Detected Kinopois");

                // Фильм "Во все тяжкие" ("Breaking Bad", 2008-2013) #kinopoisk
                // http://www.kinopoisk.ru/film/404900/
                var regex = new Regex(@"Фильм ""(?<rusName>.+)"" \(""(?<engName>.+)"", (?<years>\d+-?\d*)\)");

                var match = regex.Match(messageText);
                if (!match.Success)
                {
                    log.Text($"Cannot parse what you requested{Environment.NewLine}{messageText}");
                    return;
                }

                var searchRequest = match.Groups["rusName"] + " " + match.Groups["engName"];
                this.SearchTorrents(searchRequest, log);
            }
            else if (messageText.StartsWith("/torrent"))
            {
                Console.WriteLine("Searching torrents");

                var regex = new Regex(@"/torrent(?<botName>@\w+)? (?<searchRequest>.+)");
                var match = regex.Match(messageText);

                if (!match.Success)
                {
                    log.ReplyBack("Cant parse what you requested.");
                    return;
                }

                var searchRequest = match.Groups["searchRequest"];
                if (searchRequest.Value.IsNullOrEmpty())
                {
                    log.ReplyBack($"You requested search for nothing!");
                    return;
                }

                this.SearchTorrents(searchRequest.Value, log);
            }
            else
            {
                log.ReplyBack($"No idea what to do");
                Console.WriteLine(
                    $"Received a text message in chat {e.Message.Chat.Id}.{Environment.NewLine}{messageText}");
            }
        }

        int GetResultIndex(TrackerCacheResult r) =>  this.torrents.Results.IndexOf(r) + 1;

        private async void SearchTorrents(string searchRequest, Logger bot)
        {
            bot.TextWithAction($"Searching for {searchRequest}", ChatAction.Typing);
            var jackett = new JackettIntegration(this.configuration.Jackett);

            try
            {
                this.torrents = jackett.SearchTorrents(searchRequest);
                this.torrents.Results = this.torrents.Results.OrderByDescending(r => r.Size).ToArray();
                
                Console.WriteLine("Search is done");
            }
            catch (Exception exception)
            {
                bot.Text(exception.GetShortDescription($"Search of '{searchRequest}'failed"));
                Console.WriteLine(exception);
            }
            
            this.ShowResults(1, bot);
        }

        private void ShowResults(int pageNumber, Logger bot)
        {
            if (this.torrents == null)
            {
                bot.ReplyBack("No search results, try your search again.");
                bot.LogLastMessage();
                return;
            }
            
            int lasPageNumber = (this.torrents.Results.Count / ChatSettings.PageResultNumber) + 1;
            var resultsToShow = this.torrents.Results
                .Skip(ChatSettings.PageResultNumber * (pageNumber - 1))
                .Take(ChatSettings.PageResultNumber).ToArray();

            InlineKeyboardButton PrepareButton(string text, int page, params int[] excludePages) =>
                page != pageNumber && page > 0 && page <= lasPageNumber && (!excludePages.Any() || !excludePages.Contains(page))
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
                                    return InlineKeyboardButton.WithCallbackData($"{this.GetResultIndex(r)}", $"Call {hashedUrl}");
                                }),
                        new[]
                            {
                                PrepareButton("<<-", 1),
                                PrepareButton("<-", pageNumber - 1, excludePages: new []{ 1, lasPageNumber}),
                                PrepareButton("->", pageNumber + 1, excludePages: new []{ 1, lasPageNumber}),
                                PrepareButton("-->", lasPageNumber, excludePages: 1),
                            }.Where(b => b != null)
                    });

            string GetReadableSize(long? size) =>
                size.HasValue
                    ? ByteSize.FromBytes(size.Value).ToString()
                    : null;

            var orderedIndexes = this.torrents.Indexers.OrderByDescending(i => i.Results).ToArray();
            bot.ReplyBack(
                resultsToShow
                    .Select(
                        r => $"{this.GetResultIndex(r)}. {GetReadableSize(r.Size)} {r.Title}"
                            + $"{NewLine}{r.Tracker} {nameof(r.Grabs)}:{r.Grabs} "
                            + $"{nameof(r.Seeders)}:{r.Seeders} {r.PublishDate}").JoinToString(NewLine + NewLine)
                + NewLine + NewLine
                + $"Page {pageNumber}/{lasPageNumber}"
                + NewLine
                + $"Found {this.torrents.Results.Count} results from "
                + $"{orderedIndexes.Count(i => i.Results > 0)} indexers: "
                + orderedIndexes.Select(i => $"{i.Name} ({i.Results})").JoinToString(", ") 
                
                ,
                replyMarkup: inlineKeyboardMarkup);
        }

        private class Logger
        {
            private readonly ITelegramBotClient botClient;

            private readonly Message message;

            private string lastText;
            
            public Logger(ITelegramBotClient botClient, Message message)
            {
                this.botClient = botClient;
                this.message = message;
            }

            public async void Text(string text)
            {
                this.lastText = text;
                await botClient.SendTextMessageAsync(this.message.Chat, text);
            }

            public async void TextWithAction(
                string text,
                ChatAction chatAction,
                CancellationToken cancellationToken = default)
            {
                this.Text(text);
                await this.botClient.SendChatActionAsync(this.message.Chat, chatAction, cancellationToken);
            }

            public async void ReplyBack(string text, IReplyMarkup replyMarkup = default)
            {
                this.lastText = text;

                await this.botClient.SendTextMessageAsync(
                    this.message.Chat,
                    text,
                    ParseMode.Default, //be careful with Markdown, it waits that special characters be closed
                    replyToMessageId: this.message.MessageId,
                    replyMarkup: replyMarkup);
            }

            public async void LogLastMessage()
            {
                this.Log(this.lastText);
            }
            
            public async void Log(string text)
            {
                Console.WriteLine(text);
            }
        }
    }
}