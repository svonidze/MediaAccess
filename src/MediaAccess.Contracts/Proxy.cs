namespace MediaServer.Contracts
{
    public class Proxy
    {
        public Proxy()
        {
        }

        public Proxy(string host, int port, string userName, string password)
        {
            Host = host;
            Port = port;
            UserName = userName;
            Password = password;
        }

        public string Host { get; set; }
        public int Port { get; set; }

        public string UserName { get; set; }
        public string Password { get; set; }
    }
}