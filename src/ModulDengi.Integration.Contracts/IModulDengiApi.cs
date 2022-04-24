namespace ModulDengi.Integration.Contracts
{
    using System;

    using ModulDengi.Integration.Contracts.Responses;

    public interface IModulDengiApi
    {
        BalanceResponse GetBalance(string myCompanyId);

        InvestmentPendingResponse[] GetProjectsRisingFunds();

        InvestmentCreatedResponse CreateInvestment(string projectId, double money);

        ModulDengiResponse SignInvestment(string investmentId);

        ModulDengiResponse ConfirmInvestment(string investmentId, string confirmationCode);

        InvestmentDoneResponse[] GetInvestments(string myCompanyId);

        AccountStatementResponse[] GetAccountStatements(string companyId, DateTime? sinceDate = null, DateTime? dateTo = null);
    }
}