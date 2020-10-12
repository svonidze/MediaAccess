namespace ModulDengi.Contracts
{
    public class Project
    {
        public string Id { get; set; }

        public string Number { get; set; }

        public string BorrowerShortName { get; set; }

        public int MyInvestmentAmount { get; set; }

        public string LoanAmount { get; set; }

        public int LoanTerm { get; set; }

        public string LoanRate { get; set; }

        public string RaisedAmount { get; set; }
    }
}