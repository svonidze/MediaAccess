namespace ZenMoney.Integration.Contracts.Types;

using System.Collections.Generic;

using Newtonsoft.Json;

public class Connector
{
    [JsonProperty("company")]
    public int? Company { get; set; }

    [JsonProperty("title")]
    public string Title { get; set; }

    [JsonProperty("www")]
    public string Www { get; set; }

    [JsonProperty("connections")]
    public Dictionary<int, Connection> Connections { get; set; }

    [JsonProperty("parameters")]
    public List<object> Parameters { get; set; }
}