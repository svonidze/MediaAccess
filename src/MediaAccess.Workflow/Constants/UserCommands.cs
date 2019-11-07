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

            public static readonly Regex Regex = new Regex(
                @"/(torrent|t)(?<botName>@\w+)?(?<delimiter>\s+)?(?<searchRequest>.+)?");
        }

        public static class Kinopoisk
        {
            public static class Groups
            {
                public const string RusName = "rusName";

                public const string EngName = "engName";

                public const string Years = "years";
            }

            public static readonly Regex Regex = new Regex(
                @"Фильм ""(?<rusName>.+)""\s?\((""(?<engName>.+)"")?,?\s?(?<years>\d+-?\d*)\)");
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
            public static readonly Regex Regex = new Regex(@"/start");
        }
    }
}