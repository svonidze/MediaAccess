namespace ModulDengi.Integration.Contracts.Requests
{
    using Newtonsoft.Json;

    public class SignInvestmentRequest
    {
        [JsonProperty("investmentId")]
        public string InvestmentId { get; set; }         
    }
}