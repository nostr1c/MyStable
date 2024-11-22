using FluentValidation.Results;

namespace api.Helpers
{
    public class ValidationError
    {
        public string Field { get; set; }
        public string Error { get; set; }
    }

    public static class ValidationErrorHelper
    {
        public static List<ValidationError> ExtractValidationErrors(ValidationResult result)
        {
            return result.Errors
                .Select(x => new ValidationError
                {
                    Field = x.PropertyName,
                    Error = x.ErrorMessage
                })
                .ToList();
        }
    }
}
