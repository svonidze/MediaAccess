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
        /// <param name="codepoint"></param>
        /// <returns></returns>
        public static string ConvertCodepointToUnicode(this string codepoint)
        {
            if (codepoint.Contains('-'))
            {
                var pair = codepoint.Split('-');
                var hilos = new string[pair.Length];
                var chars = new char[pair.Length];
                for (var i = 0; i < pair.Length; i++)
                {
                    var part = Convert.ToInt32(pair[i], 16);
                    if (part >= 0x10000 && part <= 0x10FFFF)
                    {
                        var hi = Math.Floor((decimal)(part - 0x10000) / 0x400) + 0xD800;
                        var lo = ((part - 0x10000) % 0x400) + 0xDC00;
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

            var j = Convert.ToInt32(codepoint, fromBase: 16);
            return char.ConvertFromUtf32(j);
        }
    }
}