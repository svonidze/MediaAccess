namespace ModulDengi.Integration.Contracts.Types
{
    using Newtonsoft.Json;

    public class DefenseHolderBalance    {
        [JsonProperty("free")]
        public int Free { get; set; } 
    }
}