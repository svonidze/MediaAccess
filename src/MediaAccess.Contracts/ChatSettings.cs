namespace MediaServer.Contracts
{
    public class ChatSettings
    {
        public int PageResultNumber { get; set; }

        public string OrderBy { get; set; }

        public string SortingDirection { get; set; }

        public string? MaxSize { get; set; }

        public string? MinSize { get; set; }
    }
}