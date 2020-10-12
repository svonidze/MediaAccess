namespace ModulDengi.Integration.Contracts.Types
{
    using Newtonsoft.Json;

    public class CalcCession    {
        [JsonProperty("investor")]
        public Investor Investor { get; set; } 

        [JsonProperty("borrower")]
        public Borrower2 Borrower { get; set; } 
    }
}