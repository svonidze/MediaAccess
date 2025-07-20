namespace FreedomFinanceBank.Integration;

using Common.Spreadsheets.Contracts.Enum;
using Common.Spreadsheets.Excel.EPPlus;
using Common.System.Collections;

using FreedomFinanceBank.Contracts;

using Microsoft.Extensions.Logging;

public class ExcelStatementReader(ILogger<ExcelStatementReader> logger)
{
    private const SpreadsheetColumns LastColumn = SpreadsheetColumns.Z;

    public IEnumerable<Transaction> Extract(string fileName, int skipFirstRow = 3)
    {
        logger.LogInformation("Extracting transactions from {FileName}", fileName);
        using var fileStream = File.OpenRead(fileName);
        var reader = new SpreadsheetReader(fileStream);
        var sheet = reader.Read().AllowVerboseException().Single();

        var transactionCount = 0;
        foreach (var row in sheet.Cells.GroupBy(c => c.Row))
        {
            if (row.Key <= skipFirstRow) continue;

            var rowCells = row.Where(c => c.Column <= LastColumn)
                .GroupBy(r => r.Column)
                .ToDictionary(r => r.Key, r => r.Single().Text);
            var keepWorking = _TryExtract(rowCells, out var transaction);
            if (!keepWorking)
            {
                yield break;
            }

            if (transaction is not null)
            {
                transactionCount++;
                yield return transaction;
            }
        }

        logger.LogInformation("Extracted {TransactionCount} transactions from {FileName}", transactionCount, fileName);
    }

    private static bool _TryExtract(
        IReadOnlyDictionary<SpreadsheetColumns, string> rowCells,
        out Transaction? transaction)
    {
        var textA = rowCells[SpreadsheetColumns.A];
        if (textA == "Итого:")
        {
            transaction = null;
            return false;
        }

        DateTime? operationDateFromText = null;
        string? payeeFromText, descriptionFromText = null, currencyFromText = null;
        decimal? amountFromText = null;

        var textJ = rowCells[SpreadsheetColumns.J].Trim();
        if (textJ is "Безвозмездный перевод" or "MATERIAL AID" or "МАТЕРИАЛЬНАЯ ПОМОЩЬ")
        {
            payeeFromText = rowCells[SpreadsheetColumns.F];
        }
        else if (textJ.StartsWith("Выплата процентов по вкладу"))
        {
            payeeFromText = "Freedom Finance";
            descriptionFromText = textJ;
        }
        else if (!TransactionConverter.TryExtract(
                     textJ,
                     out payeeFromText,
                     out operationDateFromText,
                     out amountFromText,
                     out currencyFromText))
        {
            Console.WriteLine($@"Cannot parse '{textJ}'");
            transaction = null;
            return true;
        }

        // the dates in Column A and in Description might be different
        operationDateFromText ??= TransactionConverter.ConvertDate(textA);

        transaction = new Transaction
            {
                CreatedAt = operationDateFromText.Value,
                Payee = payeeFromText,
                Description = descriptionFromText
            };

        if (TransactionConverter.TryConvert(rowCells[SpreadsheetColumns.H], out var amount))
        {
            transaction.Amount = -amount;
        }
        else if (TransactionConverter.TryConvert(rowCells[SpreadsheetColumns.I], out amount))
        {
            transaction.Amount = amount;
        }

        var textG = rowCells[SpreadsheetColumns.G];
        if (TransactionConverter.TryExtractCurrency(textG, out var currency) && currency is not null)
        {
            transaction.Currency = currency;
        }

        if (transaction.Description == null
            && (Math.Abs(transaction.Amount) != amountFromText || transaction.Currency != currencyFromText))
        {
            transaction.Description = $"{amountFromText} {currencyFromText}";
        }

        return true;
    }
}