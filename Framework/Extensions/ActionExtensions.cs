namespace Macabre2D.Framework.Extensions {

    using System;

    /// <summary>
    /// Extensions for actions.
    /// </summary>
    public static class ActionExtensions {

        /// <summary>
        /// Invokes an action safely.
        /// </summary>
        /// <param name="action">The action.</param>
        public static void SafeInvoke(this Action action) {
            action?.Invoke();
        }

        /// <summary>
        /// Invokes an action with a parameter safely.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action">The action.</param>
        /// <param name="parameter">The parameter.</param>
        public static void SafeInvoke<T>(this Action<T> action, T parameter) {
            action?.Invoke(parameter);
        }
    }
}