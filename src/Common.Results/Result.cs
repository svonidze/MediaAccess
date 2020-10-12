namespace Common.Results
{
    public class Result
    {
        public bool Success { get; set; }

        public string Message { get; set; }
    }

    public class Result<T> : Result
    {
        public T Value { get; set; }
    }

    public class StatusResult<TStatusCode> : Result
        where TStatusCode : struct
    {
        public TStatusCode StatusCode { get; set; }
    }
    
    public class Result<T, TStatusCode> : StatusResult<TStatusCode>
        where TStatusCode : struct
    {
        public T Value { get; set; }
    }
}