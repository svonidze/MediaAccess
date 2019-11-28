namespace MediaServer.Workflow.Constants
{
    using System.Text.RegularExpressions;

    public static class BotCommands
    {
        public static class GoToPage
        {
            public const string Format = "Go {0} page";
            public static readonly Regex Regex = new Regex(string.Format(Format, @"(?<page>\d+)"));

            public static class Groups
            {
                public const string Page = "page";
            }
        }
        
        public static class PickTorrent
        {
            public const string Format = nameof(PickTorrent) + " {0}";
            public static readonly Regex Regex = new Regex(string.Format(Format, @"(?<hashUrl>\w{32})"));

            public static class Groups
            {
                public const string HashUrl = "hashUrl";
            }
        }
        
        public static class StartTorrent
        {
            public const string Format = nameof(StartTorrent) + " {0}";
            public static readonly Regex Regex = new Regex(string.Format(Format, @"(?<location>.*)"));

            public static class Groups
            {
                public const string HashUrl = "location";
            }
        }
        
        public static class DownloadTorrentFile
        {
            public const string Format = nameof(DownloadTorrentFile) + " {0}";
            public static readonly Regex Regex = new Regex(string.Format(Format, @"(?<hashUrl>\w{32})"));

            public static class Groups
            {
                public const string HashUrl = "hashUrl";
            }
        }
        
        public static class SortResults
        {
            public const string Format = "Sort by {0}";
            public static readonly Regex Regex = new Regex(string.Format(Format, @"(?<sortingType>\w+)"));

            public static class Groups
            {
                public const string SortingType = "sortingType";
            }
        }
    }
}