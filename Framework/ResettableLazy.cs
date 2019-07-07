namespace Macabre2D.Framework {

    using System;

    /// <summary>
    /// Wraps a <see cref="Lazy{T}"/> in a way that lets it be reset with the same <see
    /// cref="Func{T}"/> over and over.
    /// </summary>
    /// <typeparam name="T">The lazy type.</typeparam>
    public sealed class ResettableLazy<T> {
        private readonly Func<T> _valueFactory;
        private Lazy<T> _lazy;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResettableLazy{T}"/> class.
        /// </summary>
        public ResettableLazy() : this(new Func<T>(() => default)) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResettableLazy"/> class.
        /// </summary>
        /// <param name="valueFactory">The value factory.</param>
        public ResettableLazy(Func<T> valueFactory) {
            this._valueFactory = valueFactory;
            this._lazy = new Lazy<T>(valueFactory);
        }

        /// <summary>
        /// Gets a value indicating whether this instance is value created.
        /// </summary>
        /// <value><c>true</c> if this instance is value created; otherwise, <c>false</c>.</value>
        public bool IsValueCreated {
            get {
                return this._lazy.IsValueCreated;
            }
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>The value.</value>
        public T Value {
            get {
                return this._lazy.Value;
            }
        }

        /// <summary>
        /// Resets this instance.
        /// </summary>
        public void Reset() {
            this._lazy = this._lazy.Reset(this._valueFactory);
        }
    }
}