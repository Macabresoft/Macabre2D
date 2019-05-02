namespace Macabre2D.Framework.Extensions {

    using System;

    /// <summary>
    /// Extension methods for <see cref="System.Lazy{T}"/>.
    /// </summary>
    public static class LazyExtensions {

        /// <summary>
        /// Resets the lazy variable, but only if the value has already been created.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="lazyVariable">The lazy variable.</param>
        /// <param name="valueFactory">The value factory.</param>
        /// <returns>
        /// A new lazy variable if the previous one already has a value created; otherwise, it
        /// returns the original lazy variable.
        /// </returns>
        public static Lazy<T> Reset<T>(this Lazy<T> lazyVariable, Func<T> valueFactory) {
            if (lazyVariable == null || lazyVariable.IsValueCreated) {
                return new Lazy<T>(valueFactory);
            }

            return lazyVariable;
        }
    }
}