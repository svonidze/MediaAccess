namespace MediaServer.Contracts
{
    using System;

    using JetBrains.Annotations;

    public class TelegramBotConfiguration
    {
        public string Token { get; set; }

        public long[] AllowedChats { get; set; }

        public TimeSpan? Timeout { get; set; }
        
        [CanBeNull]
        public Proxy Proxy { get; set; }
    }
}