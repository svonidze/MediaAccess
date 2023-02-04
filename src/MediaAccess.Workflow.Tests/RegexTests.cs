namespace MediaAccess.Workflow.Tests
{
    using System;
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

        [TestCase("/t", ExpectedResult = new string [0])]
        [TestCase("/torrent", ExpectedResult = new string [0])]
        [TestCase("/t request", ExpectedResult = new [] { "request"})]
        [TestCase("/torrent request", ExpectedResult = new [] { "request"})]
        [TestCase("/t@somebot request", ExpectedResult = new [] { "request"})]
        [TestCase("/torrent@somebot request", ExpectedResult = new [] { "request"})]
        [TestCase("/t search request", ExpectedResult = new [] { "search request"})]
        [TestCase("/torrent search request", ExpectedResult = new [] { "search request"})]
        [TestCase("/t@somebot search request", ExpectedResult = new [] { "search request"})]
        [TestCase("/torrent@somebot search request", ExpectedResult = new [] { "search request"})]
        [TestCase("/t Rutracker: search request", ExpectedResult = new [] { "Rutracker", "search request"})]
        [TestCase("/t LostFilm.tv: search request", ExpectedResult = new [] { "LostFilm.tv", "search request"})]
        public string[] Torrent(string text)
        {
            if (!UserCommands.Torrent.Regex.TryMath(text, out var match))
            {
                Assert.Fail($"Cant parse what you requested {text}");
            }

            var input = match.Groups[UserCommands.Torrent.Groups.Input].Value;

            if (!UserCommands.SearchRequest.Regex.TryMath(input, out match))
            {
                return Array.Empty<string>();
            }
            
            var searchRequest = match.Groups[UserCommands.SearchRequest.Groups.Input].Value;
            var trackerName = match.Groups[UserCommands.SearchRequest.Groups.TrackerName].Value;
            
            return new[]
                {
                    trackerName, searchRequest
                }.Where(v => !string.IsNullOrWhiteSpace(v)).ToArray();
        }

        [TestCase(@"Фильм ""Во все тяжкие"" (""Breaking Bad"", 2008-2013)", ExpectedResult = "Во все тяжкие Breaking Bad 2008 2013")]
        [TestCase(@"Фильм ""Зеленый слоник"" (1999)", ExpectedResult = "Зеленый слоник 1999")]
        [TestCase(@"Фильм ""Королевство"" (""Kingdom"", 2019-...) #kinopoisk", ExpectedResult = "Королевство Kingdom 2019 ...")]
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
                    GetValue(UserCommands.Kinopoisk.Groups.StartYear),
                    GetValue(UserCommands.Kinopoisk.Groups.EndYear)
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