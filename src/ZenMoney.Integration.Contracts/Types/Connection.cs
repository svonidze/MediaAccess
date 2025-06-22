namespace ZenMoney.Integration.Contracts.Types;

using System.Collections.Generic;

using Newtonsoft.Json;

public class Connection
{
    [JsonProperty("id")]
    public int? Id { get; set; }

    [JsonProperty("user")]
    public int? User { get; set; }

    [JsonProperty("company")]
    public int? Company { get; set; }

    [JsonProperty("account")]
    public object Account { get; set; }

    [JsonProperty("state")]
    public string State { get; set; }

    [JsonProperty("sync_time")]
    public string SyncTime { get; set; }

    [JsonProperty("sync_interval")]
    public object SyncInterval { get; set; }

    [JsonProperty("sheduled")]
    public bool? Sheduled { get; set; }

    [JsonProperty("title")]
    public string Title { get; set; }

    [JsonProperty("error_code")]
    public object ErrorCode { get; set; }

    [JsonProperty("changed")]
    public string Changed { get; set; }

    [JsonProperty("static_id")]
    public object StaticId { get; set; }

    [JsonProperty("uid")]
    public string Uid { get; set; }

    [JsonProperty("data_changed")]
    public string DataChanged { get; set; }

    [JsonProperty("accounts")]
    public List<string> Accounts { get; set; }

    [JsonProperty("data")]
    public List<Datum> Data { get; set; }
}