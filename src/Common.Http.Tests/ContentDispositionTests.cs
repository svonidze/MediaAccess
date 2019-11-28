namespace Common.Http.Tests
{
    using NUnit.Framework;

    public class ContentDispositionTests
    {
        [TestCase(
            @"attachment; filename=""Scrubs - S0E1-09 - rus - DVDRip - MTV.torrent""; filename*=UTF-8''Scrubs%20-%20S0E1-09%20-%20rus%20-%20DVDRip%20-%20MTV.torrent",
            "filename",
            ExpectedResult = "Scrubs - S0E1-09 - rus - DVDRip - MTV.torrent")]
        public string Parse(string contentDisposition, string key)
        {
            return ContentDisposition.Parse(contentDisposition)?[key];
        }
    }
}