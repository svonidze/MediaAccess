namespace FreedomFinanceBank.Integration;

using System.Globalization;
using System.Text.RegularExpressions;

using FreedomFinanceBank.Contracts;

public abstract class TransactionConverter
{
    private const string DateFormat = "dd.MM.yyyy";

    private const string TargetSeparator = @"\";

    private static readonly Regex TransactionRegex = new(
        "Дата\\s?транзакции:\\s(?<date>\\d{2}\\.\\d{2}\\.\\d{4}).+"
        + "Сумма\\s?транзакции:\\s?(?<amount>\\d+(\\.\\d{1,2})?)\\s?(?<currency>\\w{3})\\s?"
        + "Операция:\\s?(Покупка|Возврат)(?<target>.+)?" + "Учетный\\s?курс");

    public static bool TryConvert(string line, out Transaction? transaction)
    {
        line = line.Replace("\n", "");
        var match = TransactionRegex.Match(line);
        if (!match.Success)
        {
            transaction = null;
            return false;
        }

        var target = GetValue("target");
        target = target.Contains(TargetSeparator)
            ? target.Split(TargetSeparator).LastOrDefault()?.Trim()
            : null;
        transaction = new Transaction
            {
                Currency = GetValue("currency"),
                Amount = ConvertDecimal(GetValue("amount")),
                Date = ConvertDate(GetValue("date")),
                Target = target,
            };
        return true;
        string GetValue(string groupName) => match.Groups[groupName].Value;
    }

    public static bool TryConvert(string input, out DateTime date) =>
        DateTime.TryParseExact(input, DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out date);

    public static bool TryConvert(string input, out decimal value) =>
        decimal.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out value);

    public static DateTime ConvertDate(string input)
    {
        if (!TryConvert(input, out DateTime date))
        {
            throw new FormatException($"Cant parse '{input}' to {nameof(DateTime)}");
        }

        return date;
    }

    public static decimal ConvertDecimal(string input)
    {
        if (!TryConvert(input, out decimal value))
        {
            throw new FormatException($"Cant parse '{input}' to {nameof(Decimal)}");
        }

        return value;
    }
}