namespace FreedomFinanceBank.Integration;

using Common.Collections;
using Common.Spreadsheets.Enum;
using Common.Spreadsheets.Excel.EPPlus;

using FreedomFinanceBank.Contracts;

public class Worker
{
    public static IEnumerable<Transaction> Extract(string fileName)
    {
        using var fileStream = File.OpenRead(fileName);
        using var reader = new SpreadsheetReader(fileStream);
        var sheet = reader.Read().AllowVerboseException().Single();

        foreach (var row in sheet.Cells.GroupBy(c => c.Row))
        {
            if (row.Key < 3) continue;

            var rowCells = row.Where(c => c.Column <= SpreadsheetColumns.J).GroupBy(r => r.Column).ToDictionary(
                r => r.Key,
                r => r.Single().Text);
            var keepWorking = TryExtract(rowCells, out var transaction);
            if (!keepWorking)
            {
                yield break;
            }

            if (transaction is not null)
            {
                yield return transaction;
            }
        }
    }

    private static bool TryExtract(
        IReadOnlyDictionary<SpreadsheetColumns, string> rowCells,
        out Transaction? transaction)
    {
        var textA = rowCells[SpreadsheetColumns.A];
        if (textA == "Итого:")
        {
            transaction = null;
            return false;
        }

        DateTime? operationDate = null;
        string? payee;

        var textJ = rowCells[SpreadsheetColumns.J].Trim();
        if (textJ is "Безвозмездный перевод" or "MATERIAL AID")
        {
            payee = rowCells[SpreadsheetColumns.E];
        }
        else if (!TransactionConverter.TryExtract(textJ, out payee, out operationDate, out _, out _))
        {
            Console.WriteLine($"Cannot parse '{textJ}'");
            transaction = null;
            return true;
        }

        // the dates in Column A and in Description might be different
        operationDate ??= TransactionConverter.ConvertDate(textA);

        transaction = new Transaction
            {
                CreatedAt = operationDate.Value,
                Payee = payee
            };

        if (TransactionConverter.TryConvert(rowCells[SpreadsheetColumns.H], out decimal amount))
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

        return true;
    }
}