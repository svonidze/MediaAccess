namespace Jackett.Contracts
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    [DataContract]
    public class Capability
    {
        [DataMember]
        public string ID { get; set; }
        [DataMember]
        public string Name { get; set; }
    }

    [DataContract]
    public class Indexer
    {
        [DataMember]
        public string id { get; private set; }
        [DataMember]
        public string name { get; private set; }
        [DataMember]
        public string description { get; private set; }
        [DataMember]
        public string type { get; private set; }
        [DataMember]
        public bool configured { get; private set; }
        [DataMember]
        public string site_link { get; private set; }
        [DataMember]
        public IEnumerable<string> alternativesitelinks { get; private set; }
        [DataMember]
        public string language { get; private set; }
        [DataMember]
        public string last_error { get; private set; }
        [DataMember]
        public bool potatoenabled { get; private set; }

        [DataMember]
        public IEnumerable<Capability> caps { get; private set; }

    }
}