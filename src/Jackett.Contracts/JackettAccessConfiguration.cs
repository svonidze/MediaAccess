namespace Jackett.Contracts
{
    public class JackettAccessConfiguration : IJackettAccessConfiguration
    {
        public string Url { get; set; }
        
        public string ApiKey { get; set; }
    }
}