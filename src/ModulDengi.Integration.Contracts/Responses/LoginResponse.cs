namespace ModulDengi.Integration.Contracts.Responses
{
    using Newtonsoft.Json;

    public class LoginResponse : ModulDengiResponse
    {
        [JsonProperty("token")] 
        public string Token { get; set; }
    }
}