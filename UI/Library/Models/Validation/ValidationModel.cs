namespace Macabre2D.UI.Library.Models.Validation {

    using Macabre2D.Framework;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Text;

    public class ValidationModel : NotifyPropertyChanged, IDataErrorInfo {
        private readonly Dictionary<string, IEnumerable<ValidationAttribute>> _validationAttributes = new Dictionary<string, IEnumerable<ValidationAttribute>>();
        private readonly Dictionary<string, IEnumerable<string>> _validationErrors = new Dictionary<string, IEnumerable<string>>();

        public ValidationModel() {
            var properties = this.GetType().GetProperties().Where(x => x.GetCustomAttributes<ValidationAttribute>(true).Any());
            foreach (var property in properties) {
                this.Validate(property.GetValue(this), property.Name);
            }
        }

        public string Error {
            get {
                return this.CreateErrorFromMany(this.GetErrors());
            }
        }

        public bool HasErrors {
            get {
                return this.GetErrors().Any();
            }
        }

        public string this[string columnName] {
            get {
                return this.CreateErrorFromMany(this.GetErrors(columnName));
            }
        }

        public IEnumerable<string> GetErrors(string propertyName) {
            if (this._validationErrors.TryGetValue(propertyName, out var errors)) {
                return errors;
            }

            return new List<string>();
        }

        public IEnumerable<string> GetErrors() {
            var errors = new List<string>();
            foreach (var propertyErrors in this._validationErrors.Values) {
                errors.AddRange(propertyErrors);
            }

            errors.AddRange(this.RunCustomValidation());

            return errors;
        }

        protected virtual IEnumerable<string> RunCustomValidation() {
            return new List<string>();
        }

        protected override bool Set<T>(ref T field, T value, [CallerMemberName] string propertyName = "") {
            this.Validate(value, propertyName);
            return base.Set(ref field, value, propertyName);
        }

        protected void Validate(object value, [CallerMemberName] string propertyName = "") {
            if (!this._validationAttributes.TryGetValue(propertyName, out var attributes)) {
                var propertyInfo = this.GetType().GetProperty(propertyName);
                attributes = propertyInfo.GetCustomAttributes<ValidationAttribute>(true);
                this._validationAttributes[propertyName] = attributes;
            }

            this._validationErrors[propertyName] = attributes.Select(x => x.Validate(value)).Where(x => !x.IsValid).Select(x => x.ToString()).ToList();
            this.RaisePropertyChanged(nameof(this.HasErrors));
        }

        private string CreateErrorFromMany(IEnumerable<string> errors) {
            var stringBuilder = new StringBuilder();

            foreach (var error in errors) {
                stringBuilder.AppendLine(error);
            }

            return stringBuilder.ToString();
        }
    }
}