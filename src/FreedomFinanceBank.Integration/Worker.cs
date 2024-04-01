namespace FreedomFinanceBank.Integration;

using Common.Serialization.Json;

using FreedomFinanceBank.Contracts;

using OfficeOpenXml;

public class Worker
{
    private static readonly int[] AllowedColumns =
        {
            10 // J
        };

    public void Do(string excelPath)
    {
        //var pdfPath = @"/home/sergey/Desktop/legal_statement_2024-01-01_2024-12-31_1711645429.pdf";
        var lines = ReadLines(excelPath);

        //Console.WriteLine(lines.JoinToString(Environment.NewLine));

        foreach (var line in lines)
        {
            Console.WriteLine(
                TransactionConverter.TryConvert(line, out Transaction? transaction)
                    ? transaction?.ToJson()
                    : $"Cant convert {line}");
        }
    }

    private static IEnumerable<string> ReadLines(string excelFileName)
    {
        using var excelStream = File.OpenRead(excelFileName);
        using var excel = new ExcelPackage(excelStream);
        Console.WriteLine($"Total pages: {excel.Workbook.Worksheets.Count}");
        foreach (var worksheet in excel.Workbook.Worksheets)
        foreach (var cell in worksheet.Cells)
        {
            if (!AllowedColumns.Contains(cell.Start.Column)) continue;

            yield return cell.Text.Trim();
        }
    }
}