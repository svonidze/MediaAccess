namespace Jackett.Contracts
{
    public interface IJackettIntegration
    {
        ManualSearchResult SearchTorrents(string searchRequest);
    }
}