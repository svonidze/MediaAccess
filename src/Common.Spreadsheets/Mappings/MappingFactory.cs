namespace Common.Spreadsheets.Mappings
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Common.Reflection;
    using Common.Spreadsheets.Attributes;
    using Common.Spreadsheets.Enum;
    using Common.Spreadsheets.Implementation.Mappings;
    using Common.Spreadsheets.Model;
    using Common.Spreadsheets.Properties;

    public class MappingFactory
    {
        public static ISpreadsheetMapping CreateSpreadsheetMapping<T>()
        {
            return CreateSpreadsheetMapping(typeof(T));
        }

        public static ISpreadsheetMapping CreateSpreadsheetMapping(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            var spreadsheetAttribute = type.GetCustomAttributes<SpreadsheetMappingAttribute>(true).SingleOrDefault();

            if (spreadsheetAttribute == null)
            {
                throw new ArgumentException(string.Format(Resources.SpreadsheetMappingAttributeNotSet, type));
            }

            var result = new SpreadsheetMapping
                {
                    ColumnMappings = new List<IColumnMapping>(), 
                    FirstRowsToSkip = spreadsheetAttribute.FirstRowsToSkip, 
                    RowMapping = CreateTypeMapping(type), 
                    SpreadsheetName = spreadsheetAttribute.SpreadsheetName, 
                    MappingMode = spreadsheetAttribute.MappingMode
                };

            foreach (var property in type.GetProperties())
            {
                switch (result.MappingMode)
                {
                    case MappingMode.MappingByFirsRowKeys:
                        {
                            var keyColumnAttribute =
                                property.GetCustomAttributes<KeyColumnMappingAttribute>(true).SingleOrDefault();

                            if (keyColumnAttribute != null)
                            {
                                result.ColumnMappings.Add(
                                    new ColumnPropertyMapping(keyColumnAttribute.KeyColumn, property));
                            }
                        }

                        break;
                    case MappingMode.MappingByColumnNumber:
                        {
                            var columnAttribute =
                                property.GetCustomAttributes<ColumnMappingAttribute>(true).SingleOrDefault();

                            if (columnAttribute != null)
                            {
                                result.ColumnMappings.Add(new ColumnPropertyMapping(columnAttribute.Column, property));
                            }
                        }

                        break;
                    default:
                        throw new NotImplementedException();
                }
            }

            return result;
        }

        private static IRowMapping CreateTypeMapping(Type type)
        {
            if (typeof(SpreadsheetInformation).IsAssignableFrom(type))
            {
                return new RowSpreadsheetInformationMapping(type);
            }
            
            return new RowSimpleTypeMapping(type);
        }
    }
}