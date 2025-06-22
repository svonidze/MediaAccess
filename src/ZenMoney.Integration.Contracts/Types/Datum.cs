namespace ZenMoney.Integration.Contracts.Types;

using Newtonsoft.Json;

public class Datum
{
    [JsonProperty("id")]
    public int? Id { get; set; }

    [JsonProperty("user")]
    public int? User { get; set; }

    [JsonProperty("connection")]
    public int? Connection { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("value")]
    public string Value { get; set; }

    [JsonProperty("account")]
    public int? Account { get; set; }

    [JsonProperty("active")]
    public bool? Active { get; set; }

    [JsonProperty("error_text")]
    public object ErrorText { get; set; }

    [JsonProperty("changed")]
    public string Changed { get; set; }

    [JsonProperty("static_id")]
    public object StaticId { get; set; }

    [JsonProperty("uid")]
    public string Uid { get; set; }
}