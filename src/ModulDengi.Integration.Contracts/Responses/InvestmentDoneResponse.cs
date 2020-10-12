namespace ModulDengi.Integration.Contracts.Responses
{
    using System;
    using System.Collections.Generic;

    using ModulDengi.Integration.Contracts.Types;

    using Newtonsoft.Json;

    public class InvestmentDoneResponse : ModulDengiResponse   {
        [JsonProperty("id")]
        public string Id { get; set; } 

        [JsonProperty("number")]
        public string Number { get; set; } 

        [JsonProperty("statusId")]
        public string StatusId { get; set; } 

        [JsonProperty("loanFundedAt")]
        public DateTime LoanFundedAt { get; set; } 

        [JsonProperty("loanRefundedAt")]
        public DateTime LoanRefundedAt { get; set; } 

        [JsonProperty("loanDue")]
        public DateTime LoanDue { get; set; } 

        [JsonProperty("loanTerm")]
        public string LoanTerm { get; set; } 

        [JsonProperty("loanRate")]
        public string LoanRate { get; set; } 

        [JsonProperty("loanPenaltyRate")]
        public string LoanPenaltyRate { get; set; } 

        [JsonProperty("isCessionPrefixed")]
        public bool IsCessionPrefixed { get; set; } 

        [JsonProperty("cessionPrefixDateAt")]
        public DateTime CessionPrefixDateAt { get; set; } 

        [JsonProperty("myInvestmentAmount")]
        public int MyInvestmentAmount { get; set; } 

        [JsonProperty("eulaVersion")]
        public string EulaVersion { get; set; } 

        [JsonProperty("investments")]
        public List<Investment> Investments { get; set; } 

        [JsonProperty("borrower")]
        public Borrower Borrower { get; set; } 

        [JsonProperty("calcCommon")]
        public CalcCommon CalcCommon { get; set; } 

        [JsonProperty("state")]
        public State State { get; set; } 

        [JsonProperty("calcInvestor")]
        public CalcInvestor CalcInvestor { get; set; } 

        [JsonProperty("calcCession")]
        public CalcCession CalcCession { get; set; } 
    }
}