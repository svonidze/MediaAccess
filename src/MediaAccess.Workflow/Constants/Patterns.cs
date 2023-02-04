namespace MediaServer.Workflow.Constants;

using System.Text.RegularExpressions;

public static class Patterns
{
    public const string TrackerName = "[{\\p{L}}}\\.]+";

    public static class Http
    {
        public static readonly Regex Regex = new(
            @"https?:\/\/(www\.)?[-a-zA-Z0-9@:%._\+~#=]{1,256}\.[a-zA-Z0-9()]{1,6}\b([-a-zA-Z0-9()@:%_\+.~#?&//=]*)");
    }

    public static class Html
    {
        public static class Groups
        {
            public const string Title = "Title";
        }

        public static readonly Regex TitleRegex = new(@"\<title\b[^>]*\>\s*(?<Title>[\s\S]*?)\</title\>");
    }
}