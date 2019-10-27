using System;
using System.Collections.Specialized;
using System.Text;
using NUnit.Framework;

namespace Common.Http.Tests
{
    public class HttpRequestBuilderTests
    {
        [Test]
        public void METHOD()
        {
            var httpBuilder = new HttpRequestBuilder();
            httpBuilder.SetUrl("http://192.168.1.201:9117", new NameValueCollection
            {
                {"q", "Во все тяжкие Breaking Bad"}
            });

            Console.WriteLine(httpBuilder.Uri);
        }
    }
}