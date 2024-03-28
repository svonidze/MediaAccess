namespace FreedomFinanceBank.Integration;

using System.Globalization;
using System.Text.RegularExpressions;

using Common.Serialization.Json;

using OfficeOpenXml;

using Spire.Pdf;

public class Worker
{
    private const string TargetSeparator = @"\";

    private static readonly int[] AllowedColumns =
        {
            13, 14, 15, 16, 17
        };

    private static readonly Regex WordRegex = new("\\p{L}+");

    private static readonly Regex TransactionRegex = new(
        "Дата транзакции: (?<date>\\d{2}\\.\\d{2}\\.\\d{4}) .+ Сумма транзакции: (?<amount>\\d+(\\.\\d{1,2})?) (?<currency>\\w{3}) Операция: (Покупка|Возврат) (?<target>.+)?Учетный курс");

    public void Do(string pdfPath)
    {
        //var pdfPath = @"/home/sergey/Desktop/legal_statement_2024-01-01_2024-12-31_1711645429.pdf";
        var outputFileFormat = FileFormat.XLSX;

        var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(pdfPath);
        
        var directoryPath = Path.GetDirectoryName(pdfPath)!;
        var excelFileName = Path.Combine(directoryPath, $"{fileNameWithoutExtension}.{outputFileFormat}");

        ConvertPdfTo(outputFileFormat, pdfPath, excelFileName);

        var lines = ReadLines(excelFileName);

        //Console.WriteLine(lines.JoinToString(Environment.NewLine));

        foreach (var line in lines)
        {
            Console.WriteLine(
                TryConvert(line, out var transaction)
                    ? transaction?.ToJson()
                    : $"Cant convert {line}");
        }
    }

    private static List<string> ReadLines(string excelFileName)
    {
        var newLineBefore = 0;
        var lines = new List<string>();

        using var excelStream = File.OpenRead(excelFileName);
        using var excel = new ExcelPackage(excelStream);
        Console.WriteLine($"Total pages: {excel.Workbook.Worksheets.Count}");
        foreach (var worksheet in excel.Workbook.Worksheets)
        {
            foreach (var cell in worksheet.Cells)
            {
                var column = cell.Start.Column;

                if (!AllowedColumns.Contains(column)) continue;

                var text = cell.Text.Trim();

                //Console.WriteLine($"{cell.FullAddress}: {cell.Start.Column}: {text}");
                if (string.IsNullOrWhiteSpace(text))
                {
                    newLineBefore++;
                    if (newLineBefore == AllowedColumns.Length)
                    {
                        lines.Add(string.Empty);
                    }
                }
                else if (WordRegex.Match(text).Success)
                {
                    var last = lines.Count - 1;
                    lines[last] += " " + text;
                    newLineBefore = 0;
                }
            }
        }

        return lines;
    }

    private static void ConvertPdfTo(FileFormat outputFileFormat, string inputFilePath, string outputFilePath)
    {
        using var document = new PdfDocument();
        document.LoadFromFile(inputFilePath);
        document.SaveToFile(outputFilePath, outputFileFormat);
    }

    private static bool TryConvert(string line, out Transaction? transaction)
    {
        var match = TransactionRegex.Match(line);
        if (!match.Success)
        {
            transaction = null;
            return false;
        }

        var amountString = GetValue("amount");
        if (!decimal.TryParse(amountString, NumberStyles.Any, CultureInfo.InvariantCulture, out var amount))
        {
            throw new FormatException($"Cant parse '{amountString}' to {nameof(Decimal)}");
        }

        var dateString = GetValue("date");
        if (!DateTime.TryParseExact(dateString, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
        {
            throw new FormatException($"Cant parse '{dateString}' to {nameof(DateTime)}");
        }

        var target = GetValue("target");
        target = target.Contains(TargetSeparator)
            ? target.Split(TargetSeparator).LastOrDefault()?.Trim()
            : null;
        transaction = new Transaction
            {
                Currency = GetValue("currency"),
                Amount = amount,
                Date = date,
                Target = target,
            };
        return true;
        string GetValue(string groupName) => match.Groups[groupName].Value;
    }

    /*
     Formats
     4700 Безвозмездный перевод
     Дата транзакции: (?<date>\d{2}\.\d{2}\.\d{4}) .+ Сумма транзакции: (?<amount>\d+(\.\d{1,2})?) (?<currency>\w{3}) Операция: Покупка с нашей карты в чужом устройстве (?<target>.+)?Учетный курс
     Дата транзакции: 13.01.2024 Код авторизации: 008317 Номер карты: 5269********5327 Сумма транзакции: 2.29 EUR Операция: Покупка с нашей карты в чужом устройстве 045875630\LTU\VALENCIA \0517- SUP.EX.RAMON LLUL Учетный курс 451.33
     Дата транзакции: 13.01.2024 Код авторизации: 002879 Номер карты: 5269********5327 Сумма транзакции: 44 EUR Операция: Покупка с нашей карты в чужом устройстве Учетный курс 451.33
     Дата транзакции: 11.01.2024 Код авторизации: 102161 Номер карты: 5269********5327 Сумма транзакции: 8 EUR Операция: Возврат покупки по своей карте в чужом ТСП 322904186\AUS\VALENCIA \TEZENIS JUAN AUSTRIA T Учетный курс 451.33

     045875630\LTU\VALENCIA \0517- SUP.EX.RAMON LLUL
     */
}

class Transaction
{
    public DateTime Date { get; set; }

    public decimal Amount { get; set; }

    public string Currency { get; set; }

    public string? Target { get; set; }
}