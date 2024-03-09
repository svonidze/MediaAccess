namespace Common.Http
{
    using Common.Collections;

    using global::System;
    using global::System.Collections.Specialized;
    using global::System.Linq;

    public static class ContentDisposition
    {
        public static NameValueCollection? Parse(string contentDisposition)
        {
            if (string.IsNullOrEmpty(contentDisposition))
                return null;
            
            return contentDisposition
                .Split(';', StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Split("=", 2, StringSplitOptions.RemoveEmptyEntries))
                .ToNameValueCollection(
                    x => x[0].Trim(),
                    x => x.Length > 1
                        ? x[1].Trim().Replace("\"", null)
                        : null);
        }
    }
}