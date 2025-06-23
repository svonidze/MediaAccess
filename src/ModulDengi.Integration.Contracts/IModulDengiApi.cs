namespace ModulDengi.Integration.Contracts;

using System;
using System.Threading.Tasks;

using ModulDengi.Integration.Contracts.Responses;

public interface IModulDengiApi
{
    Task<BalanceResponse?> GetBalance(string myCompanyId);

    Task<InvestmentPendingResponse[]?> GetProjectsRisingFunds();

    Task<InvestmentCreatedResponse?> CreateInvestment(string projectId, double money);

    Task<ModulDengiResponse?> SignInvestment(string investmentId);

    Task<ModulDengiResponse?> ConfirmInvestment(string investmentId, string confirmationCode);

    Task<InvestmentDoneResponse[]?> GetInvestments(string myCompanyId);

    Task<AccountStatementResponse[]?> GetAccountStatements(
        string companyId,
        DateTime? dateSince = null,
        DateTime? dateTo = null);
}