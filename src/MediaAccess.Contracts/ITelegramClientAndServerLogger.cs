namespace MediaServer.Contracts
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Telegram.Bot.Types.Enums;
    using Telegram.Bot.Types.ReplyMarkups;

    public interface ITelegramClientAndServerLogger
    {
        void Text(string text);

        void TextWithAction(
            string text,
            ChatAction chatAction,
            CancellationToken cancellationToken = default);

        void ReplyBack(string text, IReplyMarkup replyMarkup = default);

        void LogLastMessage();

        void Log(string text);

        Task TrySendDocumentBackAsync(Uri @from);
    }
}