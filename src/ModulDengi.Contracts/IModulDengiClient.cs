namespace ModulDengi.Contracts
{
    using Common.Results;

    public interface IModulDengiClient
    {
        Project[] GetProjectsRisingFunds();

        double GetMyFreeMoneyAmount();
        
        Result<Investment> StartInvestmentFLow(Project project, in double amount);

        Result ConfirmInvestment(string investmentId, string confirmationCode);

        bool IsApiAvailable(out string reason);
    }
}