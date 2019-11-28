namespace Common.Http
{
    using System;
    using System.Collections.Specialized;
    using System.Linq;

    using Common.Collections;

    using JetBrains.Annotations;

    public static class ContentDisposition
    {
        [CanBeNull]
        public static NameValueCollection Parse(string contentDisposition)
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