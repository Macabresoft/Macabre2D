namespace Macabre2D.Framework {

    using System;

    /// <summary>
    /// Interface for a component that can be enabled.
    /// </summary>
    public interface IEnableableComponent {

        /// <summary>
        /// Occurs when [enabled changed].
        /// </summary>
        event EventHandler IsEnabledChanged;

        /// <summary>
        /// Occurs when [update order changed].
        /// </summary>
        event EventHandler UpdateOrderChanged;

        /// <summary>
        /// Gets a value indicating whether this <see cref="IUpdateableComponent"/> is enabled.
        /// </summary>
        /// <value><c>true</c> if enabled; otherwise, <c>false</c>.</value>
        bool IsEnabled { get; }

        /// <summary>
        /// Gets the update order.
        /// </summary>
        /// <value>The update order.</value>
        int UpdateOrder { get; }
    }
}