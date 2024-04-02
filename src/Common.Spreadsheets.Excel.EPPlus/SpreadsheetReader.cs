namespace Common.Spreadsheets.Excel.EPPlus
{
    using Common.Spreadsheets.Enum;
    using Common.Spreadsheets.Model;
    using Common.System;

    using global::System.Text.RegularExpressions;

    using OfficeOpenXml;

    public class SpreadsheetReader : ISpreadsheetReader
    {
        // A1
        private readonly Regex cellAddressRegex = new("(?<column>[a-zA-Z]{1,2})(?<row>\\d+)");

        private readonly ExcelPackage excelPackage;

        public SpreadsheetReader(Stream excelStream)
        {
            this.excelPackage = new ExcelPackage(excelStream);
        }

        public IEnumerable<Sheet> Read()
        {
            foreach (var worksheet in this.excelPackage.Workbook.Worksheets)
            {
                var item = new Sheet
                    {
                        SheetName = worksheet.Name, 
                        Cells = new List<Cell>()
                    };
                foreach (var cell in worksheet.Cells)
                {
                    var match = this.cellAddressRegex.Match(cell.Address);
                    if (!match.Success)
                    {
                        throw new NotImplementedException();
                    }

                    item.Cells.Add(
                        new Cell
                            {
                                Column = match.Groups["column"].Value.StringToEnum<SpreadsheetColumns>(), 
                                Row = int.Parse(match.Groups["row"].Value), 
                                Text = cell.Text
                            });
                }

                yield return item;
            }
        }

        public IEnumerable<T> Read<T>()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            this.excelPackage.Dispose();
        }
    }
}