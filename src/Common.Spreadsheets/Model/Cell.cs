namespace Common.Spreadsheets.Model
{
    using Common.Spreadsheets.Enum;

    public class Cell
    {
        public SpreadsheetColumns Column { get; set; }

        public int Row { get; set; }

        public string Text { get; set; }
    }
}