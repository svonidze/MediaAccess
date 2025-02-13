namespace Jackett.Contracts
{
    using Common.Http.Contracts;

    public class ActionDoneResponse : Response
    {
        public string result { get; set; }
    }
}