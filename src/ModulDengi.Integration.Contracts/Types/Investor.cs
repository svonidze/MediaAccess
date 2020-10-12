namespace ModulDengi.Integration.Contracts.Types
{
    using Newtonsoft.Json;

    public class Investor    {
        [JsonProperty("profitAmountNow")]
        public int ProfitAmountNow { get; set; } 

        [JsonProperty("profitNdflAmountNow")]
        public int ProfitNdflAmountNow { get; set; } 

        [JsonProperty("cessionPrefixedAmount")]
        public int CessionPrefixedAmount { get; set; } 

        [JsonProperty("debtAmountNow")]
        public int DebtAmountNow { get; set; } 

        [JsonProperty("platformCessionInterestAmount")]
        public int PlatformCessionInterestAmount { get; set; } 

        [JsonProperty("platformCessionInterestFactAmount")]
        public int PlatformCessionInterestFactAmount { get; set; } 

        [JsonProperty("expectingIncome")]
        public int ExpectingIncome { get; set; } 

        [JsonProperty("expectingNdfl")]
        public int ExpectingNdfl { get; set; } 
    }
}