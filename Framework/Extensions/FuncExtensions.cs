namespace Macabre2D.Framework {

    using System;

    /// <summary>
    /// Extensions for <see cref="Func{TResult}"/> .
    /// </summary>
    public static class FuncExtensions {

        /// <summary>
        /// Safes the invoke.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func">The function.</param>
        /// <returns></returns>
        public static T SafeInvoke<T>(this Func<T> func) {
            if (func != null) {
                return func();
            }

            return default(T);
        }
    }
}