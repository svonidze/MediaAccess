namespace ModulDengi.Integration.Contracts.Types
{
    using System.Collections.Generic;

    using Newtonsoft.Json;

    public class OtherProjects    {
        [JsonProperty("ownedByBorrower")]
        public List<OwnedByBorrower> OwnedByBorrower { get; set; } 
    }
}