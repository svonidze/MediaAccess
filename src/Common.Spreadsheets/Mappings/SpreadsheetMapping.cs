namespace Common.Spreadsheets.Mappings
{
    using System.Collections.Generic;

    using Common.Spreadsheets.Enum;

    public class SpreadsheetMapping : ISpreadsheetMapping
    {
        public string SpreadsheetName { get; set; }

        public short FirstRowsToSkip { get; set; }

        public MappingMode MappingMode { get; set; }

        public IRowMapping RowMapping { get; set; }

        public List<IColumnMapping> ColumnMappings { get; set; }
    }
}