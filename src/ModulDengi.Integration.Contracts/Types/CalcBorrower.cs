namespace ModulDengi.Integration.Contracts.Types
{
    using Newtonsoft.Json;

    public class CalcBorrower    {
        [JsonProperty("mmFeeAmount")]
        public double MmFeeAmount { get; set; } 

        [JsonProperty("outputAmount")]
        public double OutputAmount { get; set; } 

        [JsonProperty("percentOverpaymentAmountPlan")]
        public double PercentOverpaymentAmountPlan { get; set; } 

        [JsonProperty("percentOverpaymentNdflAmountPlan")]
        public int PercentOverpaymentNdflAmountPlan { get; set; } 

        [JsonProperty("refundAmountPlan")]
        public double RefundAmountPlan { get; set; } 

        [JsonProperty("refundNdflAmountPlan")]
        public int RefundNdflAmountPlan { get; set; } 

        [JsonProperty("totalOverpaymentAmountPlan")]
        public double TotalOverpaymentAmountPlan { get; set; } 

        [JsonProperty("totalOverpaymentNdflAmountPlan")]
        public int TotalOverpaymentNdflAmountPlan { get; set; } 

        [JsonProperty("percentOverpaymentAmountNow")]
        public int PercentOverpaymentAmountNow { get; set; } 

        [JsonProperty("percentOverpaymentNdflAmountNow")]
        public int PercentOverpaymentNdflAmountNow { get; set; } 

        [JsonProperty("refundAmountNow")]
        public int RefundAmountNow { get; set; } 

        [JsonProperty("refundNdflAmountNow")]
        public int RefundNdflAmountNow { get; set; } 

        [JsonProperty("totalOverpaymentAmountNow")]
        public int TotalOverpaymentAmountNow { get; set; } 

        [JsonProperty("totalOverpaymentNdflAmountNow")]
        public int TotalOverpaymentNdflAmountNow { get; set; } 

        [JsonProperty("percentOverpaymentAmountPerDay")]
        public int PercentOverpaymentAmountPerDay { get; set; } 

        [JsonProperty("percentOverpaymentNdflAmountPerDay")]
        public int PercentOverpaymentNdflAmountPerDay { get; set; } 

        [JsonProperty("refundedAmountFact")]
        public int RefundedAmountFact { get; set; } 

        [JsonProperty("refundedNdflAmountFact")]
        public int RefundedNdflAmountFact { get; set; } 

        [JsonProperty("overdueBaseAmount")]
        public int OverdueBaseAmount { get; set; } 

        [JsonProperty("overdueBaseNdflAmount")]
        public int OverdueBaseNdflAmount { get; set; } 

        [JsonProperty("overduePercentAmountNow")]
        public int OverduePercentAmountNow { get; set; } 

        [JsonProperty("overduePercentNdflAmountNow")]
        public int OverduePercentNdflAmountNow { get; set; } 

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