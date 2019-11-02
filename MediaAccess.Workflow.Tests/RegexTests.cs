namespace MediaAccess.Workflow.Tests
{
    using System.Text.RegularExpressions;

    using Common.Text;

    using NUnit.Framework;

    public class RegexTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [TestCase("/torrent request", ExpectedResult = "request")]
        [TestCase("/torrent@somebot request", ExpectedResult = "request")]
        [TestCase("/torrent search request", ExpectedResult = "search request")]
        [TestCase("/torrent@somebot search request", ExpectedResult = "search request")]
        public string Test1(string text)
        {
            var regex = new Regex(@"/torrent(?<botName>@\S+)? (?<searchRequest>.+)");
            var match = regex.Match(text);

            if (!match.Success)
            {
                Assert.Fail($"Cant parse what you requested {text}");
            }

            var searchRequest = match.Groups["searchRequest"];
            if (searchRequest.Value.IsNullOrEmpty())
            {
                Assert.Fail($"You requested search for nothing!");
            }

            return searchRequest.Value;
        }
    }
}