namespace FreedomFinanceBank.Contracts;

public class Transaction
{
    public DateTime Date { get; set; }

    public decimal Amount { get; set; }

    public string Currency { get; set; }

    public string? Target { get; set; }
}