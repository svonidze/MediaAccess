namespace ModulDengi.Integration.Contracts.Types
{
    using Newtonsoft.Json;

    public class Document    {
        [JsonProperty("id")]
        public string Id { get; set; } 

        [JsonProperty("type")]
        public string Type { get; set; } 

        [JsonProperty("investorCompanyTitle")]
        public string InvestorCompanyTitle { get; set; } 

        [JsonProperty("investmentAmount")]
        public string InvestmentAmount { get; set; } 

        [JsonProperty("investmentId")]
        public string InvestmentId { get; set; } 

        [JsonProperty("isDefenseEnabled")]
        public bool? IsDefenseEnabled { get; set; } 
    }
}