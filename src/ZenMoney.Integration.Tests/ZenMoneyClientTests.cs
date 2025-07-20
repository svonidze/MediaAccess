namespace ZenMoney.Integration.Tests;

using Common.Http;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using ZenMoney.Integration.Contracts.Requests;

public class ZenMoneyClientTests
{
    private ZenMoneyClient _client;

    [SetUp]
    public void Setup()
    {
        var httpMessageHandler = new LoggingHandler(new HttpClientHandler(), _CreateLogger<LoggingHandler>());
        var httpRequestBuilder = new HttpRequestBuilder(_CreateLogger<ZenMoneyClient>(), httpMessageHandler);

        _client = new ZenMoneyClient(
            Options.Create(
                new ZenMoneyConfiguration
                    {
                        Cookie =
                            "_ga=GA1.2.1983459596.1698219072; _ym_uid=1698219072671102771; __utmz=180328751.1698219093.1.1.utmcsr=(direct)|utmccn=(direct)|utmcmd=(none); _ga_Z1Z1XNZELK=GS1.2.1722778499.7.0.1722778499.0.0.0; _ym_d=1739477874; __utma=180328751.1983459596.1698219072.1740936849.1741348584.35; PHPSESSID=e07unac0ne5i2te96s2knsu3p6"
                    }),
            _CreateLogger<ZenMoneyClient>(),
            httpRequestBuilder);
    }

    [Test]
    public async Task FetchTransactionsTest()
    {
        var trans = await _client.FetchTransactions(
            new TransactionFilter
                {
                    Payee = "Uber",
                    Skip = 0,
                    Limit = 21,
                    Finder = string.Empty,
                    TypeNotLike = "uit",
                });
    }

    [Test]
    public async Task FindAndUpdateAllTransactionsTest()
    {
        await _client.FindAndUpdateAllTransactions(
            getTransactionFilter: skip => new TransactionFilter
                {
                    //Payee = "Uber",
                    Skip = skip,
                    Limit = 100,
                    Finder = string.Empty,
                    TypeNotLike = "uit",
                    Account = 14491595 //CaixaBank
                },
            update: transaction =>
                {
                    var income = transaction.Income;
                    var outcome = transaction.Outcome;
                    transaction.Income = outcome;
                    transaction.Outcome = income;
                });
        Assert.Pass();
    }

    private static ILogger<T> _CreateLogger<T>()
    {
        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Trace));

        return loggerFactory.CreateLogger<T>();
    }
}