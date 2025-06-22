namespace ZenMoney.Integration.Contracts.Types;

using Newtonsoft.Json;

public class Wmid
{
    [JsonProperty("id")]
    public int? Id { get; set; }

    [JsonProperty("company")]
    public int? Company { get; set; }

    [JsonProperty("level")]
    public string Level { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("editable")]
    public bool? Editable { get; set; }

    [JsonProperty("title")]
    public string Title { get; set; }

    [JsonProperty("description")]
    public object Description { get; set; }

    [JsonProperty("check")]
    public string Check { get; set; }

    [JsonProperty("default")]
    public object Default { get; set; }
}