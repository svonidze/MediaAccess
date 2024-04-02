namespace Common.Spreadsheets.Attributes
{
    using System;

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class KeyColumnMappingAttribute : Attribute
    {
        public KeyColumnMappingAttribute(string keyColumn)
        {
            this.KeyColumn = keyColumn;
        }

        public string KeyColumn { get; private set; }
    }
}