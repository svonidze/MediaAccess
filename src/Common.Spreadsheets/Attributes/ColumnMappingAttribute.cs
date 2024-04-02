namespace Common.Spreadsheets.Attributes
{
    using Common.Spreadsheets.Enum;
    using System;

    [AttributeUsage(
        AttributeTargets.Property,
        AllowMultiple = false,
        Inherited = true)]
    public class ColumnMappingAttribute : Attribute
    {
        public ColumnMappingAttribute(SpreadsheetColumns column)
        {
            this.Column = column;
        }

        public SpreadsheetColumns Column { get; private set; }

        public RepeatMode RepeatMode { get; set; }
    }
}
