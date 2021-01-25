namespace ModulDengi.Contracts
{
    using Common.Serialization.Json;

    public struct Description
    {
        public Description(int projectId, string borrowerName = null, int? timePeriodInDays = null, int? percent = null)
        {
            this.ProjectId = projectId;
            this.BorrowerName = borrowerName;
            this.Percent = percent;
            this.TimePeriodInDays = timePeriodInDays;
        }

        public int ProjectId { get; set; }

        public string BorrowerName { get; set; }

        public int? TimePeriodInDays { get; set; }

        public int? Percent { get; set; }

        public override string ToString() => this.ToJsonIndented();
    }
}