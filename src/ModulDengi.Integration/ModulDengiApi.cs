namespace ModulDengi.Integration;

using System;
using System.Collections.Specialized;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using Common.Http;

using Microsoft.Extensions.Options;

using ModulDengi.Contracts;
using ModulDengi.Integration.Contracts;
using ModulDengi.Integration.Contracts.Requests;
using ModulDengi.Integration.Contracts.Responses;

public class ModulDengiApi(IOptions<ModulDengiAccessConfig> accessConfig, IHttpRequestBuilder httpRequestBuilder)
    : IModulDengiApi
{
    private readonly ModulDengiAccessConfig accessConfig = accessConfig.Value;

    private string? token;

    public async Task<CompanyResponse[]?> MyCompanies() =>
        await this._AuthAndRun(
            "/api/companies",
            (httpBuilder, url) => httpBuilder.RequestAndValidateAsync<CompanyResponse[]>(HttpMethod.Get, url));

    public async Task<BalanceResponse?> GetBalance(string myCompanyId) =>
        await this._AuthAndRun(
            $"/api/companies/{myCompanyId}/balance",
            (httpBuilder, url) => httpBuilder.RequestAndValidateAsync<BalanceResponse>(HttpMethod.Get, url));

    public async Task<InvestmentDoneResponse[]?> GetInvestments(string myCompanyId) =>
        await this._AuthAndRun(
            "/api/projects/my/investments",
            (httpBuilder, url) => httpBuilder.RequestAndValidateAsync<InvestmentDoneResponse[]>(
                HttpMethod.Get,
                UrlBuilder.Get(
                    url,
                    new NameValueCollection
                        {
                            { "companyId", myCompanyId }
                        })));

    public Task<InvestmentPendingResponse[]?> GetProjectsRisingFunds() =>
        this._AuthAndRun(
            "/api/projects/rising-funds",
            (httpBuilder, url) => httpBuilder.RequestAndValidateAsync<InvestmentPendingResponse[]>(HttpMethod.Get, url));

    // if companyId is not found then 404
    //https://cabinet.moduldengi.ru/api/projects/3ccaffb2-4def-4584-b7ba-ec0ab8e8fd7f?companyId=a33523e3-4f7f-4170-bed2-c91ee2790d96
    public Task<CompanyResponse2?> CompanyInfo(string companyId, string myCompanyId) =>
        this._AuthAndRun(
            $"/api/projects/{companyId}",
            (httpBuilder, url) => httpBuilder.RequestAndValidateAsync<CompanyResponse2>(HttpMethod.Get, UrlBuilder.Get(url, new NameValueCollection
                {
                    {
                        "companyId", myCompanyId
                    }
                })));

    public Task<InvestmentCreatedResponse?> CreateInvestment(string projectId, double money) =>
        this._AuthAndRun(
            "/api/projects/investment/create",
            (httpBuilder, url) => httpBuilder.RequestAndValidateAsync<InvestmentCreatedResponse>(
                HttpMethod.Post,
                url,
                request => request.SetJsonPayload(
                    new CreateInvestmentRequest
                        {
                            Amount = (int)money,
                            CompanyId = this.accessConfig.MyCompanyId,
                            ProjectId = projectId
                        })));

    public Task<ModulDengiResponse?> SignInvestment(string investmentId) =>
        this._AuthAndRun(
            "/api/projects/investment/sign",
            (httpBuilder, url) => httpBuilder.RequestAndValidateAsync<ModulDengiResponse>(
                HttpMethod.Post,
                url,
                request => request.SetJsonPayload(
                    new SignInvestmentRequest
                        {
                            InvestmentId = investmentId
                        })));

    public Task<ModulDengiResponse?> ConfirmInvestment(string investmentId, string confirmationCode) =>
        this._AuthAndRun(
            "/api/projects/investment/confirm",
            (httpBuilder, url) => httpBuilder.RequestAndValidateAsync<ModulDengiResponse>(
                HttpMethod.Post,
                url,
                request => request.SetJsonPayload(
                    new ConfirmInvestmentRequest
                        {
                            InvestmentId = investmentId,
                            Code = confirmationCode
                        })));

    //dateSince=2017-08-31T21%3A00%3A00.000Z
    //&dateTo=2021-01-02T15%3A21%3A54.110Z
    //&companyId=a33523e3-4f7f-4170-bed2-c91ee2790d96
    //&accountType=investor
    public Task<AccountStatementResponse[]?> GetAccountStatements(string companyId, DateTime? dateSince, DateTime? dateTo)
    {
        var queryValues = new NameValueCollection
            {
                { "companyId", companyId },
                { "accountType", "investor" },
            };
        if (dateSince.HasValue) queryValues.Add("dateSince", dateSince.Value.ToString("u"));
        if (dateTo.HasValue) queryValues.Add("dateTo", dateTo.Value.ToString("u"));

        return this._AuthAndRun(
            "/api/companies/account-statement",
            (httpBuilder, url) => httpBuilder.RequestAndValidateAsync<AccountStatementResponse[]>(
                HttpMethod.Get,
                UrlBuilder.Get(url, queryValues)));
    }

    private async Task<bool> _TryLoginAndSetupToken()
    {
        var url = $"{this.accessConfig.SiteUrl}/api/users/login";

        var response = await httpRequestBuilder.RequestAndValidateAsync<LoginResponse>(HttpMethod.Post, url,
            request => request.SetJsonPayload(
                new LoginRequest
                    {
                        Login = this.accessConfig.Credential.Login,
                        Password = this.accessConfig.Credential.Password
                    }));

        if (response == null) return false;
        if (response.IsSuccessful) this.token = response.Token;

        return response.IsSuccessful;
    }

    private async Task<T> _AuthAndRun<T>(string urlPath, Func<HttpRequestBuilder, string, Task<T>> func)
    {
        if (this.token == null && !await this._TryLoginAndSetupToken()) throw new Exception("Login failed");

        var httpBuilder = httpRequestBuilder.SetAuthorizationHeader(this.token);

        try
        {
            return await func(httpBuilder, this.accessConfig.SiteUrl + urlPath);
        }
        catch (HttpException httpException)
        {
            if (httpException.StatusCode != HttpStatusCode.Unauthorized) throw;
            
            if (!await this._TryLoginAndSetupToken()) throw new Exception("Login failed", httpException);
            return await this._AuthAndRun(urlPath, func);
        }
    }
}