namespace MediaServer.Workflow
{
    using System;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Net.Http;

    using Common.Collections;
    using Common.DateTime;
    using Common.Http;

    using Jackett.Contracts;

    public class JackettIntegration : IJackettIntegration
    {
        private const string UrlFormat = "{0}/api/v2.0/indexers/all/results";

        private readonly IJackettAccessConfiguration config;

        public JackettIntegration(IJackettAccessConfiguration config)
        {
            this.config = config;
        }

        public ManualSearchResult SearchTorrents(string searchRequest, params string?[] trackerNames)
        {
            var url = string.Format(UrlFormat, this.config.Url);

            var queryValues = new NameValueCollection
                {
                    { ParameterKeys.ApiKey, this.config.ApiKey },
                    { ParameterKeys.Query, searchRequest },
                    { ParameterKeys.Date, DateTime.UtcNow.ToUnixTimestamp().ToString() }
                };
            trackerNames
                .Where(tn => !string.IsNullOrWhiteSpace(tn))
                .Foreach(tn => queryValues.Add(ParameterKeys.Tracker, tn!.ToLower()));
            
            var httpBuilder = new HttpRequestBuilder(this.config.Timeout).SetUrl(
                url,
                queryValues);
            return httpBuilder.RequestAndValidate<ManualSearchResult>(HttpMethod.Get);
        }
        
        private static class ParameterKeys
        {
            public const string ApiKey = "apikey";
            public const string Query = "Query";
            public const string Date = "_";
            public const string Tracker = "Tracker[]";
        }
    }
}