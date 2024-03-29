﻿namespace Common.Text
{
    public static class StringExtensions
    {
        public static bool IsNullOrEmpty(this string? input) => string.IsNullOrEmpty(input);

        public static string TrimEnd(this string? input, string? suffixToRemove)
        {
            if (input != null && suffixToRemove != null && input.EndsWith(suffixToRemove))
            {
                return input.Substring(0, input.Length - suffixToRemove.Length);
            }

            return input;
        }
    }
}