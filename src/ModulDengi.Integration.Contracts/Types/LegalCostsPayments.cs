namespace ModulDengi.Integration.Contracts.Types
{
    using Newtonsoft.Json;

    public class LegalCostsPayments    {
        [JsonProperty("legalCosts")]
        public string LegalCosts { get; set; } 

        [JsonProperty("partialOff")]
        public string PartialOff { get; set; } 
    }
}