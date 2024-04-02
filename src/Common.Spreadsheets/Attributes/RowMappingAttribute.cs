namespace Common.Spreadsheets.Attributes
{
    using Common.Spreadsheets.Enum;
    using System;

    [AttributeUsage(
        AttributeTargets.Property,
        AllowMultiple = false,
        Inherited = true)]
    public class RowMappingAttribute : Attribute
    {
        public RowMappingAttribute(int row)
        {
            this.Row = row;
        }

        public int Row { get; private set; }

        public RepeatMode RepeatMode { get; set; }
    }
}
