namespace ModulDengi.Integration.Contracts.Responses
{
    using Common.Http.Contracts;

    using Newtonsoft.Json;

    public class ModulDengiResponse : Response
    {
        [JsonProperty("isSuccessful")]
        public bool IsSuccessful { get; set; }

        [JsonProperty("error")]
        public Error Error { get; set; }
    }
}