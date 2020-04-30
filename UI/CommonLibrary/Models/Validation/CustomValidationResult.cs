namespace Macabre2D.UI.CommonLibrary.Models.Validation {

    using System.Windows.Controls;

    public class CustomValidationResult : ValidationResult {

        public CustomValidationResult(string error) : base(false, error) {
        }

        public CustomValidationResult() : base(true, null) {
        }

        public override string ToString() {
            if (this.ErrorContent is string error) {
                return error;
            }

            return base.ToString();
        }
    }
}