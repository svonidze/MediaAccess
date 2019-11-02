namespace Jackett.Contracts
{
    public interface IJackettAccessConfiguration
    {
        string Url { get; set; }

        string ApiKey { get; set; }
    }
}