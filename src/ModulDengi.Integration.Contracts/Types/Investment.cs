namespace ModulDengi.Integration.Contracts.Types
{
    using Newtonsoft.Json;

    public class Investment    {
        [JsonProperty("id")]
        public string Id { get; set; } 

        [JsonProperty("companyId")]
        public string CompanyId { get; set; } 

        [JsonProperty("amount")]
        public string Amount { get; set; } 

        [JsonProperty("defenseId")]
        public object DefenseId { get; set; } 

        [JsonProperty("isDefenseEnabled")]
        public bool IsDefenseEnabled { get; set; } 
    }
}