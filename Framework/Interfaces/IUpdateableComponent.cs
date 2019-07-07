namespace Macabre2D.Framework {

    using Microsoft.Xna.Framework;
    using System;

    /// <summary>
    /// An updateable component.
    /// </summary>
    public interface IUpdateableComponent : IBaseComponent {

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

        /// <summary>
        /// Updates this instance.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        void Update(GameTime gameTime);
    }
}