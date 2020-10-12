namespace ModulDengi.Integration.Contracts.Types
{
    using Newtonsoft.Json;

    public class State    {
        [JsonProperty("isBorrower")]
        public bool IsBorrower { get; set; } 

        [JsonProperty("isInvestor")]
        public bool IsInvestor { get; set; } 

        [JsonProperty("hasBorrowerOverdue")]
        public bool HasBorrowerOverdue { get; set; } 

        [JsonProperty("isInvestmentsCessionPrefixed")]
        public bool IsInvestmentsCessionPrefixed { get; set; } 

        [JsonProperty("isInvestmentsCessionState")]
        public bool IsInvestmentsCessionState { get; set; } 

        [JsonProperty("isCessionReceived")]
        public bool IsCessionReceived { get; set; } 

        [JsonProperty("isCessionPartlyReceived")]
        public bool IsCessionPartlyReceived { get; set; } 

        [JsonProperty("isCessionAccepted")]
        public bool IsCessionAccepted { get; set; } 
    }
}