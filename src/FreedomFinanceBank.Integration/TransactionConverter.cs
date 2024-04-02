namespace FreedomFinanceBank.Integration;

using System.Globalization;
using System.Text.RegularExpressions;

using FreedomFinanceBank.Contracts;

public abstract class TransactionConverter
{
    private const string DateFormat = "dd.MM.yyyy";

    private const string TargetSeparator = @"\";

    private static readonly Regex CurrencyRegex = new("(\\p{L}{3})$");
    
    private static readonly Regex TransactionRegex = new(
        "Дата\\s?транзакции:\\s(?<date>\\d{2}\\.\\d{2}\\.\\d{4}).+"
        + "Сумма\\s?транзакции:\\s?(?<amount>\\d*(\\.\\d{1,2})?)\\s?(?<currency>\\w{3})\\s?"
        + "Операция:\\s?(Покупка|Возврат)(?<target>.+)?" + "Учетный\\s?курс");

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
    
    public static bool TryExtract(string input, out string? payee, out DateTime? date)
    {
        input = input.Replace("\n", "");
        var match = TransactionRegex.Match(input);
        if (!match.Success)
        {
            payee = null;
            date = null;
            return false;
        }

        payee = GetValue("target");
        payee = payee.Contains(TargetSeparator)
            ? payee.Split(TargetSeparator).LastOrDefault()?.Trim()
            : null;

        date = ConvertDate(GetValue("date"));

        return true;
        
        string GetValue(string groupName) => match.Groups[groupName].Value;
    }
    
    public static bool TryConvert(string input, out Transaction? transaction)
    {
        input = input.Replace("\n", "");
        var match = TransactionRegex.Match(input);
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
                // TODO respect negative amount
                Amount = ConvertDecimal(GetValue("amount")),
                CreatedAt = ConvertDate(GetValue("date")),
                Payee = target,
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