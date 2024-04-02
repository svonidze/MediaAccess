namespace FreedomFinanceBank.Integration;

using System.Globalization;
using System.Text.RegularExpressions;

public abstract class TransactionConverter
{
    private const string DateFormat = "dd.MM.yyyy";

    private const string PayeeSeparator = @"\";

    private static readonly Regex CurrencyRegex = new("(\\p{L}{3})$");

    private static readonly Regex TransactionRegex = new(
        "Дата\\s?транзакции:\\s(?<date>\\d{2}\\.\\d{2}\\.\\d{4}).+"
        + "Сумма\\s?транзакции:\\s?(?<amount>\\d*(\\.\\d{1,2})?)\\s?(?<currency>\\w{3})\\s?"
        + "Операция:\\s?(Покупка|Возврат)(?<payee>.+)?" + "Учетный\\s?курс");

    public static bool TryExtractCurrency(string accountNumber, out string? currency)
    {
        var match = CurrencyRegex.Match(accountNumber);
        if (!match.Success)
        {
            currency = null;
            return false;
        }

        currency = match.Value;
        return true;
    }

    public static bool TryExtract(
        string input,
        out string? payee,
        out DateTime? createdAt,
        out decimal? amount,
        out string? currency)
    {
        input = input.Replace("\n", "");
        var match = TransactionRegex.Match(input);
        if (!match.Success)
        {
            payee = null;
            createdAt = null;
            amount = null;
            currency = null;
            return false;
        }

        payee = GetValue("payee");
        payee = payee.Contains(PayeeSeparator)
            ? payee.Split(PayeeSeparator).LastOrDefault()?.Trim()
            : null;

        createdAt = ConvertDate(GetValue("date"));

        amount = ConvertDecimal(GetValue("amount"));
        currency = GetValue("currency");

        return true;

        string GetValue(string groupName) => match.Groups[groupName].Value;
    }

    private static bool TryConvert(string input, out DateTime date) =>
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

    private static decimal ConvertDecimal(string input)
    {
        if (!TryConvert(input, out decimal value))
        {
            throw new FormatException($"Cant parse '{input}' to {nameof(Decimal)}");
        }

        return value;
    }
}