namespace Macabre2D.Framework {

    using System;

    /// <summary>
    /// Arguments for when a value has changed.
    /// </summary>
    /// <typeparam name="T">The type of value.</typeparam>
    /// <seealso cref="EventArgs"/>
    public sealed class ValueChangedEventArgs<T> : EventArgs {

        /// <summary>
        /// The new value.
        /// </summary>
        public readonly T NewValue;

        /// <summary>
        /// The old value.
        /// </summary>
        public readonly T OldValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValueChangedEventArgs{T}"/> class.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        public ValueChangedEventArgs(T oldValue, T newValue) {
            this.OldValue = oldValue;
            this.NewValue = newValue;
        }
    }
}