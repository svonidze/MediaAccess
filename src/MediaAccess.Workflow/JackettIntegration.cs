namespace MediaServer.Workflow
{
    using System;
    using System.Collections.Specialized;
    using System.Net.Http;

    using Common.DateTime;
    using Common.Http;

    using Jackett.Contracts;

    public class JackettIntegration : IJackettIntegration
    {
        private readonly IJackettAccessConfiguration config;

        public JackettIntegration(IJackettAccessConfiguration config)
        {
            this.config = config;
        }

        public ManualSearchResult SearchTorrents(string searchRequest)
        {
            var url = $"{this.config.Url}/api/v2.0/indexers/all/results";

            // Tracker%5B%5D
            var httpBuilder = new HttpRequestBuilder(this.config.Timeout).SetUrl(
                url,
                new NameValueCollection
                    {
                        { "apikey", this.config.ApiKey },
                        { "Query", searchRequest },
                        { "_", DateTime.UtcNow.ToUnixTimestamp().ToString() }
                    });
            return httpBuilder.RequestAndValidate<ManualSearchResult>(HttpMethod.Get);
        }
    }
}