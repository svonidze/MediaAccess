namespace ModulDengi.Integration.Contracts.Types
{
    using Newtonsoft.Json;

    public class CalcInvestor    {
        [JsonProperty("baseAmount")]
        public int BaseAmount { get; set; } 

        [JsonProperty("defenseAmount")]
        public int DefenseAmount { get; set; } 

        [JsonProperty("profitAmountPlan")]
        public double ProfitAmountPlan { get; set; } 

        [JsonProperty("profitNdflAmountPlan")]
        public int ProfitNdflAmountPlan { get; set; } 

        [JsonProperty("profitAmountNow")]
        public int ProfitAmountNow { get; set; } 

        [JsonProperty("profitNdflAmountNow")]
        public int ProfitNdflAmountNow { get; set; } 

        [JsonProperty("investRefundedAmountFact")]
        public int InvestRefundedAmountFact { get; set; } 

        [JsonProperty("investRefundedNdflAmountFact")]
        public int InvestRefundedNdflAmountFact { get; set; } 

        [JsonProperty("investOverdueBaseAmount")]
        public int InvestOverdueBaseAmount { get; set; } 

        [JsonProperty("investOverdueBaseNdflAmount")]
        public int InvestOverdueBaseNdflAmount { get; set; } 

        [JsonProperty("overdueProfitAmountPerDay")]
        public int OverdueProfitAmountPerDay { get; set; } 

        [JsonProperty("overdueProfitNdflAmountPerDay")]
        public int OverdueProfitNdflAmountPerDay { get; set; } 

        [JsonProperty("overdueProfitAmountNow")]
        public int OverdueProfitAmountNow { get; set; } 

        [JsonProperty("overdueProfitNdflAmountNow")]
        public int OverdueProfitNdflAmountNow { get; set; } 

        [JsonProperty("remainingProfitAmount")]
        public int RemainingProfitAmount { get; set; } 

        [JsonProperty("remainingProfitNdflAmount")]
        public int RemainingProfitNdflAmount { get; set; } 

        [JsonProperty("debtAmountNow")]
        public int DebtAmountNow { get; set; } 

        [JsonProperty("refundedPenaltyAmountFact")]
        public int RefundedPenaltyAmountFact { get; set; } 

        [JsonProperty("refundedPenaltyNdflAmountFact")]
        public int RefundedPenaltyNdflAmountFact { get; set; } 

        [JsonProperty("currentBaseAmount")]
        public int CurrentBaseAmount { get; set; } 

        [JsonProperty("currentInterestAmount")]
        public int CurrentInterestAmount { get; set; } 

        [JsonProperty("currentPenaltyAmount")]
        public int CurrentPenaltyAmount { get; set; } 
    }
}