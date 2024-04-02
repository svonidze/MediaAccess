namespace Common.Spreadsheets.Mappings
{
    using System.Collections.Generic;

    using Common.Spreadsheets.Enum;

    public interface ISpreadsheetMapping
    {
        string SpreadsheetName { get; }

        short FirstRowsToSkip { get; }

        MappingMode MappingMode { get; }

        IRowMapping RowMapping { get; }

        List<IColumnMapping> ColumnMappings { get; }
    }
}