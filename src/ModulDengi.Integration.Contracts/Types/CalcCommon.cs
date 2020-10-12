namespace ModulDengi.Integration.Contracts.Types
{
    using Newtonsoft.Json;

    public class CalcCommon    {
        [JsonProperty("daysSpentNow")]
        public int DaysSpentNow { get; set; } 

        [JsonProperty("daysLeftNow")]
        public int DaysLeftNow { get; set; } 

        [JsonProperty("daysOverdueNow")]
        public int DaysOverdueNow { get; set; } 

        [JsonProperty("daysSpentBeforeRefunded")]
        public int DaysSpentBeforeRefunded { get; set; } 

        [JsonProperty("platformDebtAmount")]
        public int PlatformDebtAmount { get; set; } 

        [JsonProperty("currentCessionPlatformInterestRate")]
        public int CurrentCessionPlatformInterestRate { get; set; } 
    }
}