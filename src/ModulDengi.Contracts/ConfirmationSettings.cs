namespace ModulDengi.Contracts
{
    using System;

    public class ConfirmationSettings
    {
        public EmailSettings Email { get; set; }
    }

    public class TimeSettings
    {
        public TimeSpan CheckInterval { get; set; }

        public TimeSpan? TimeOut { get; set; }
    }
}