namespace ModulDengi.Integration.Contracts.Types
{
    using System;

    using Newtonsoft.Json;

    public class Borrower    {
        [JsonProperty("id")]
        public string Id { get; set; } 

        [JsonProperty("shortName")]
        public string ShortName { get; set; } 

        [JsonProperty("fullName")]
        public string FullName { get; set; } 

        [JsonProperty("registerDateAt")]
        public DateTime RegisterDateAt { get; set; } 

        [JsonProperty("formalAddress")]
        public string FormalAddress { get; set; } 

        [JsonProperty("inn")]
        public string Inn { get; set; } 

        [JsonProperty("ogrn")]
        public string Ogrn { get; set; } 

        [JsonProperty("reachStatusId")]
        public string ReachStatusId { get; set; } 

        [JsonProperty("latestContractFinalDateAt")]
        public DateTime LatestContractFinalDateAt { get; set; } 

        [JsonProperty("totalContractsAmount")]
        public string TotalContractsAmount { get; set; } 

        [JsonProperty("totalContractsCount")]
        public int TotalContractsCount { get; set; } 

        [JsonProperty("contractsListUrl")]
        public string ContractsListUrl { get; set; } 

        [JsonProperty("contracts223ListUrl")]
        public string Contracts223ListUrl { get; set; } 

        [JsonProperty("typeId")]
        public string TypeId { get; set; } 

        [JsonProperty("actAddress")]
        public string ActAddress { get; set; } 

        [JsonProperty("okved")]
        public string Okved { get; set; } 

        [JsonProperty("user")]
        public User User { get; set; } 

        [JsonProperty("publicContacts")]
        public string PublicContacts { get; set; } 

        [JsonProperty("directorFounder")]
        public DirectorFounder DirectorFounder { get; set; } 
    }
}