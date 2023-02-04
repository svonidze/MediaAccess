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

                public const string Input = "input";
            }

            public static readonly string[] Commands =
                {
                    "/torrent", "/t"
                };

            public static readonly Regex Regex = new(
                "/(torrent|t)(?<botName>@\\w+)?\\s*(?<input>.+)?");
        }
        
        public static class SearchRequest
        {
            public static class Groups
            {
                public const string Input = "input";
                
                public const string TrackerName = "trackerName";
            }
            
            public static readonly Regex Regex = new(
                "(?<tracker>(?<trackerName>"
                + Patterns.TrackerName
                + "):)?\\s*(?<input>.+)?");
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

            public static readonly Regex Regex = new(
                @"Фильм ""(?<rusName>.+)""\s?\((""(?<engName>.+)"")?,?\s?(?<startYear>\d{4})-?(?<endYear>(\d{4}|\.{3}))?\)");
        }

        public static class Film
        {
            public static class Groups
            {
                public const string Name = "name";
            }

            public static readonly Regex Regex = new(@"Фильм (?<name>.+)");
        }

        public static class StartBotCommunication
        {
            public const string Command = @"/start";

            public static readonly Regex Regex = new(Command);
        }

        public static class Http
        {
            public static readonly Regex Regex = new(
                @"https?:\/\/(www\.)?[-a-zA-Z0-9@:%._\+~#=]{1,256}\.[a-zA-Z0-9()]{1,6}\b([-a-zA-Z0-9()@:%_\+.~#?&//=]*)");
        }
    }
}