namespace Macabre2D.Engine.Windows.Models.Validation {

    using System.Windows.Controls;

    public sealed class RequiredValidationAttribute : ValidationAttribute {
        public string FieldName { get; set; }

        public override ValidationResult Validate(object value) {
            if (value == null) {
                if (string.IsNullOrEmpty(this.FieldName)) {
                    return new CustomValidationResult("This is required.");
                }
                else {
                    return new CustomValidationResult($"{this.FieldName} is required.");
                }
            }

            return ValidationResult.ValidResult;
        }
    }
}