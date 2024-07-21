namespace FreedomFinanceBank.Contracts;

public class Transaction
{
    public DateTime CreatedAt { get; set; }

    public decimal Amount { get; set; }

    public string Currency { get; set; }

    public string? Payee { get; set; }
    
    public string? Description { get; set; }
}