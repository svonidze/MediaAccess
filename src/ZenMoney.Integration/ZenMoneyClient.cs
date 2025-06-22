using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using Common.Http;

public class ZenMoneyClient
{
    IHttpRequestBuilder _httRequestBuilder;
    
    // update this from the website
    private const string COOKIE =
        "PHPSESSID=njkv8l77l0ah940t3pb0qoih37; _ga=GA1.2.1983459596.1698219072; _ym_uid=1698219072671102771; __utmc=180328751; __utmz=180328751.1698219093.1.1.utmcsr=(direct)|utmccn=(direct)|utmcmd=(none); _ga_Z1Z1XNZELK=GS1.2.1722778499.7.0.1722778499.0.0.0; _ym_d=1739477874; _ym_isad=2; __utma=180328751.1983459596.1698219072.1739477875.1740931728.33";

    private const string HOST_URL = "https://zenmoney.ru";

    private const string API_URL = $"{HOST_URL}/api/v2";

    private const string REFERRER_URL = $"{HOST_URL}/a/";

    private static readonly HttpClient Client = new HttpClient();

    private static readonly Dictionary<string, string> HEADERS = new()
        {
            { "accept", "application/json, text/javascript, */*; q=0.01" },
            { "accept-language", "en-US,en;q=0.9,ru;q=0.8" },
            { "priority", "u=1, i" },
            { "sec-ch-ua", "\"Not(A:Brand\";v=\"99\", \"Google Chrome\";v=\"133\", \"Chromium\";v=\"133\"" },
            { "sec-ch-ua-mobile", "?0" },
            { "sec-ch-ua-platform", "\"Linux\"" },
            { "sec-fetch-dest", "empty" },
            { "sec-fetch-mode", "cors" },
            { "sec-fetch-site", "same-origin" },
            { "x-requested-with", "XMLHttpRequest" },
            { "cookie", COOKIE },
            { "Referer", REFERRER_URL }
        };

    private static readonly Dictionary<string, string> HEADERS_POST = new(HEADERS)
        {
            { "content-type", "application/x-www-form-urlencoded" }
        };

    private const string REFERRER_POLICY = "strict-origin-when-cross-origin";

    public void Add(IEnumerable<ZenMoney.Integration.Contracts.Types.Transaction> transactions)
    {
        _httRequestBuilder.SetCookie(COOKIE);
    }
    
    public static async Task<List<Dictionary<string, object>>> FetchTransactions(int limit, int skip, string payee)
    {
        // Fetches a batch of transactions based on limit and skip.
        var url =
            $"{API_URL}/transaction/?limit={limit}&skip={skip}&type_notlike=uit&payee%5B%5D={Uri.EscapeDataString(payee)}&finder=";
        Console.WriteLine(url);

        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        foreach (var header in HEADERS)
        {
            request.Headers.TryAddWithoutValidation(header.Key, header.Value);
        }

        var response = await Client.SendAsync(request);
        if (response.IsSuccessStatusCode)
        {
            string responseBody = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<Dictionary<string, object>>>(responseBody);
        }

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            Console.WriteLine($"HTTPError: {response.ReasonPhrase}. Retrying...");
            await Task.Delay(15000);
        }
        else
        {
            response.EnsureSuccessStatusCode();
        }

        return null;
    }

    private static async Task<Dictionary<string, object>?> _UpdateTransaction(
        string transactionId,
        Dictionary<string, object> payload)
    {
        // Updates a specific transaction with a POST call.
        var url = $"{API_URL}/transaction/{transactionId}/";
        using HttpContent content = new StringContent(
            JsonSerializer.Serialize(payload),
            Encoding.UTF8,
            "application/json");

        using var request = new HttpRequestMessage(HttpMethod.Post, url);
        request.Content = content;
        foreach (var header in HEADERS_POST)
        {
            request.Headers.TryAddWithoutValidation(header.Key, header.Value);
        }

        var response = await Client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var responseBody = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<Dictionary<string, object>>(responseBody);
    }

    private static async Task FindAndUpdateAllTransactions(string oldPayee, string newPayee, int limit = 40)
    {
        var skip = 0;
        while (true)
        {
            // Fetch the current batch of transactions
            var transactions = await FetchTransactions(limit, skip, oldPayee);
            var transactionCount = transactions?.Count ?? 0;
            Console.WriteLine($"Fetched {transactionCount} transactions: {JsonSerializer.Serialize(transactions)}");

            if (transactions == null || transactions.Count == 0)
            {
                Console.WriteLine("No more transactions to process.");
                break;
            }

            foreach (var transaction in transactions)
            {
                // Prepare payload for POST request (customize as needed)
                var transactionId = transaction["id"].ToString();
                var payload = new Dictionary<string, object>
                    {
                        { "category", "0" },
                        { "tag_groups", new[] { "13126654" } },
                        { "date", transaction["date"] },
                        { "comment", transaction["comment"] },
                        { "payee", newPayee },
                        { "outcome", transaction["outcome"] },
                        { "income", 0 },
                        { "account_income", transaction["account_income"] },
                        { "account_outcome", transaction["account_outcome"] },
                        { "merchant", "" },
                        { "id", transactionId }
                    };

                // Update each transaction
                try
                {
                    var result = await _UpdateTransaction(transactionId, payload);
                    Console.WriteLine(
                        $"Successfully updated transaction id {transactionId}: {JsonSerializer.Serialize(result)}");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Failed to update transaction id {transactionId}: {e.Message}");
                }
            }

            if (transactionCount < limit)
            {
                Console.WriteLine("Nothing more to process.");
                break;
            }

            // Increment for the next batch
            skip += limit;
        }
    }
}