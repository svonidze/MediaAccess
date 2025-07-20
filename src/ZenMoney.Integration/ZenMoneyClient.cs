using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Common.Http;
using Common.Serialization.Json;

using Microsoft.Extensions.Logging;

using ZenMoney.Integration.Contracts.Requests;
using ZenMoney.Integration.Contracts.Types;

namespace ZenMoney.Integration;

using Microsoft.Extensions.Options;

public class ZenMoneyClient(
    IOptions<IZenMoneyConfiguration> config,
    ILogger logger,
    IHttpRequestBuilder httRequestBuilder)
{
    public delegate TransactionFilter GetTransactionFilterDelegate(int skip);
    
    private const string HostUrl = "https://zenmoney.ru";

    private const string ApiV2Url = $"{HostUrl}/api/v2";
    private const string ApiS1Url = $"{HostUrl}/api/s1";

    private const string ReferrerUrl = $"{HostUrl}/a/";
    
    private static readonly Dictionary<string, string> Headers = new()
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
            { "Referer", ReferrerUrl }
        };

    private static readonly Dictionary<string, string> HeadersPost = new(Headers)
        {
            { "content-type", "application/x-www-form-urlencoded" }
        };

    private const string ReferrerPolicy = "strict-origin-when-cross-origin";

    public async Task<Transaction[]?> FetchTransactions(TransactionFilter filter)
    {
        var uri = UrlBuilder.Get($"{ApiV2Url}/transaction/?", filter.ToQueryNameValueCollection(urlEncode: true));

        return await httRequestBuilder.RequestAndValidateAsync<Transaction[]>(
            HttpMethod.Get,
            uri,
            _ConfigureRequest);
    }
    
    public async Task<Dictionary<int, Connector>?> FetchConnectors()
    {
        return await httRequestBuilder.RequestAndValidateAsync<Dictionary<int, Connector>>(
            HttpMethod.Get,
            $"{ApiS1Url}/connector/",
            _ConfigureRequest);
    }

    private void _ConfigureRequest(HttpRequestMessage request)
    {
        request.Headers.Add("cookie", config.Value.Cookie);
        foreach (var header in Headers)
        {
            request.Headers.TryAddWithoutValidation(header.Key, header.Value);
        }
    }

    private async Task<Dictionary<string, object>?> _UpdateTransaction(
        int transactionId,
        Transaction transaction)
    {
        var url = $"{ApiV2Url}/transaction/{transactionId}/";

        var response = await httRequestBuilder.RequestAndValidateAsync<Dictionary<string, object>>(
            HttpMethod.Post,
            url,
            request =>
                {
                    _ConfigureRequest(request);
                    request.SetContent(new StringContent(transaction.ToJson(), Encoding.UTF8, "application/json"));
                });

        return response;
    }
    
    public async Task FindAndUpdateAllTransactions(GetTransactionFilterDelegate getTransactionFilter, Action<Transaction> update)
    {
        var skip = 0;
        while (true)
        {
            // Fetch the current batch of transactions
            var transactionFilter = getTransactionFilter(skip);
            var limit = transactionFilter.Limit;
            
            var transactions = await this.FetchTransactions(transactionFilter);
            
            var transactionCount = transactions?.Length ?? 0;
            logger.LogInformation("Fetched {TransactionCount} transactions", transactionCount);
            if (transactions == null || transactions.Length == 0)
            {
                logger.LogWarning("No more transactions to process");
                break;
            }

            foreach (var pair in transactions.Index())
            {
                var transaction = pair.Item;
                var transactionId = transaction.Id;
                update(transaction);

                logger.LogDebug(
                    "Updating {Index}/{Count} transaction with id {TransactionId}",
                    pair.Index,
                    transactionCount,
                    transactionId);
                try
                {
                    var result = await this._UpdateTransaction(transactionId!.Value, transaction);
                    var message = result?.GetValueOrDefault("message")?.ToString();
                    if (message?.Contains("Error") == true || message?.Contains("fail") == true)
                    {
                        logger.LogError("Failed to update transaction id {TransactionId}: {Message}", transactionId, message);
                    }
                    logger.LogInformation(
                        "Successfully updated transaction id {TransactionId}: {Info}",
                        transactionId,
                        result?.ToJson());
                }
                catch (Exception e)
                {
                    logger.LogError(e, "Failed to update transaction id {TransactionId}", transactionId);
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

    public async Task CreateTransactions(params Transaction[] transactions)
    {
        foreach (var transaction in transactions)
        {
            await CreateTransaction(transaction);
        }
    }

    public async Task CreateTransaction(Transaction transaction)
    {
        var url = $"{ApiV2Url}/transaction/";
        var response = await httRequestBuilder.RequestAndValidateAsync<Dictionary<string, object>>(
            HttpMethod.Put,
            url,
            request =>
                {
                    _ConfigureRequest(request);
                    request.SetContent(new StringContent(transaction.ToJson(), Encoding.UTF8, "application/json"));
                });
    }
}