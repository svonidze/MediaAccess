namespace ModulDengi.Core
{
    using System;
    using System.Text.RegularExpressions;

    using MailKit;
    using MailKit.Net.Imap;

    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    using ModulDengi.Contracts;

    public class EmailConfirmationManager : IConfirmationManager
    {
        private static class RegexGroups
        {
            public const string ConfirmationCode = "code";

            public const string ProjectNumber = "projectNumber";
        }

        //<p>Входящее - ModulDengi (Контакт не определен) <br/>Текст сообщения: Код 7220<br />Инвестиция 1 500.00 р.<br />Оферта N308311
        private static readonly Regex EmailBodyRegex = new Regex(
            @"Входящее - ModulDengi" 
            + ".+"
            + "Текст сообщения: Код (?<code>\\d{4})"
            + ".+"
            + "Оферта N(?<projectNumber>\\d+)");

        private readonly EmailSettings emailSettings;

        private readonly ILogger<EmailConfirmationManager> logger;

        public EmailConfirmationManager(ILogger<EmailConfirmationManager> logger, IOptions<EmailSettings> emailSettings)
        {
            this.logger = logger;
            this.emailSettings = emailSettings.Value;
        }

        public bool CheckConfirmationReceived(DateTime checkSinceUtc, string projectNumber, out string confirmationCode)
        {
            if (this.emailSettings.EmailType != EmailType.Imap)
                throw new NotSupportedException($"{this.emailSettings.EmailType}");

            confirmationCode = null;

            using IMailStore client = new ImapClient();
            try
            {
                client.Connect(this.emailSettings.Host, this.emailSettings.Port, this.emailSettings.UseSsl);
                client.Authenticate(this.emailSettings.Credential.Login, this.emailSettings.Credential.Password);

                // The Inbox folder is always available on all IMAP servers...
                var inbox = client.Inbox;
                inbox.Open(FolderAccess.ReadOnly);

                this.logger.LogDebug($"Recent/Total messages: {inbox.Recent}/{inbox.Count}");

                for (var mailNumber = inbox.Count; mailNumber > 0; mailNumber--)
                {
                    var message = inbox.GetMessage(mailNumber - 1);
                    this.logger.LogDebug($"Reading #{mailNumber} email from {message.Date.UtcDateTime}");

                    if (message.Date.UtcDateTime < checkSinceUtc)
                    {
                        this.logger.LogWarning(
                            $"Email reading is stopped since the #{mailNumber} email got at "
                            + $"{message.Date.UtcDateTime} which is older than the min date {checkSinceUtc}");
                        return false;
                    }

                    var match = EmailBodyRegex.Match(message.HtmlBody);
                    if (!match.Success)
                    {
                        this.logger.LogDebug(
                            $"Skipping #{mailNumber} email since the email body does not correlate with the expected patter."
                            + Environment.NewLine + $"Subject: {message.Subject}" + Environment.NewLine
                            + $"Body: {message.HtmlBody}");

                        continue;
                    }

                    var actualProjectNumber = match.Groups[RegexGroups.ProjectNumber].Value;
                    if (actualProjectNumber != projectNumber)
                    {
                        this.logger.LogDebug(
                            $"Skipping #{mailNumber} email since {nameof(projectNumber)}='{actualProjectNumber}' "
                            + $" in the email body does not equal to the expected one '{projectNumber}'");
                        continue;
                    }

                    confirmationCode = match.Groups[RegexGroups.ConfirmationCode].Value;
                    this.logger.LogDebug(
                        $"Extracted {nameof(confirmationCode)}='{confirmationCode}' from #{mailNumber} email");
                    return true;
                }
            }
            finally
            {
                client.Disconnect(quit: true);
                client.Dispose();
            }

            this.logger.LogWarning($"No {nameof(confirmationCode)} found, see log above for details");
            return false;
        }
    }
}