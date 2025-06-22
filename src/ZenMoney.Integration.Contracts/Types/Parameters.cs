namespace ZenMoney.Integration.Contracts.Types;

using Newtonsoft.Json;

public class Parameters
{
    [JsonProperty("wmid")]
    public Wmid Wmid { get; set; }

    [JsonProperty("account_autocreation")]
    public AccountAutocreation AccountAutocreation { get; set; }

    [JsonProperty("sync_state")]
    public SyncState SyncState { get; set; }

    [JsonProperty("parser")]
    public Parser Parser { get; set; }

    [JsonProperty("comment")]
    public Comment Comment { get; set; }

    [JsonProperty("token")]
    public Token Token { get; set; }

    [JsonProperty("syncid")]
    public Syncid Syncid { get; set; }

    [JsonProperty("bookmarklet")]
    public Bookmarklet Bookmarklet { get; set; }

    [JsonProperty("allow-user-accounts")]
    public AllowUserAccounts AllowUserAccounts { get; set; }

    [JsonProperty("email-integration")]
    public EmailIntegration EmailIntegration { get; set; }
}