namespace Macabre2D.Engine.Windows.Models.Validation {

    using System;
    using System.Windows.Controls;

    public sealed class ValidateModelAttribute : ValidationAttribute {

        public override ValidationResult Validate(object value) {
            if (value is ValidationModel model) {
                if (model.HasErrors) {
                    return new CustomValidationResult(model.Error);
                }

                return ValidationResult.ValidResult;
            }

            throw new NotSupportedException("'value' must be of type 'ValidationModel'");
        }
    }
}