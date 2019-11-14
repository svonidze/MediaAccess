namespace Common.Text
{
    // https://apps.timwhitlock.info/emoji/tables/unicode
    // http://www.unicode.org/emoji/charts/full-emoji-list.html
    public static class Emojies
    {
        private static Emoji Emoji(string code) => new Emoji { Code = code };
        
        public static readonly Emoji PlayButton = Emoji("25B6");
        public static readonly Emoji ReverseButton = Emoji("25C0");
        public static readonly Emoji NextTrackButton = Emoji("23ED");
        public static readonly Emoji LastTrackButton = Emoji("23EE");
        public static readonly Emoji UpDownArrow = Emoji("2195");
        public static readonly Emoji UpwardsBlackArrow = Emoji("2B06");
        public static readonly Emoji DownwardsBlackArrow = Emoji("2B07");
    }
}