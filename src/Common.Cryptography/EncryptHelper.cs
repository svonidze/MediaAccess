namespace Common.Cryptography
{
    using System;
    using System.Security.Cryptography;
    using System.Text;

    public static class EncryptHelper
    {
        public static readonly Encoding DefaultEncoding = Encoding.UTF8;

        public static string GetMD5(string data, Encoding encoding = null, bool isLower = false)
        {
            return GetHash(data, MD5.Create(), encoding, isLower);
        }

        public static string GetHash(
            string data,
            HashAlgorithm algorithm,
            Encoding encoding = null,
            bool isLower = false)
        {
            var format = isLower
                             ? "x2"
                             : "X2";
            var strHash = new StringBuilder();
            foreach (var b in GetBytes(data, algorithm, encoding))
            {
                strHash.Append(b.ToString(format));
            }

            return strHash.ToString();
        }

        public static byte[] GetBytes(string data, HashAlgorithm algorithm, Encoding encoding = null)
        {
            encoding = encoding ?? DefaultEncoding;
            return algorithm.ComputeHash(encoding.GetBytes(data));
        }

        public static string ToBase64(string data, Encoding encoding = null)
        {
            encoding = encoding ?? DefaultEncoding;
            return Convert.ToBase64String(encoding.GetBytes(data));
        }

        public static string ToBase64(byte[] data)
        {
            return Convert.ToBase64String(data);
        }

        public static string FromBase64(string data, Encoding encoding = null)
        {
            encoding = encoding ?? DefaultEncoding;
            return encoding.GetString(Convert.FromBase64String(data));
        }

        public static byte[] FromBase64String(string data)
        {
            return Convert.FromBase64String(data);
        }
    }
}