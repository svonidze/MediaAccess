namespace Common.Net
{
    using System.Net;

    public static class Internet
    {
        public static bool IsAvailable(string url = "http://google.com/generate_204")
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