namespace ModulDengi.Integration.Contracts.Requestes
{
    using Newtonsoft.Json;

    public class SignInvestmentRequest
    {
        [JsonProperty("investmentId")]
        public string InvestmentId { get; set; }         
    }
}