namespace Common.Spreadsheets.Implementation.Mappings
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics;

    using Common.Spreadsheets.Mappings;
    using Common.Spreadsheets.Model;

    public class RowSpreadsheetInformationMapping : IRowMapping
    {
        private readonly Type rowDataType;

        public RowSpreadsheetInformationMapping(Type rowDataType)
        {
            Debug.Assert(typeof(SpreadsheetInformation).IsAssignableFrom(rowDataType));

            this.rowDataType = rowDataType;
        }

        public object CreateInstance() => Activator.CreateInstance(this.rowDataType);

        public void AddValidationResult(object @object, ValidationResult validationResult) =>
            ((SpreadsheetInformation)@object).ValidationResults.Add(validationResult);
    }
}