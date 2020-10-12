namespace ModulDengi.Contracts
{
    public class EmailSettings
    {
        public EmailType EmailType { get; set; }

        public string Host { get; set; }

        public int Port { get; set; }

        public bool UseSsl { get; set; }

        public Credential Credential { get; set; }
    }
}