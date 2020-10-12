namespace ModulDengi.Integration.Contracts.Types
{
    using System;

    using Newtonsoft.Json;

    public class OwnedByBorrower    {
        [JsonProperty("id")]
        public string Id { get; set; } 

        [JsonProperty("statusId")]
        public string StatusId { get; set; } 

        [JsonProperty("loanTerm")]
        public string LoanTerm { get; set; } 

        [JsonProperty("loanRate")]
        public int LoanRate { get; set; } 

        [JsonProperty("loanPenaltyRate")]
        public int LoanPenaltyRate { get; set; } 

        [JsonProperty("loanDue")]
        public DateTime LoanDue { get; set; } 

        [JsonProperty("loanRefundedAt")]
        public DateTime LoanRefundedAt { get; set; } 

        [JsonProperty("number")]
        public string Number { get; set; } 

        [JsonProperty("loanRaisedFunds")]
        public string LoanRaisedFunds { get; set; } 

        [JsonProperty("hasOverdue")]
        public bool HasOverdue { get; set; } 

        [JsonProperty("myInvestmentAmount")]
        public int MyInvestmentAmount { get; set; } 

        [JsonProperty("loanFundedAt")]
        public DateTime LoanFundedAt { get; set; } 
    }
}