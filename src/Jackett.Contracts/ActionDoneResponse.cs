namespace Jackett.Contracts
{
    using global::Common.Http.Contracts;

    public class ActionDoneResponse: Response
    {
        public string result { get; set; }
    }
}