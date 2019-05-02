namespace Macabre2D.Framework.Extensions {

    using System;

    /// <summary>
    /// Extension methods for Events.
    /// </summary>
    public static class EventExtensions {

        /// <summary>
        /// Invokes an event handler safely.
        /// </summary>
        /// <param name="handler">The handler.</param>
        /// <param name="sender">The sender.</param>
        public static void SafeInvoke(this EventHandler handler, object sender) {
            handler?.Invoke(sender, null);
        }

        /// <summary>
        /// Invokes an event handler with a parameter safely.
        /// </summary>
        /// <typeparam name="T">The type parameter.</typeparam>
        /// <param name="handler">The handler.</param>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        public static void SafeInvoke<T>(this EventHandler<T> handler, object sender, T e) {
            handler?.Invoke(sender, e);
        }
    }
}