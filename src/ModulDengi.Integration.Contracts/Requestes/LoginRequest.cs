namespace ModulDengi.Integration.Contracts.Requestes
{
    using Newtonsoft.Json;

    public class LoginRequest
    {
        [JsonProperty("login")] public string Login { get; set; }

        [JsonProperty("password")] public string Password { get; set; }
    }
}