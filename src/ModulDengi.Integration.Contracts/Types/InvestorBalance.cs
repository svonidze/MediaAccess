namespace ModulDengi.Integration.Contracts.Types
{
    using Newtonsoft.Json;

    public class InvestorBalance    {
        [JsonProperty("free")]
        public double Free { get; set; } 

        [JsonProperty("freeAcquiring")]
        public int FreeAcquiring { get; set; } 

        [JsonProperty("reserved")]
        public int Reserved { get; set; } 

        [JsonProperty("expectedPrimaryIncome")]
        public double ExpectedPrimaryIncome { get; set; } 

        [JsonProperty("expectedPercentIncome")]
        public int ExpectedPercentIncome { get; set; } 

        [JsonProperty("overduePrimaryIncome")]
        public double OverduePrimaryIncome { get; set; } 

        [JsonProperty("overduePercentIncome")]
        public double OverduePercentIncome { get; set; } 

        [JsonProperty("overduePenaltyIncome")]
        public double OverduePenaltyIncome { get; set; } 

        [JsonProperty("profitExpectedAmount")]
        public double ProfitExpectedAmount { get; set; } 

        [JsonProperty("totalRefundedProfit")]
        public double TotalRefundedProfit { get; set; } 

        [JsonProperty("platformDebtAmount")]
        public int PlatformDebtAmount { get; set; } 

        [JsonProperty("overdueAmount")]
        public double OverdueAmount { get; set; } 

        [JsonProperty("overdueFactIncomeAmount")]
        public double OverdueFactIncomeAmount { get; set; } 

        [JsonProperty("restriction")]
        public int Restriction { get; set; } 

        [JsonProperty("defenseInterest")]
        public int DefenseInterest { get; set; } 
    }
}