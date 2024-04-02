namespace Common.Spreadsheets.Attributes
{
    using System;

    using Common.Spreadsheets.Enum;

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class SpreadsheetMappingAttribute : Attribute
    {
        public SpreadsheetMappingAttribute(string spreadsheetName)
        {
            this.SpreadsheetName = spreadsheetName;
            MappingMode = MappingMode.MappingByColumnNumber;
        }

        public SpreadsheetMappingAttribute(string spreadsheetName, short firstRowsToSkip)
            : this(spreadsheetName)
        {
            this.FirstRowsToSkip = firstRowsToSkip;
        }

        public string SpreadsheetName { get; set; }

        public short FirstRowsToSkip { get; set; }

        public MappingMode MappingMode { get; set; }
    }
}