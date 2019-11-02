namespace Jackett.Contracts
{
    using System;

    public interface IJackettAccessConfiguration
    {
        string Url { get; set; }

        string ApiKey { get; set; }

        TimeSpan? Timeout { get; set; }
    }
}