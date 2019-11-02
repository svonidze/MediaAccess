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
        
        public static class Blackhole
        {
            public const string Format = "Blackhole {0}";
            public static readonly Regex Regex = new Regex(string.Format(Format, @"(?<hashUrl>\w{32})"));

            public static class Groups
            {
                public const string HashUrl = "hashUrl";
            }
        }
        
        
    }
}