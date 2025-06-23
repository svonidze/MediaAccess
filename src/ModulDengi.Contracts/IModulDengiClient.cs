namespace ModulDengi.Contracts
{
    using System.Threading.Tasks;

    using Common.Results;

    public interface IModulDengiClient
    {
        Task<Project[]?> GetProjectsRisingFunds();

        Task<double> GetMyFreeMoneyAmount();

        Task<Result<Investment>> StartInvestmentFlow(Project project, double amount);

        Task<Result> ConfirmInvestment(string investmentId, string confirmationCode);

        bool IsApiAvailable(out string? reason);
    }
}