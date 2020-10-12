namespace ModulDengi.Integration.Contracts.Types
{
    using System;

    using Newtonsoft.Json;

    public class User    {
        [JsonProperty("birthPlace")]
        public string BirthPlace { get; set; } 

        [JsonProperty("birthDateAt")]
        public DateTime BirthDateAt { get; set; } 

        [JsonProperty("registrationAddress")]
        public string RegistrationAddress { get; set; } 
    }
}