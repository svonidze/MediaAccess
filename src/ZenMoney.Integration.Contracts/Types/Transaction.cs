namespace ZenMoney.Integration.Contracts
{
    using System.Collections.Generic;

    using Newtonsoft.Json;

    public class Transaction
    {
        [JsonProperty("category")]
        public string Category { get; set; }

        [JsonProperty("tag_groups")]
        public string[] TagGroups { get; set; }

        [JsonProperty("date")]
        public string Date { get; set; }

        [JsonProperty("comment")]
        public string Comment { get; set; }

        [JsonProperty("payee")]
        public string Payee { get; set; }

        [JsonProperty("outcome")]
        public string Outcome { get; set; }

        [JsonProperty("income")]
        public string Income { get; set; }

        [JsonProperty("account_income")]
        public string AccountIncome { get; set; }

        [JsonProperty("account_outcome")]
        public string AccountOutcome { get; set; }
    }
}