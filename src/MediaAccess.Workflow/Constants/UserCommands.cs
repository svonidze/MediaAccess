namespace MediaServer.Workflow.Constants
{
    using System.Text.RegularExpressions;

    public static class UserCommands
    {
        public static class Torrent
        {
            public static class Groups
            {
                public const string BotName = "botName";

                public const string SearchRequest = "searchRequest";

                public const string Delimiter = "searchRequest";
            }

            public static readonly string[] Commands = {
                    "/torrent", "/t"
                };
            
            public static readonly Regex Regex = new Regex(
                @"/(torrent|t)(?<botName>@\w+)?(?<delimiter>\s+)?(?<searchRequest>.+)?");
        }

        public static class Kinopoisk
        {
            public static class Groups
            {
                public const string RusName = "rusName";

                public const string EngName = "engName";

                public const string StartYear = "startYear";
                
                public const string EndYear = "endYear";
            }

            public static readonly Regex Regex = new Regex(
                @"Фильм ""(?<rusName>.+)""\s?\((""(?<engName>.+)"")?,?\s?(?<startYear>\d{4})-?(?<endYear>(\d{4}|\.{3}))?\)");
        }

        public static class Film
        {
            public static class Groups
            {
                public const string Name = "name";
            }
            
            public static readonly Regex Regex = new Regex(@"Фильм (?<name>.+)");
        }

        public static class StartBotCommunication
        {
            public const string Command = @"/start";

            public static readonly Regex Regex = new Regex(Command);
        }
    }
}