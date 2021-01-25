namespace ModulDengi.Integration.Contracts.Requests
{
    using Newtonsoft.Json;

    public class CreateInvestmentRequest
    {
        [JsonProperty("projectId")]
        public string ProjectId { get; set; } 

        [JsonProperty("companyId")]
        public string CompanyId { get; set; } 

        [JsonProperty("amount")]
        public int Amount { get; set; } 

        [JsonProperty("isDefenseEnabled")]
        public bool IsDefenseEnabled { get; set; } 

        [JsonProperty("defenseId")]
        public string DefenseId { get; set; } 

        [JsonProperty("otherPlatforms")]
        public int OtherPlatforms { get; set; } 
    }
}