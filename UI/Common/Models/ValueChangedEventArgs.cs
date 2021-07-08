namespace Macabresoft.Macabre2D.UI.Common.Models {
    using System;

    /// <summary>
    /// A simple event that gives the value before and after changes.
    /// </summary>
    /// <typeparam name="T">The type.</typeparam>
    public sealed class ValueChangedEventArgs<T> : EventArgs {
        
        /// <summary>
        /// Initializes a new instance of the <see cref="ValueChangedEventArgs{T}" /> class.
        /// </summary>
        /// <param name="originalValue">The original value.</param>
        /// <param name="updatedValue">The updated value.</param>
        public ValueChangedEventArgs(T originalValue, T updatedValue) {
            this.OriginalValue = originalValue;
            this.UpdatedValue = updatedValue;
        }

        /// <summary>
        /// Gets the original value from before changes occurred.
        /// </summary>
        public T OriginalValue { get; }

        /// <summary>
        /// Gets the updated value after changes occurred.
        /// </summary>
        public T UpdatedValue { get; }
    }
}