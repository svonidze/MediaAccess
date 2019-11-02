namespace Jackett.Contracts
{
    using System;

    public class JackettAccessConfiguration : IJackettAccessConfiguration
    {
        public string Url { get; set; }
        
        public string ApiKey { get; set; }

        public TimeSpan? Timeout { get; set; }
    }
}