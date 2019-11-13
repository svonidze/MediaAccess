namespace Common.Text
{
    public static class EmojiExtensions
    {
        public static string ToUnicode(this Emoji emoji)
        {
            return emoji.Code.ConvertCodepointToUnicode();
        }
    }
}