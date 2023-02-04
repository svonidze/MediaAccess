namespace MediaServer.Contracts
{
    public class ViewFilterConfiguration
    {
        public int ResultNumberOnPage { get; set; }

        public SortingType SortBy { get; set; }

        public bool Ascending { get; set; }

        public string MaxSize { get; set; }

        public string MinSize { get; set; }
        
        public string? TrackerName { get; set; }

    }
}