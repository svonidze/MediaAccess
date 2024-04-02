namespace Common.Spreadsheets.Mappings
{
    using System;

    using Common.Spreadsheets.Enum;

    public interface IColumnMapping
    {
        SpreadsheetColumns? Column { get; set; }

        string Key { get; }

        bool HasPropertyType { get; }

        Type PropertyType { get; }

        string PropertyName { get; }

        void SetValue(object @object, object value);

        object GetValue(object objectInstanceToInspect);
    }
}