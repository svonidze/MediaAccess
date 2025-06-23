namespace Jackett.Contracts
{
    using System.Threading.Tasks;

    public interface IJackettIntegration
    {
        Task<ManualSearchResult?> SearchTorrents(string searchRequest, params string?[] trackerNames);
    }
}