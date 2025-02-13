namespace ModulDengi.Integration
{
    using System;
    using System.Collections.Specialized;
    using System.Net;
    using System.Net.Http;

    using Common.Http;

    using Microsoft.Extensions.Options;

    using ModulDengi.Contracts;
    using ModulDengi.Integration.Contracts;
    using ModulDengi.Integration.Contracts.Requests;
    using ModulDengi.Integration.Contracts.Responses;

    public class ModulDengiApi : IModulDengiApi
    {
        private readonly ModulDengiAccessConfig accessConfig;

        private readonly HttpRequestBuilder httpRequestBuilder;
        
        private string token;

        public ModulDengiApi(IOptions<ModulDengiAccessConfig> accessConfig, HttpRequestBuilder httpRequestBuilder)
        {
            this.httpRequestBuilder = httpRequestBuilder;
            this.accessConfig = accessConfig.Value;
        }

        public CompanyResponse[] MyCompanies() =>
            this.AuthAndRun(
                "/api/companies",
                httpBuilder => httpBuilder.RequestAndValidate<CompanyResponse[]>(HttpMethod.Get));

        public BalanceResponse GetBalance(string myCompanyId) =>
            this.AuthAndRun(
                $"/api/companies/{myCompanyId}/balance",
                httpBuilder => httpBuilder.RequestAndValidate<BalanceResponse>(HttpMethod.Get));

        public InvestmentDoneResponse[] GetInvestments(string myCompanyId) =>
            this.AuthAndRun(
                "/api/projects/my/investments",
                httpBuilder => httpBuilder.AddUrlQueryValues(
                        new NameValueCollection
                            {
                                { "companyId", myCompanyId }
                            })
                    .RequestAndValidate<InvestmentDoneResponse[]>(HttpMethod.Get));

        public InvestmentPendingResponse[] GetProjectsRisingFunds() =>
            this.AuthAndRun(
                "/api/projects/rising-funds",
                httpBuilder => httpBuilder.RequestAndValidate<InvestmentPendingResponse[]>(HttpMethod.Get));

        // if companyId is not found then 404
        //https://cabinet.moduldengi.ru/api/projects/3ccaffb2-4def-4584-b7ba-ec0ab8e8fd7f?companyId=a33523e3-4f7f-4170-bed2-c91ee2790d96
        public CompanyResponse2 CompanyInfo(string companyId, string myCompanyId) =>
            this.AuthAndRun(
                $"/api/projects/{companyId}",
                httpBuilder => httpBuilder.AddUrlQueryValues(
                        new NameValueCollection
                            {
                                { "companyId", myCompanyId }
                            })
                    .RequestAndValidate<CompanyResponse2>(HttpMethod.Get));

        public InvestmentCreatedResponse CreateInvestment(string projectId, double money) =>
            this.AuthAndRun(
                "/api/projects/investment/create",
                httpBuilder => httpBuilder.SetJsonPayload(
                        new CreateInvestmentRequest
                            {
                                Amount = (int)money,
                                CompanyId = this.accessConfig.MyCompanyId,
                                ProjectId = projectId
                            })
                    .RequestAndValidate<InvestmentCreatedResponse>(HttpMethod.Post));

        public ModulDengiResponse SignInvestment(string investmentId) =>
            this.AuthAndRun(
                "/api/projects/investment/sign",
                httpBuilder => httpBuilder.SetJsonPayload(
                        new SignInvestmentRequest
                            {
                                InvestmentId = investmentId
                            })
                    .RequestAndValidate<ModulDengiResponse >(HttpMethod.Post));

        public ModulDengiResponse ConfirmInvestment(string investmentId, string confirmationCode) =>
            this.AuthAndRun(
                "/api/projects/investment/confirm",
                httpBuilder => httpBuilder
                    .SetJsonPayload(new ConfirmInvestmentRequest
                        {
                            InvestmentId = investmentId,
                            Code = confirmationCode
                        })
                    .RequestAndValidate<ModulDengiResponse>(HttpMethod.Post));
        
        //dateSince=2017-08-31T21%3A00%3A00.000Z
        //&dateTo=2021-01-02T15%3A21%3A54.110Z
        //&companyId=a33523e3-4f7f-4170-bed2-c91ee2790d96
        //&accountType=investor
        public AccountStatementResponse[] GetAccountStatements(string companyId, DateTime? dateSince, DateTime? dateTo)
        {
            var queryValues = new NameValueCollection
                {
                    { "companyId", companyId },
                    { "accountType", "investor" },
                };
            if(dateSince.HasValue)
                queryValues.Add("dateSince", dateSince.Value.ToString("u"));
            if(dateTo.HasValue)
                queryValues.Add("dateTo", dateTo.Value.ToString("u"));
            
            return this.AuthAndRun(
                "/api/companies/account-statement",
                httpBuilder => httpBuilder.AddUrlQueryValues(queryValues)
                    .RequestAndValidate<AccountStatementResponse[]>(HttpMethod.Get));
        }

        private bool TryLoginAndSetupToken()
        {
            var url = $"{this.accessConfig.SiteUrl}/api/users/login";

            var httpBuilder = this.httpRequestBuilder
                .SetUrl(url)
                .SetJsonPayload(new LoginRequest
                    {
                        Login = this.accessConfig.Credential.Login,
                        Password = this.accessConfig.Credential.Password
                    
                    });
            var response = httpBuilder.RequestAndValidate<LoginResponse>(HttpMethod.Post);

            if (response.IsSuccessful)
                this.token = response.Token;
            
            return response.IsSuccessful;
        }
        
        private T AuthAndRun<T>(string urlPath, Func<HttpRequestBuilder, T> func)
        {
            if (this.token == null && !this.TryLoginAndSetupToken())
                throw new Exception("Login failed");
            
            var httpBuilder = this.httpRequestBuilder
                .SetUrl(this.accessConfig.SiteUrl + urlPath)
                .SetAuthorization(this.token);
            
            try
            {
                return func(httpBuilder);
            }
            catch (HttpException httpException)
            {
                if (httpException.StatusCode == HttpStatusCode.Unauthorized)
                {
                    if (!this.TryLoginAndSetupToken())
                        throw new Exception("Login failed", httpException);
                        
                    return this.AuthAndRun(urlPath, func);
                }
                
                throw;
            }
        }
    }
}