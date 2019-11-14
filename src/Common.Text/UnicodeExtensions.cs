namespace Common.Text
{
    using System;
    using System.Linq;

    public static class UnicodeExtensions
    {
        // https://github.com/lajjne/emojione/blob/master/lib/cs/src/EmojiOne/EmojiOne.cs
        /// <summary>
        /// Converts unicode code point/code pair(s) to a unicode character.
        /// </summary>
        /// <param name="codepoint">Dash separated Unicode pair(s) like snail: 1F40C or flag of France 1F1EB-1F1F7</param>
        /// <returns></returns>
        public static string ConvertCodepointToUnicode(this string codepoint)
        {
            int ConvertToInt(string value) => Convert.ToInt32(value, fromBase: 16);

            if (!codepoint.Contains('-'))
                return char.ConvertFromUtf32(ConvertToInt(codepoint));
            var pairs = codepoint.Split('-');
            var hilos = new string[pairs.Length];
            var chars = new char[pairs.Length];
            for (var i = 0; i < pairs.Length; i++)
            {
                var part = ConvertToInt(pairs[i]);
                if (part >= 0x10000 && part <= 0x10FFFF)
                {
                    var hi = Math.Floor((decimal)(part - 0x10000) / 0x400) + 0xD800;
                    var lo = (part - 0x10000) % 0x400 + 0xDC00;
                    hilos[i] = new string(new[] { (char)hi, (char)lo });
                }
                else
                {
                    chars[i] = (char)part;
                }
            }

            return hilos.Any(x => x != null)
                ? string.Concat(hilos)
                : new string(chars);
        }
    }
}