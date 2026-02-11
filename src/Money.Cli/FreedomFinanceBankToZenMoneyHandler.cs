namespace Money.Cli;

using System.Linq;
using System.Threading.Tasks;

using Common.Serialization.Json;

using FreedomFinanceBank.Integration;

using Microsoft.Extensions.Logging;

using ZenMoney.Integration;

internal class FreedomFinanceBankToZenMoneyHandler(
    ExcelStatementReader excelStatementReader,
    ZenMoneyClient zenMoneyClient,
    ILogger<FreedomFinanceBankToZenMoneyHandler> logger)
{
    public async Task Handle(string inputFilePath)
    {
        var ffinTransactions = excelStatementReader.Extract(inputFilePath).ToArray();
        if (ffinTransactions.Length == 0)
        {
            logger.LogWarning("No transactions found");
            return;
        }

        logger.LogDebug($"Found {ffinTransactions.Length} transactions: {ffinTransactions.ToJsonIndented()}");

        foreach (var transaction in ZenMoneyConverter.ConvertToTransactions(
                     ffinTransactions,
                     accountId: Constants.Wallets.FreedomFinanceBank.Eur))
        {
            var zenTransaction = await zenMoneyClient.CreateTransaction(transaction);
            logger.LogDebug(zenTransaction.ToJsonIndented());
        }
    }
}