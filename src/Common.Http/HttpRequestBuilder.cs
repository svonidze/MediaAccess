using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Common.Exceptions;
using Common.Http.Contracts;
using Common.Serialization.Json;
using Common.Text;
using Newtonsoft.Json;

namespace Common.Http
{
    public class HttpRequestBuilder
    {
        private readonly HttpClient httpClient;

        public string Uri { get; private set; }

        private HttpContent httpContent;

        public HttpRequestBuilder(TimeSpan? timeout = null)
        {
            //new LoggingHandler(
            // new HttpClientHandler
            // {
            //     UseCookies = false
            // })
            this.httpClient = new HttpClient
            {
                Timeout = timeout ?? TimeSpan.FromMinutes(2)
            };
        }

        public HttpRequestBuilder SetUrl(string url, NameValueCollection queryValues = null)
        {
            var builder = new UriBuilder(url);
            var query = HttpUtility.ParseQueryString(builder.Query);
            if (queryValues != null)
            {
//                foreach (var key in queryValues.AllKeys)
//                {
//                    var value = queryValues[key];
//                    var bytes = encoding.GetBytes(value);
//                    var encodedBytes = WebUtility.UrlEncodeToBytes(bytes, 0, bytes.Length);
//                    var encodedString = encoding.GetString(encodedBytes,0, encodedBytes.Length);
//                    Console.WriteLine(encodedString);
////                    query.Add(key, encodedString);
////                    query.Add(key, Uri.EscapeDataString(queryValues[key]));
//                }
                query.Add(queryValues);
            }

            Console.WriteLine(query.ToString());
            builder.Query = query.ToString();

            this.Uri = builder.ToString();
            return this;
        }

        public HttpRequestBuilder SetCookie(NameValueCollection cookieValues)
        {
            if (cookieValues == null)
            {
                return this;
            }

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

        public HttpRequestBuilder SetJsonPayload(string jsonPayload)
        {
            this.httpContent = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
            return this;
        }

        public T RequestAndValidate<T>(HttpMethod httpMethod) where T : IResponse, new()
        {
            string content;
            var response = this.RequestAndValidate<T>(httpMethod, out content);

            return response;
        }

        public T[] RequestAndValidateArrayOf<T>(HttpMethod httpMethod) where T : IResponse, new()
        {
            string content;
            this.RequestAndValidate<T>(httpMethod, out content, ignoreSerializationErrors: true);

            var collectionResponse = JsonConvert.DeserializeObject<T[]>(content);
            return collectionResponse;
        }

        private T RequestAndValidate<T>(
            HttpMethod httpMethod,
            out string content,
            bool ignoreSerializationErrors = false) where T : IResponse, new()
        {
            var taskResponseMessage = this.SendRequestAsync<T>(httpMethod);
            var responseMessage = taskResponseMessage.Result;

            content = responseMessage.Content.ReadAsStringAsync().Result;
            T response = new T();
            if (!content.IsNullOrEmpty())
            {
                try
                {
                    response = content.FromJsonTo<T>();
                }
                catch
                {
                    if (!ignoreSerializationErrors && responseMessage.IsSuccessStatusCode)
                    {
                        throw;
                    }
                }
            }

            Func<string, bool> isNotEmpty = input => !string.IsNullOrWhiteSpace(input);

            if (!responseMessage.IsSuccessStatusCode || isNotEmpty(response.ErrorCode) ||
                isNotEmpty(response.ErrorMessage))
            {
                var messageBuilder = new StringBuilder()
                    .AppendIf(
                        !responseMessage.IsSuccessStatusCode,
                        $"{responseMessage.StatusCode}. Reason: {responseMessage.ReasonPhrase}. ")
                    .AppendIf(isNotEmpty(response.ErrorCode), response.ErrorCode + " ")
                    .AppendIf(isNotEmpty(response.ErrorMessage), response.ErrorMessage + " ")
                    .AppendIf(!isNotEmpty(response.ErrorCode + response.ErrorMessage), content);

                throw new HttpException((int) responseMessage.StatusCode, messageBuilder.ToString());
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