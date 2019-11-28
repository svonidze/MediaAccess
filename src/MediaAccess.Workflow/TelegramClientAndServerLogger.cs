namespace MediaServer.Workflow
{
    using System;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;

    using Common.Exceptions;
    using Common.Http;

    using MediaServer.Contracts;

    using Telegram.Bot;
    using Telegram.Bot.Types;
    using Telegram.Bot.Types.Enums;
    using Telegram.Bot.Types.InputFiles;
    using Telegram.Bot.Types.ReplyMarkups;

    public class TelegramClientAndServerLogger : ITelegramClientAndServerLogger
    {
        private readonly ITelegramBotClient botClient;

        private readonly Message message;

        private string lastText;

        public TelegramClientAndServerLogger(ITelegramBotClient botClient, Message message)
        {
            this.botClient = botClient;
            this.message = message;
        }

        public async void Text(string text)
        {
            this.lastText = text;
            await this.botClient.SendTextMessageAsync(this.message.Chat, text);
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

        public void LogLastMessage()
        {
            this.Log(this.lastText);
        }

        public void Log(string text)
        {
            Console.WriteLine($"{this.message.Chat.Id}\t{text}");
        }
        
        public async Task TrySendDocumentBackAsync(Uri @from)
        {
            Console.WriteLine(@from);

            var webClient = new WebClient();
            
            string ExtractFromContentDisposition(string key) => 
                ContentDisposition.Parse(webClient.ResponseHeaders["content-disposition"])?[key];

            try
            {
                await using var stream = await webClient.OpenReadTaskAsync(@from);
                await this.botClient.SendDocumentAsync(
                        this.message.Chat,
                        new InputOnlineFile(stream, ExtractFromContentDisposition("filename")))
                    .ContinueWith(task => this.Log($"{nameof(this.TrySendDocumentBackAsync)} is {task.Status}. " 
                        + $"{nameof(task.Result.Document.FileName)}={task.Result.Document.FileName}"));
            }
            catch (Exception e)
            {
                var customMessage = $"{nameof(this.TrySendDocumentBackAsync)} is Failed";
                
                this.Text(e.GetShortDescription(customMessage));
                this.Log(e.GetFullDescription(customMessage));
            }
        }
    }
}