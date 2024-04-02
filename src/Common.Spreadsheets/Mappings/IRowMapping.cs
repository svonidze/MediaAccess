namespace Common.Spreadsheets.Mappings
{
    using System.ComponentModel.DataAnnotations;

    public interface IRowMapping
    {
        object CreateInstance();

        void AddValidationResult(object @object, ValidationResult validationResult);
    }
}
