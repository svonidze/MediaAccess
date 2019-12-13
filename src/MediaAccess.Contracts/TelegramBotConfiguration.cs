namespace MediaServer.Contracts
{
    using System;

    public class TelegramBotConfiguration
    {
        public string Token { get; set; }

        public long[] AllowedChats { get; set; }

        public TimeSpan? Timeout { get; set; }
    }
}