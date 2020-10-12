namespace ModulDengi.Integration.Contracts.Responses
{
    using ModulDengi.Integration.Contracts.Types;

    using Newtonsoft.Json;

    public class BalanceResponse : ModulDengiResponse    {
        [JsonProperty("investor")]
        public InvestorBalance Investor { get; set; } 

        [JsonProperty("borrower")]
        public BorrowerBalance Borrower { get; set; } 

        [JsonProperty("defenseHolder")]
        public DefenseHolderBalance DefenseHolder { get; set; } 
    }
}