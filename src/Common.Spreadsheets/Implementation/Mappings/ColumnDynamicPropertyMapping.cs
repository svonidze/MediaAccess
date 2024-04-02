namespace Common.Spreadsheets.Implementation.Mappings
{
    using System;
    using System.Collections.Generic;

    using Common.Spreadsheets.Enum;
    using Common.Spreadsheets.Mappings;
    using Common.Spreadsheets.Properties;

    public class ColumnDynamicPropertyMapping : IColumnMapping
    {
        public ColumnDynamicPropertyMapping(SpreadsheetColumns column, string propertyName)
        {
            this.Column = column;
            this.PropertyName = propertyName;
        }

        public SpreadsheetColumns? Column { get; set; }

        public string Key { get; private set; }

        public bool HasPropertyType => false;

        public Type PropertyType => throw new InvalidOperationException();

        public string PropertyName { get; }

        private IDictionary<string, object> AsExpando(object @object)
        {
            var dictionary = @object as IDictionary<string, object>;

            if (dictionary == null)
            {
                throw new ArgumentException(Resources.ObjectIsNotExpando, nameof(@object));
            }

            return dictionary;
        }

        public void SetValue(object @object, object value)
        {
            var dictionary = this.AsExpando(@object);

            dictionary.Add(this.PropertyName, value);
        }

        public object GetValue(object objectInstanceToInspect)
        {
            var dictionary = this.AsExpando(objectInstanceToInspect);

            if (dictionary.ContainsKey(this.PropertyName))
            {
                return dictionary[this.PropertyName];
            }

            return null;
        }
    }
}
