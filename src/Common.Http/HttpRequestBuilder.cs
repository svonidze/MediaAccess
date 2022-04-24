namespace Common.Http
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.IO;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web;

    using Common.Exceptions;
    using Common.Http.Contracts;
    using Common.Serialization.Json;
    using Common.Text;

    using JetBrains.Annotations;

    using Newtonsoft.Json;

    public class HttpRequestBuilder
    {
        private UriBuilder uriBuilder;

        private readonly HttpClient httpClient;

        [CanBeNull]
        public Uri Uri => this.uriBuilder?.Uri;

        private HttpContent httpContent;

        public HttpRequestBuilder(TimeSpan? timeout = null, bool enableLogging = false)
        {
            static HttpMessageHandler CreateMessageLogger() =>
                new LoggingHandler(
                    new HttpClientHandler
                        {
                            UseCookies = false
                        });

            this.httpClient = enableLogging
                ? new HttpClient(CreateMessageLogger())
                : new HttpClient();
            this.httpClient.Timeout = timeout ?? TimeSpan.FromMinutes(2);
        }

        public HttpRequestBuilder SetUrl(string url, NameValueCollection queryValues = null)
        {
            this.uriBuilder = new UriBuilder(url);

            if (queryValues != null)
                this.AddUrlQueryValues(queryValues);

            return this;
        }

        public HttpRequestBuilder AddUrlQueryValues(NameValueCollection queryValues)
        {
            if (this.uriBuilder == null || queryValues == null)
                throw new NotSupportedException();

            var query = HttpUtility.ParseQueryString(this.uriBuilder.Query);
            query.Add(queryValues);

            this.uriBuilder.Query = query.ToString()!;

            return this;
        }

        public HttpRequestBuilder SetAuthorization(string token)
        {
            this.httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return this;
        }

        public HttpRequestBuilder SetCookie(NameValueCollection cookieValues)
        {
            if (cookieValues == null)
                return this;

            var cookies = HttpUtility.ParseQueryString(string.Empty);
            cookies.Add(cookieValues);

            this.httpClient.DefaultRequestHeaders.Add("Cookie", cookies.ToString());
            return this;
        }

        public HttpRequestBuilder SetMultipartFormDataDisposition(
            Dictionary<string, FileStream> dispositionFiles = null,
            NameValueCollection dispositionParameters = null)
        {
            var contentContainer = new MultipartFormDataContent();

            if (dispositionParameters != null)
            {
                foreach (string key in dispositionParameters.Keys)
                {
                    var value = dispositionParameters[key];

                    var payloadStringContent = new StringContent(value, Encoding.UTF8, "multipart/form-data");
                    payloadStringContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                        {
                            Name = key,
                        };
                    contentContainer.Add(payloadStringContent);
                }
            }

            if (dispositionFiles != null)
            {
                foreach (var newName in dispositionFiles.Keys)
                {
                    var fileStream = dispositionFiles[newName];

                    var streamContent = new StreamContent(fileStream);
                    streamContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                        {
                            Name = newName,
                            FileName = $"\"{fileStream.Name}\"",
                        };
                    streamContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

                    contentContainer.Add(streamContent);
                }
            }

            this.httpContent = contentContainer;

            return this;
        }

        public HttpRequestBuilder SetJsonPayload<T>(T payload) => this.SetPayload(payload.ToJson());

        public HttpRequestBuilder SetPayload(string jsonPayload, string mediaType = "application/json")
        {
            this.httpContent = new StringContent(jsonPayload, Encoding.UTF8, mediaType);
            return this;
        }

        public T RequestAndValidate<T>(HttpMethod httpMethod)
            where T : IResponse, new() =>
            this.RequestAndValidate<T>(httpMethod, out _);

        public T[] RequestAndValidateArrayOf<T>(HttpMethod httpMethod)
            where T : IResponse, new()
        {
            this.RequestAndValidate<T>(httpMethod, out var content, ignoreSerializationErrors: true);

            var collectionResponse = JsonConvert.DeserializeObject<T[]>(content);
            return collectionResponse;
        }

        private T RequestAndValidate<T>(
            HttpMethod httpMethod,
            out string content,
            bool ignoreSerializationErrors = false)
            where T : IResponse, new()
        {
            var taskResponseMessage = this.SendRequestAsync<T>(httpMethod);
            var responseMessage = taskResponseMessage.Result;

            content = responseMessage.Content.ReadAsStringAsync().Result;

            Console.WriteLine(content);

            T response = default;
            if (!content.IsNullOrEmpty())
            {
                try
                {
                    response = content.FromJsonTo<T>();
                }
                catch
                {
                    if (!ignoreSerializationErrors && responseMessage.IsSuccessStatusCode)
                        throw;

                    response = new T();
                }
            }

            if (response == null)
                throw new NotSupportedException();

            bool IsNotEmpty(string input) => !string.IsNullOrWhiteSpace(input);

            if (!responseMessage.IsSuccessStatusCode || IsNotEmpty(response.ErrorCode)
                || IsNotEmpty(response.ErrorMessage))
            {
                var messageBuilder = new StringBuilder()
                    .AppendIf(
                        !responseMessage.IsSuccessStatusCode,
                        $"{responseMessage.StatusCode}. Reason: {responseMessage.ReasonPhrase}. ")
                    .AppendIf(IsNotEmpty(response.ErrorCode), response.ErrorCode + " ")
                    .AppendIf(IsNotEmpty(response.ErrorMessage), response.ErrorMessage + " ").AppendIf(
                        !IsNotEmpty(response.ErrorCode + response.ErrorMessage),
                        content);

                throw new HttpException(responseMessage.StatusCode, messageBuilder.ToString());
            }

            return response;
        }

        private Task<HttpResponseMessage> SendRequestAsync<T>(HttpMethod httpMethod)
            where T : IResponse, new()
        {
            Task<HttpResponseMessage> taskResponseMessage;
            if (httpMethod == HttpMethod.Get)
            {
                taskResponseMessage = this.httpClient.GetAsync(this.Uri, HttpCompletionOption.ResponseContentRead);
            }
            else if (httpMethod == HttpMethod.Delete)
            {
                taskResponseMessage = this.httpClient.DeleteAsync(this.Uri);
            }
            else if (httpMethod == HttpMethod.Post)
            {
                taskResponseMessage = this.httpClient.PostAsync(this.Uri, this.httpContent);
            }
            else if (httpMethod == HttpMethod.Put)
            {
                taskResponseMessage = this.httpClient.PutAsync(this.Uri, this.httpContent);
            }
            else
            {
                throw new NotSupportedException(httpMethod.ToString());
            }

            return taskResponseMessage;
        }
    }
}