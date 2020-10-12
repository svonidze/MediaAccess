namespace ModulDengi.Integration.Contracts.Types
{
    using Newtonsoft.Json;

    public class Borrower2    {
        [JsonProperty("refundAmountNow")]
        public int RefundAmountNow { get; set; } 

        [JsonProperty("refundAmountPrefixedCession")]
        public int RefundAmountPrefixedCession { get; set; } 
    }
}