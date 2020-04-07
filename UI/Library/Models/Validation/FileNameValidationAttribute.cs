namespace Macabre2D.UI.Library.Models.Validation {

    using System.IO;
    using System.Windows.Controls;

    public class FileNameValidationAttribute : ValidationAttribute {
        private readonly char[] _invalidFileNameChars = Path.GetInvalidFileNameChars();
        private readonly string _invalidMessage = "Must not contain any invalid file name characters.";

        public override ValidationResult Validate(object value) {
            if (value is string text) {
                if (!string.IsNullOrEmpty(text) && text.IndexOfAny(this._invalidFileNameChars) < 0) {
                    return ValidationResult.ValidResult;
                }
            }

            return new CustomValidationResult(this._invalidMessage);
        }
    }
}