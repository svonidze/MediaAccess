namespace Common.Spreadsheets.Model
{
    using System.Collections.Generic;

    public class Sheet
    {
        public string SheetName { get; set; }

        public IList<Cell> Cells { get; set; }
    }
}