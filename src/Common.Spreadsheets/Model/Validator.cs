using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Common.Spreadsheets.Model
{
    public class Validator
    {
        public static bool TryValidate<T> (T item, out List<ValidationResult> validationResults)
        {
            var validationContext = new ValidationContext(item, null, null);

            var localResults = new List<ValidationResult>();

            var isValid = System.ComponentModel.DataAnnotations.Validator.TryValidateObject(item, validationContext, localResults, true);

            validationResults = localResults;

            return isValid;
        }
    }
}
