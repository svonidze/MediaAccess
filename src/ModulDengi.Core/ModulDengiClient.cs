namespace ModulDengi.Core
{
    using System.Linq;
    using System.Threading.Tasks;

    using Common.Net;
    using Common.Results;

    using Microsoft.Extensions.Options;

    using ModulDengi.Contracts;
    using ModulDengi.Integration.Contracts;

    using Investment = ModulDengi.Contracts.Investment;

    public class ModulDengiClient : IModulDengiClient
    {
        private readonly IModulDengiApi api;

        private readonly ModulDengiAccessConfig accessConfig;
        
        public ModulDengiClient(IModulDengiApi api, IOptions<ModulDengiAccessConfig> accessConfig)
        {
            this.api = api;
            this.accessConfig = accessConfig.Value;
        }

        public bool IsApiAvailable(out string? reason)
        {
            var sitesToCheck = new []
                {
                    this.accessConfig.SiteUrl, Internet.DefaultSite
                };

            foreach (var site in sitesToCheck)
            {
                if (Internet.IsAvailable(this.accessConfig.SiteUrl))
                    continue;
                
                reason = $"{site} cannot be accessed";
                return false;
            }

            reason = null;
            return true;
        }

        public async Task<Project[]?> GetProjectsRisingFunds() =>
            (await this.api.GetProjectsRisingFunds())
                ?.Select(
                    p => new Project
                        {
                            Id = p.Id,
                            Number = p.Number,
                            BorrowerShortName = p.Borrower.ShortName,
                            MyInvestmentAmount = p.MyInvestmentAmount,
                            LoanAmount = p.LoanAmount,
                            LoanTerm = p.LoanTerm,
                            LoanRate = p.LoanRate,
                            RaisedAmount = p.RaisedAmount
                        })
                .ToArray();

        public async Task<double> GetMyFreeMoneyAmount()
        {
            //var myCompany = this.api.MyCompanies().Single();
            var balance = await this.api.GetBalance(this.accessConfig.MyCompanyId);
            return balance!.Investor.Free;
        }
        
        public async Task<Result<Investment>> StartInvestmentFlow(Project project,double amount)
        {
            var investmentCreatedResponse = await this.api.CreateInvestment(project.Id, amount);
            if (!investmentCreatedResponse!.IsSuccessful)
            {
                return new Result<Investment>
                    {
                        Success = false,
                        Message = $"{nameof(this.api.CreateInvestment)} failed due to {investmentCreatedResponse.Error?.Message}"
                    };
            }

            var investment = new Investment
                {
                    Id = investmentCreatedResponse.InvestmentId
                };
            
            var signInvestmentResponse = await this.api.SignInvestment(investment.Id);
            if (!signInvestmentResponse!.IsSuccessful)
            {
                return new Result<Investment>
                    {
                        Value = investment,
                        Success = false,
                        Message = $"{nameof(this.api.SignInvestment)} failed due to {investmentCreatedResponse.Error?.Message}"
                    };
            }

            return new Result<Investment>
                {
                    Value = investment,
                    Success = true
                };
        }

        public async Task<Result> ConfirmInvestment(string investmentId, string confirmationCode)
        {
            var response = await this.api.ConfirmInvestment(investmentId, confirmationCode);
            if (!response!.IsSuccessful)
            {
                return new Result
                    {
                        Success = false,
                        Message = $"{nameof(this.api.ConfirmInvestment)} failed due to {response.Error?.Message}"
                    };
            }
            
            return new Result
                {
                    Success = true
                };
        }

        public void aa()
        {
            this.api.GetInvestments(this.accessConfig.MyCompanyId);
        }
    }
}