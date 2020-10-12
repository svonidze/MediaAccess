namespace Common.Net
{
    using System.Net;

    public static class Internet
    {
        public const string DefaultSite = "http://google.com/generate_204";
        
        public static bool IsAvailable(string url = DefaultSite)
        {
            try
            {
                using var client = new WebClient();
                using (client.OpenRead(url)) 
                    return true;
            }
            catch
            {
                return false;
            }
        }
    }
}