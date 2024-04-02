namespace Common.Spreadsheets.Model
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class SpreadsheetInformation
    {
        public SpreadsheetInformation()
        {
            this.ValidationResults = new List<ValidationResult>();
        }

        public List<ValidationResult> ValidationResults { get; set; }
    }
}
