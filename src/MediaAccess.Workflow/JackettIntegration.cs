namespace MediaServer.Workflow
{
    using System;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;

    using Common.DateTime;
    using Common.Http;
    using Common.System.Collections;

    using Jackett.Contracts;

    public class JackettIntegration : IJackettIntegration
    {
        private const string UrlFormat = "{0}/api/v2.0/indexers/all/results";

        private readonly IJackettAccessConfiguration config;

        private readonly HttpRequestBuilder httpRequestBuilder;

        public JackettIntegration(IJackettAccessConfiguration config, HttpRequestBuilder httpRequestBuilder)
        {
            this.config = config;
            this.httpRequestBuilder = httpRequestBuilder;
            if (this.config.Timeout is not null)
            {
                this.httpRequestBuilder.SetTimeout(this.config.Timeout.Value);
            }
        }

        /*
         apikey: l0nr2ggqjwzhctk12hnshsvjckh0a6er
Query: last of us
Tracker[]: lostfilm
_: 1676322671997
         fetch("http://192.168.0.243:9117/api/v2.0/indexers/all/results?apikey=l0nr2ggqjwzhctk12hnshsvjckh0a6er&Query=last%20of%20us&Tracker%5B%5D=lostfilm&_=1676322671997", {
  "headers": {
    "accept": "/", ***
        "accept-language": "en-US,en;q=0.9,ru;q=0.8",
        "x-requested-with": "XMLHttpRequest"
    },
    "referrerPolicy": "no-referrer",
    "body": null,
    "method": "GET",
    "mode": "cors",
    "credentials": "include"
});
         */
        public async Task<ManualSearchResult?> SearchTorrents(string searchRequest, params string?[] trackerNames)
        {
            var url = string.Format(UrlFormat, this.config.Url);

            var queryValues = new NameValueCollection
                {
                    { ParameterKeys.ApiKey, this.config.ApiKey },
                    { ParameterKeys.Query, searchRequest },
                    { ParameterKeys.Date, DateTime.UtcNow.ToUnixTimestamp().ToString() }
                };
            trackerNames.Where(tn => !string.IsNullOrWhiteSpace(tn))
                .Foreach(tn => queryValues.Add(ParameterKeys.Tracker, tn!.ToLower()));

            return await this.httpRequestBuilder.RequestAndValidateAsync<ManualSearchResult>(
                HttpMethod.Get,
                UrlBuilder.Get(url, queryValues));
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