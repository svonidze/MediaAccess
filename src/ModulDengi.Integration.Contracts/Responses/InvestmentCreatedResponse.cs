namespace ModulDengi.Integration.Contracts.Responses
{
    using Newtonsoft.Json;

    public class InvestmentCreatedResponse : ModulDengiResponse
    {
        [JsonProperty("investmentId")]
        public string InvestmentId { get; set; }
    }
}