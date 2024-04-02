namespace Common.Spreadsheets.Attributes
{
    using Common.Spreadsheets.Enum;
    using System;

    [AttributeUsage(
        AttributeTargets.Property,
        AllowMultiple = false,
        Inherited = true)]
    public class CellMappingAttribute : Attribute
    {
        public CellMappingAttribute(SpreadsheetColumns column, int row)
        {
            this.Column = column;
            this.Row = row;
        }

        public SpreadsheetColumns Column { get; private set; }

        public int Row { get; private set; }
    }
}
