namespace Common.Spreadsheets.Implementation.Mappings
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using Common.Spreadsheets.Mappings;

    public class RowSimpleTypeMapping : IRowMapping
    {
        private readonly Type rowDataType;

        public RowSimpleTypeMapping(Type rowDataType)
        {
            this.rowDataType = rowDataType;
        }

        public object CreateInstance() => Activator.CreateInstance(this.rowDataType);

        public void AddValidationResult(object @object, ValidationResult validationResult) =>
            throw new ValidationException(validationResult, null, @object);
    }
}