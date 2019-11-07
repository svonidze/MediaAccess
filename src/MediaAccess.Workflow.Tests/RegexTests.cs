namespace MediaAccess.Workflow.Tests
{
    using System.Linq;

    using Common.Collections;
    using Common.Text;

    using MediaServer.Workflow.Constants;

    using NUnit.Framework;

    public class RegexTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [TestCase("/t", ExpectedResult = "")]
        [TestCase("/torrent", ExpectedResult = "")]
        [TestCase("/t request", ExpectedResult = "request")]
        [TestCase("/torrent request", ExpectedResult = "request")]
        [TestCase("/t@somebot request", ExpectedResult = "request")]
        [TestCase("/torrent@somebot request", ExpectedResult = "request")]
        [TestCase("/t search request", ExpectedResult = "search request")]
        [TestCase("/torrent search request", ExpectedResult = "search request")]
        [TestCase("/t@somebot search request", ExpectedResult = "search request")]
        [TestCase("/torrent@somebot search request", ExpectedResult = "search request")]
        public string Torrent(string text)
        {
            if (!UserCommands.Torrent.Regex.TryMath(text, out var match))
            {
                Assert.Fail($"Cant parse what you requested {text}");
            }

            return match.Groups[UserCommands.Torrent.Groups.SearchRequest].Value;
        }

        [TestCase(@"Фильм ""Во все тяжкие"" (""Breaking Bad"", 2008-2013)", ExpectedResult = "Во все тяжкие Breaking Bad 2008-2013")]
        [TestCase(@"Фильм ""Зеленый слоник"" (1999)", ExpectedResult = "Зеленый слоник 1999")]
        public string Kinoposik(string text)
        {
            if (!UserCommands.Kinopoisk.Regex.TryMath(text, out var match))
            {
                Assert.Fail($"Cant parse what you requested {text}");
            }

            string GetValue(string groupName) => match.Groups[groupName].Value;

            return new[]
                {
                    GetValue(UserCommands.Kinopoisk.Groups.RusName),
                    GetValue(UserCommands.Kinopoisk.Groups.EngName),
                    GetValue(UserCommands.Kinopoisk.Groups.Years)
                }.Where(v=>!v.IsNullOrEmpty()).JoinToString(' ');
        }
        
        [TestCase(@"Фильм Зеленый слоник", ExpectedResult = "Зеленый слоник")]
        [TestCase(@"Фильм Rick and Morty", ExpectedResult = "Rick and Morty")]
        public string Film(string text)
        {
            if (!UserCommands.Film.Regex.TryMath(text, out var match))
            {
                Assert.Fail($"Cant parse what you requested {text}");
            }

            string GetValue(string groupName) => match.Groups[groupName].Value;

            return GetValue(UserCommands.Film.Groups.Name);
        }
    }
}