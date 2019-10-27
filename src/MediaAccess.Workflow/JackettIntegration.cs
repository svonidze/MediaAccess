namespace MediaServer.Workflow
{
    using System;
    using System.Collections.Specialized;
    using System.Net.Http;

    using Common.DateTime;
    using Common.Http;

    using Jackett.Contracts;

    public class JackettIntegration
    {
        private readonly Settings settings;

        public JackettIntegration(Settings settings)
        {
            this.settings = settings;
        }

        public ManualSearchResult SearchTorrents(string searchRequest)
        {
            var url = $"{this.settings.Url}/api/v2.0/indexers/all/results";

            // Tracker%5B%5D
            var httpBuilder = new HttpRequestBuilder().SetUrl(
                url,
                new NameValueCollection
                    {
                        { "apikey", settings.ApiKey },
                        { "Query", searchRequest },
                        { "_", DateTime.UtcNow.ToUnixTimestamp().ToString() }
                    });
            Console.WriteLine(httpBuilder.Uri);
            return httpBuilder.RequestAndValidate<ManualSearchResult>(HttpMethod.Get);
        }
    }
}