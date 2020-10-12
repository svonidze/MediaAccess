namespace ModulDengi.Integration.Contracts.Responses
{
    using System;

    using ModulDengi.Integration.Contracts.Types;

    using Newtonsoft.Json;

    public class InvestmentPendingResponse : ModulDengiResponse
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("number")]
        public string Number { get; set; }

        [JsonProperty("fundsStartedAt")]
        public DateTime FundsStartedAt { get; set; }

        [JsonProperty("statusId")]
        public string StatusId { get; set; }

        [JsonProperty("loanRaisingDue")]
        public DateTime LoanRaisingDue { get; set; }

        [JsonProperty("loanAmount")]
        public string LoanAmount { get; set; }

        [JsonProperty("loanTerm")]
        public int LoanTerm { get; set; }

        [JsonProperty("loanRate")]
        public string LoanRate { get; set; }

        [JsonProperty("raisedAmount")]
        public string RaisedAmount { get; set; }

        [JsonProperty("borrower")]
        public Borrower Borrower { get; set; }

        [JsonProperty("borrowerHaveOtherInvestments")]
        public bool BorrowerHaveOtherInvestments { get; set; }

        [JsonProperty("refundedLoansCountNow")]
        public int RefundedLoansCountNow { get; set; }

        [JsonProperty("myInvestmentAmount")]
        public int MyInvestmentAmount { get; set; }
    }
}