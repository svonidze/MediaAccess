namespace Common.Text
{
    using System.Text.RegularExpressions;

    public static class RegexExtensions
    {
        public static bool TryMath(this Regex regex, string input, out Match match)
        {
            match = regex.Match(input);
            return match.Success;
        }
    }
}