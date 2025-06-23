namespace ZenMoney.Integration.Contracts.Requests;

using Common.Http.Contracts;

public class TransactionFilter
{
    [HttpQueryKey("limit", order: 0)]
    public int Limit { get; set; }

    [HttpQueryKey("skip", order: 1)]
    public int Skip { get; set; }

    [HttpQueryKey("payee[]", order: 3)]
    public string? Payee { get; set; }

    [HttpQueryKey("type_notlike", order: 2)]
    public string? TypeNotLike { get; set; }

    [HttpQueryKey("finder", order: 4)]
    public string? Finder { get; set; }

    [HttpQueryKey("account[]", order: 3)]
    public int? Account { get; set; }
}