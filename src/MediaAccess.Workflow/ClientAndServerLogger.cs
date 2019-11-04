namespace MediaServer.Workflow
{
    using System;
    using System.Threading;

    using Telegram.Bot;
    using Telegram.Bot.Types;
    using Telegram.Bot.Types.Enums;
    using Telegram.Bot.Types.ReplyMarkups;

    public class ClientAndServerLogger
    {
        private readonly ITelegramBotClient botClient;

        private readonly Message message;

        private string lastText;

        public ClientAndServerLogger(ITelegramBotClient botClient, Message message)
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

        public async void LogLastMessage()
        {
            this.Log(this.lastText);
        }

        public async void Log(string text)
        {
            Console.WriteLine($"{this.message.Chat.Id}\t{text}");
        }
    }
}