namespace ModulDengi.Integration.Contracts.Types
{
    using Newtonsoft.Json;

    public class Probation    {
        [JsonProperty("typeId")]
        public object TypeId { get; set; } 

        [JsonProperty("expiredAt")]
        public string ExpiredAt { get; set; } 

        [JsonProperty("isApplied")]
        public bool IsApplied { get; set; } 

        [JsonProperty("isActive")]
        public bool IsActive { get; set; } 
    }
}