namespace Common.Http.Contracts
{
    public interface IResponse
    {
        string ErrorCode { get; set; }
        string ErrorMessage { get; set; }
    }
}