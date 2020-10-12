namespace ModulDengi.Core
{
    using System.Linq;

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

        public bool IsApiAvailable(out string reason)
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

        public Project[] GetProjectsRisingFunds() =>
            this.api.ProjectsRisingFunds()
                .Select(
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

        public double GetMyFreeMoneyAmount()
        {
            //var myCompany = this.api.MyCompanies().Single();
            var balance = this.api.MyBalance(this.accessConfig.MyCompanyId);
            return balance.Investor.Free;
        }
        
        public Result<Investment> StartInvestmentFLow(Project project, in double amount)
        {
            var investmentCreatedResponse = this.api.CreateInvestment(project.Id, amount);
            if (!investmentCreatedResponse.IsSuccessful)
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
            
            var signInvestmentResponse = this.api.SignInvestment(investment.Id);
            if (!signInvestmentResponse.IsSuccessful)
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

        public Result ConfirmInvestment(string investmentId, string confirmationCode)
        {
            var response = this.api.ConfirmInvestment(investmentId, confirmationCode);
            if (!response.IsSuccessful)
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
    }
}