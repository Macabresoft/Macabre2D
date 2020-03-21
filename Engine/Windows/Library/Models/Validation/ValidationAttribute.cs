namespace Macabre2D.Engine.Windows.Models.Validation {

    using System;
    using System.Windows.Controls;

    [AttributeUsage(AttributeTargets.Property)]
    public abstract class ValidationAttribute : Attribute {

        public abstract ValidationResult Validate(object value);
    }
}