namespace Kontur.Elba.Integration.Contracts
{
    using Newtonsoft.Json;

    public class CbrCurrencyRate
    {
        [JsonProperty("Value")]
        public int Value { get; set; }

        [JsonProperty("Nominal")]
        public int Nominal { get; set; }
    }
}