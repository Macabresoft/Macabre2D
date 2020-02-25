namespace Macabre2D.Framework {

    using Microsoft.Xna.Framework;
    using System;

    /// <summary>
    /// Represents a module that can be updated before and after all components update.
    /// </summary>
    public interface IUpdateableModule {

        /// <summary>
        /// Occurs when this instance becomes enabled or disabled.
        /// </summary>
        event EventHandler IsEnabledChanged;

        /// <summary>
        /// Occurs when the update order has changed.
        /// </summary>
        event EventHandler UpdateOrderChanged;

        /// <summary>
        /// Gets or sets a value indicating whether this instance is enabled.
        /// </summary>
        /// <value><c>true</c> if this instance is enabled; otherwise, <c>false</c>.</value>
        bool IsEnabled { get; }

        /// <summary>
        /// Gets the update order.
        /// </summary>
        /// <value>The update order.</value>
        int UpdateOrder { get; }

        /// <summary>
        /// Updates after the normal update occurs for a scene.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        void PostUpdate(GameTime gameTime);

        /// <summary>
        /// Updates before the normal update for a scene.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        void PreUpdate(GameTime gameTime);
    }
}