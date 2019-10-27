namespace Jackett.Contracts
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    [DataContract]
    public class ServerConfig
    {
        [DataMember]
        public IEnumerable<string> notices { get; set; }
        [DataMember]
        public int port { get; set; }
        [DataMember]
        public bool external { get; set; }
        [DataMember]
        public string api_key { get; set; }
        [DataMember]
        public string blackholedir { get; set; }
        [DataMember]
        public bool updatedisabled { get; set; }
        [DataMember]
        public bool prerelease { get; set; }
        [DataMember]
        public string password { get; set; }
        [DataMember]
        public bool logging { get; set; }
        [DataMember]
        public string basepathoverride { get; set; }
        [DataMember]
        public string omdbkey { get; set; }
        [DataMember]
        public string omdburl { get; set; }
        [DataMember]
        public string app_version { get; set; }
        [DataMember]
        public bool can_run_netcore { get; set; }

//        [DataMember]
//        public ProxyType proxy_type { get; set; }
        [DataMember]
        public string proxy_url { get; set; }
        [DataMember]
        public int? proxy_port { get; set; }
        [DataMember]
        public string proxy_username { get; set; }
        [DataMember]
        public string proxy_password { get; set; }

        public ServerConfig()
        {
            this.notices = new string[0];
        }
    }
}
