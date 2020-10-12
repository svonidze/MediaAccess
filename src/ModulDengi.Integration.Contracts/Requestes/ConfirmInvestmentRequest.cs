namespace ModulDengi.Integration.Contracts.Requestes
{
    using Newtonsoft.Json;

    public class ConfirmInvestmentRequest
    {
        [JsonProperty("id")]
        public string InvestmentId { get; set; }
        
        [JsonProperty("code")]
        public string Code { get; set; }
    }
}