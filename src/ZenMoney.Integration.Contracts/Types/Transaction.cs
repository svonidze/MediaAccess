namespace ZenMoney.Integration.Contracts.Types;

using System.Collections.Generic;

using Newtonsoft.Json;

public class Transaction
{
    [JsonProperty("id")]
    public int? Id { get; set; }

    [JsonProperty("user")]
    public int? User { get; set; }

    [JsonProperty("account_income")]
    public int? AccountIncome { get; set; }

    [JsonProperty("account_outcome")]
    public int? AccountOutcome { get; set; }

    [JsonProperty("date")]
    public string Date { get; set; }

    [JsonProperty("income")]
    public string Income { get; set; }

    [JsonProperty("outcome")]
    public string Outcome { get; set; }

    [JsonProperty("category")]
    public int? Category { get; set; }

    [JsonProperty("payee")]
    public string Payee { get; set; }

    [JsonProperty("merchant")]
    public object Merchant { get; set; }

    [JsonProperty("comment")]
    public string Comment { get; set; }

    [JsonProperty("instrument_income")]
    public int? InstrumentIncome { get; set; }

    [JsonProperty("instrument_outcome")]
    public int? InstrumentOutcome { get; set; }

    [JsonProperty("price")]
    public object Price { get; set; }

    [JsonProperty("created")]
    public string Created { get; set; }

    [JsonProperty("changed")]
    public string Changed { get; set; }

    [JsonProperty("deleted")]
    public bool? Deleted { get; set; }

    [JsonProperty("type_income")]
    public string TypeIncome { get; set; }

    [JsonProperty("type_outcome")]
    public string TypeOutcome { get; set; }

    [JsonProperty("direction")]
    public int? Direction { get; set; }

    [JsonProperty("inbalance_income")]
    public bool? InbalanceIncome { get; set; }

    [JsonProperty("inbalance_outcome")]
    public bool? InbalanceOutcome { get; set; }

    [JsonProperty("hold")]
    public bool? Hold { get; set; }

    [JsonProperty("tag_group")]
    public int? TagGroup { get; set; }

    [JsonProperty("tag_groups")]
    public List<int?> TagGroups { get; set; }

    [JsonProperty("payee_inflected")]
    public List<object> PayeeInflected { get; set; }
}