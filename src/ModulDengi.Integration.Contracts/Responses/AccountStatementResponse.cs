namespace ModulDengi.Integration.Contracts.Responses
{
    using System;

    using Newtonsoft.Json;

    public class AccountStatementResponse : ModulDengiResponse
    {
        [JsonProperty("createdAt")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("amount")]
        public double Amount { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }
    }
}