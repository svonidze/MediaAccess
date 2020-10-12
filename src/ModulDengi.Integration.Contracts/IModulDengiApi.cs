namespace ModulDengi.Integration.Contracts
{
    using ModulDengi.Integration.Contracts.Responses;

    public interface IModulDengiApi
    {
        //Project[] GetNewProjectsRisingFunds();
        BalanceResponse MyBalance(string myCompanyId);

        InvestmentPendingResponse[] ProjectsRisingFunds();

        InvestmentCreatedResponse CreateInvestment(string projectId, double money);

        ModulDengiResponse SignInvestment(string investmentId);

        ModulDengiResponse ConfirmInvestment(string investmentId, string confirmationCode);
    }
}