namespace Macabre2D.Framework {

    using Microsoft.Xna.Framework;
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// An interface for a game component that can update on a background thread.
    /// </summary>
    public interface IUpdateableComponentAsync : IBaseComponent {

        /// <summary>
        /// Occurs when [enabled changed].
        /// </summary>
        event EventHandler IsEnabledChanged;

        /// <summary>
        /// Gets a value indicating whether this <see cref="IUpdateableComponentAsync"/> is enabled.
        /// </summary>
        /// <value><c>true</c> if enabled; otherwise, <c>false</c>.</value>
        bool IsEnabled { get; }

        /// <summary>
        /// Updates this instance asynchronously.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        /// <returns>The task.</returns>
        Task UpdateAsync(GameTime gameTime);
    }
}