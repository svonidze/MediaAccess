using global::System;
using global::System.Net;
using global::System.Text;
using NUnit.Framework;

namespace Common.Http.Tests
{
    public class Tests
    {
        private static readonly Encoding[] Encodings =
        {
            Encoding.ASCII, Encoding.BigEndianUnicode, Encoding.Default, Encoding.Unicode, Encoding.UTF32, Encoding.UTF8
        };

        [SetUp]
        public void Setup()
        {
        }

        [TestCaseSource(nameof(Encodings))]
        public void Test1(Encoding encoding)
        {
//            var q2 = "%25D0%2592%25D0%25BE%2b%25D0%25B2%25D1%2581%25D0%25B5%2b%25D1%2582%25D1%258F%25D0%25B6%25D0%25BA%25D0%25B8%25D0%25B5%2bBreaking%2bBad";
//            var q = @"%D0%92%D0%BE%20%D0%B2%D1%81%D0%B5%20%D1%82%D1%8F%D0%B6%D0%BA%D0%B8%D0%B5%20Breaking%20Bad";
            var sourceString = "Во все тяжкие Breaking Bad";

            Console.WriteLine($"{encoding}");

            var bytes = encoding.GetBytes(sourceString);
            var encodedBytes = WebUtility.UrlEncodeToBytes(bytes, 0, bytes.Length);
            var encodedString = encoding.GetString(encodedBytes, 0, encodedBytes.Length);
            Console.WriteLine(encodedString);

            var encodedBytes2 = encoding.GetBytes(encodedString);
            CollectionAssert.AreEqual(encodedBytes, encodedBytes2);

            var decodedBytes = WebUtility.UrlDecodeToBytes(encodedBytes2, 0, encodedBytes2.Length);
            var decodedString = encoding.GetString(decodedBytes);
            Console.WriteLine(decodedString);
            Assert.AreEqual(sourceString, decodedString);


//                Console.WriteLine($"{encoding}::{System.Net.WebUtility.UrlDecode(q)}");
//                Console.WriteLine($"{encoding}::{System.Net.WebUtility.UrlDecode(q2)}");
//
//                Console.WriteLine($"{encoding}::{System.Net.WebUtility.UrlEncode(source)}");
        }
    }
}