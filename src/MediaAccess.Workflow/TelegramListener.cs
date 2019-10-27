namespace MediaServer.Workflow
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Text.RegularExpressions;
    using System.Threading;

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

        private ManualSearchResult results;

        private readonly ConcurrentDictionary<string, Uri> hashToUrl = new ConcurrentDictionary<string, Uri>();

        public TelegramListener(Configuration configuration)
        {
            this.configuration = configuration;
            
            var proxy = configuration.Proxy;
            this.botClient = new TelegramBotClient(
                configuration.TelegramBotToken,
                new WebProxy(proxy.Host, proxy.Port)
                    {
                        Credentials = new NetworkCredential(proxy.UserName, proxy.Password)
                    });
        }

        public void StartReceiving()
        {
            var me = this.botClient.GetMeAsync();
                
            this.botClient.OnMessage += this.Bot_OnMessage;
            this.botClient.OnCallbackQuery += this.BotClientOnOnCallbackQuery;
            this.botClient.StartReceiving();

            Console.WriteLine($"{nameof(this.StartReceiving)} is run for bot {me.Result.ToJson()}");
        }

        public void StopReceiving()
        {
            this.botClient.OnMessage -= this.Bot_OnMessage;
            this.botClient.OnCallbackQuery -= this.BotClientOnOnCallbackQuery;
            this.botClient.StopReceiving();
        }

        private void BotClientOnOnCallbackQuery(object sender, CallbackQueryEventArgs e)
        {
            var r = new Responser(botClient, e.CallbackQuery.Message);

            var queryData = e.CallbackQuery.Data;
            if (queryData.StartsWith("Call"))
            {
                var hashedUri = queryData.Replace("Call ", null);
                if (!hashToUrl.TryGetValue(hashedUri, out var uri))
                {
                    Console.WriteLine($"Uri not found by hash '{hashedUri}'");
                    return;
                }

                var result = results.Results.Single(t => t.Guid == uri);
                var response = new HttpRequestBuilder().SetUrl(result.BlackholeLink.AbsoluteUri)
                    .RequestAndValidate<ActionDoneResponse>(HttpMethod.Get);

                var actionName = "Sending to Blackhole";
                if (response.result == "success")
                {
                    r.Text($":white_check_mark: {actionName} done. '{result.Title}'");
                }
                else
                {
                    r.Text($":x: {actionName} not done. {response.ToJsonIndented()}");
                }
            }
        }

        private async void Bot_OnMessage(object sender, MessageEventArgs e)
        {
            var r = new Responser(botClient, e.Message);

            var messageText = e.Message.Text;
            if (messageText == null)
            {
                r.Text("I can understand only text");
                return;
            }

            if (messageText.StartsWith("Фильм "))
            {
                Console.WriteLine("Detected Kinopois");

                // Фильм "Во все тяжкие" ("Breaking Bad", 2008-2013) #kinopoisk
                // http://www.kinopoisk.ru/film/404900/
                Regex regex = new Regex(@"Фильм ""(?<rusName>.+)"" \(""(?<engName>.+)"", (?<years>\d+-?\d*)\)");

                var match = regex.Match(messageText);
                if (!match.Success)
                {
                    r.Text($"Cant parse what you requested{Environment.NewLine}{messageText}");
                    return;
                }

                var searchRequest = match.Groups["rusName"] + " " + match.Groups["engName"];

                SearchTorrents(searchRequest, r);
            }
            else if (messageText.StartsWith("torrent"))
            {
                Console.WriteLine("Searching torrents");

                Regex regex = new Regex(@"torrent (?<searchRequest>.+)");
                var match = regex.Match(messageText);

                if (!match.Success)
                {
                    r.ReplyBack("Cant parse what you requested.");
                    return;
                }

                var searchRequest = match.Groups["searchRequest"];
                if (searchRequest.Value.IsNullOrEmpty())
                {
                    r.ReplyBack($"You requested search for nothing!");
                    return;
                }

                SearchTorrents(searchRequest.Value, r);
            }
            else
            {
                r.ReplyBack($"No idea what to do");
                Console.WriteLine(
                    $"Received a text message in chat {e.Message.Chat.Id}.{Environment.NewLine}{messageText}");
            }
        }

        private async void SearchTorrents(string searchRequest, Responser r)
        {
            r.TextWithAction($"Searching for {searchRequest}", ChatAction.Typing);
            var jackett = new JackettIntegration(
                new Jackett.Contracts.Settings
                    {
                        Url = this.configuration.JacketUrl,
                        ApiKey = this.configuration.JacketApiKey
                    });

            try
            {
                results = jackett.SearchTorrents(searchRequest);
            }
            catch (Exception exception)
            {
                r.Text(exception.GetShortDescription($"Search of '{searchRequest}'failed"));
                Console.WriteLine(exception);
            }

            var resultsToShow = results.Results.OrderBy(r => r.Size).Take(5).ToArray();
            Console.WriteLine("Search is done");

            var inlineKeyboardMarkup = new InlineKeyboardMarkup(
                new[]
                    {
                        // CallbackData should be max 64bytes
                        // Data to be sent in a callback query to the bot when button is pressed, 1-64 bytes
                        resultsToShow.Select(
                            (r, i) =>
                                {
                                    var uri = r.Guid;
                                    var hashedUrl = EncryptHelper.GetMD5(uri.AbsoluteUri);
                                    hashToUrl.TryAdd(hashedUrl, uri);
                                    return InlineKeyboardButton.WithCallbackData($"{i + 1}", $"Call {hashedUrl}");
                                })
                    });

            r.ReplyBack(
                text: $"Found {results.Results.Count} results from "
                + $"{results.Indexers.OrderByDescending(i => i.Results).Count(i => i.Results > 0)} indexers: "
                + results.Indexers.OrderByDescending(i => i.Results).Select(i => $"{i.Name} ({i.Results})")
                    .JoinToString(", ") + NewLine
                + resultsToShow
                    .Select(
                        (r, i) => $"{i + 1}. {r.Size} {r.Title}" + $"{NewLine}{r.Tracker} {nameof(r.Grabs)}:{r.Grabs} "
                            + $"{nameof(r.Seeders)}:{r.Seeders} {r.PublishDate}").JoinToString(NewLine + NewLine),
                replyMarkup: inlineKeyboardMarkup);
        }

        private class Responser
        {
            private readonly ITelegramBotClient botClient;

            private readonly Message eMessage;

            public Responser(ITelegramBotClient botClient, Message eMessage)
            {
                this.botClient = botClient;
                this.eMessage = eMessage;
            }

            public async void Text(string text)
            {
                await botClient.SendTextMessageAsync(eMessage.Chat, text);
            }

            public async void TextWithAction(
                string text,
                ChatAction chatAction,
                CancellationToken cancellationToken = default)
            {
                Text(text);
                await botClient.SendChatActionAsync(eMessage.Chat, chatAction, cancellationToken);
            }

            public async void ReplyBack(string text, IReplyMarkup replyMarkup = default)
            {
                try
                {
                    await botClient.SendTextMessageAsync(
                        chatId: eMessage.Chat,
                        text: text,
                        ParseMode.Markdown,
                        replyToMessageId: eMessage.MessageId,
                        replyMarkup: replyMarkup);
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                }
            }
        }
    }
}