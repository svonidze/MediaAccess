namespace ModulDengi.Integration.Contracts.Types
{
    using Newtonsoft.Json;

    public class BorrowerBalance    {
        [JsonProperty("free")]
        public int Free { get; set; } 

        [JsonProperty("projectSum")]
        public int ProjectSum { get; set; } 

        [JsonProperty("debtMain")]
        public int DebtMain { get; set; } 

        [JsonProperty("platformDebtAmount")]
        public int PlatformDebtAmount { get; set; } 

        [JsonProperty("overdueAmount")]
        public int OverdueAmount { get; set; } 
    }
}