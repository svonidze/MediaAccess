namespace Common.Exceptions
{
    using System;
    using System.Net;

    public class HttpException : Exception
    {
        public HttpException(HttpStatusCode httpStatusCode)
        {
            this.StatusCode = httpStatusCode;
        }

        public HttpException(HttpStatusCode httpStatusCode, string message) : base(message) =>
            this.StatusCode = httpStatusCode;

        public HttpException(HttpStatusCode httpStatusCode, string message, Exception inner) : base(message, inner) =>
            this.StatusCode = httpStatusCode;

        public HttpStatusCode StatusCode { get; }
    }
}