namespace Common.Http
{
    using global::System;
    using global::System.Net.Http;
    using global::System.Threading;
    using global::System.Threading.Tasks;

    public class LoggingHandler : DelegatingHandler
    {
        public LoggingHandler(HttpMessageHandler innerHandler)
            : base(innerHandler)
        {
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            Console.WriteLine("Request:");
            Console.WriteLine(request.ToString());
            if (request.Content != null && request.Content is not MultipartFormDataContent)
            {
                Console.WriteLine(await request.Content.ReadAsStringAsync(cancellationToken));
            }

            Console.WriteLine();

            var response = await base.SendAsync(request, cancellationToken);

            Console.WriteLine("Response:");
            Console.WriteLine(response.ToString());
            // if (response.Content != null)
            // {
            //     Console.WriteLine(await response.Content.ReadAsStringAsync());
            // }

            return response;
        }
    }
}