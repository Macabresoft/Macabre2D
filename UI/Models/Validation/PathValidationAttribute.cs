namespace Macabre2D.UI.Models.Validation {

    using System.IO;
    using System.Text;
    using System.Windows.Controls;

    public sealed class PathValidationAttribute : ValidationAttribute {
        private readonly string _invalidMessage = "The following characters are not valid: ";
        private readonly char[] _invalidPathChars = Path.GetInvalidPathChars();

        public PathValidationAttribute() {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append("The following characters are not valid: ");
            foreach (var character in this._invalidPathChars) {
                stringBuilder.Append(character);
                stringBuilder.Append(", ");
            }

            stringBuilder.Remove(stringBuilder.Length - 2, 2);
            this._invalidMessage = stringBuilder.ToString();
        }

        public override ValidationResult Validate(object value) {
            if (value is string text) {
                if (!string.IsNullOrEmpty(text) && text.IndexOfAny(this._invalidPathChars) < 0) {
                    return ValidationResult.ValidResult;
                }
            }

            return new CustomValidationResult(this._invalidMessage);
        }
    }
}