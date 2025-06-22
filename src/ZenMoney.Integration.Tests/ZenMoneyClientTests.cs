namespace ZenMoney.Integration.Tests;

using Common.Http;

using Microsoft.Extensions.Logging;

public class ZenMoneyClientTests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public async Task Test1()
    {
        var logger = _CreateLogger();
        var httpMessageHandler = new LoggingHandler(new HttpClientHandler(), logger);
        var client = new ZenMoneyClient(logger, new HttpRequestBuilder(logger, httpMessageHandler));

        // var trans = await client.FetchTransactions(
        //     new TransactionFilter
        //         {
        //             //Payee = "Uber",
        //             Skip = 0,
        //             Limit = 21,
        //             Finder = string.Empty,
        //             TypeNotLike = "uit",
        //             Account = 14491595
        //         });
        await client.FindAndUpdateAllTransactions(
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

    private static ILogger _CreateLogger()
    {
        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Trace));

        return loggerFactory.CreateLogger<ZenMoneyClientTests>();
    }
}