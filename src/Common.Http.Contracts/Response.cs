namespace Common.Http.Contracts
{
    public class Response : IResponse
    {
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
    }
}