namespace Common.Spreadsheets.Implementation.Mappings
{
    using System;
    using System.Reflection;

    using Common.Spreadsheets.Enum;
    using Common.Spreadsheets.Mappings;
    using Common.Spreadsheets.Properties;

    public class ColumnPropertyMapping : IColumnMapping
    {
        private readonly PropertyInfo propertyInfo;

        public ColumnPropertyMapping(SpreadsheetColumns column, Type type, string propertyName) : this(
            column,
            GetProperty(type, propertyName))
        {
        }

        public ColumnPropertyMapping(SpreadsheetColumns column, PropertyInfo propertyInfo)
        {
            this.Column = column;

            this.propertyInfo = propertyInfo;
        }

        public ColumnPropertyMapping(string key, PropertyInfo propertyInfo)
        {
            this.Key = key;
            this.propertyInfo = propertyInfo;
        }

        public SpreadsheetColumns? Column { get; set; }

        public string Key { get; }

        public bool HasPropertyType => true;

        public Type PropertyType => this.propertyInfo.PropertyType;

        public string PropertyName => this.propertyInfo.Name;

        public void SetValue(object @object, object value) => this.propertyInfo.SetValue(@object, value, null);

        public object GetValue(object objectInstanceToInspect) =>
            this.propertyInfo.GetValue(objectInstanceToInspect, null);

        private static PropertyInfo GetProperty(Type type, string propertyName)
        {
            var property = type.GetProperty(propertyName);

            if (property == null)
            {
                throw new ArgumentException(string.Format(Resources.PropertyNotFound, propertyName, type));
            }

            return property;
        }
    }
}