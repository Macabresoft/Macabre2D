namespace Macabre2D.Framework {

    using Microsoft.Xna.Framework;
    using System;

    /// <summary>
    /// Drawable component.
    /// </summary>
    public interface IDrawableComponent : IBaseComponent, IBoundable {

        /// <summary>
        /// Occurs when [draw order changed].
        /// </summary>
        event EventHandler DrawOrderChanged;

        /// <summary>
        /// Occurs when [visible changed].
        /// </summary>
        event EventHandler IsVisibleChanged;

        /// <summary>
        /// Gets the draw order.
        /// </summary>
        /// <value>The draw order.</value>
        int DrawOrder { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is visible.
        /// </summary>
        /// <value><c>true</c> if this instance is visible; otherwise, <c>false</c>.</value>
        bool IsVisible { get; }

        /// <summary>
        /// Draws this instance.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        /// <param name="viewHeight">Height of the view for the rendering camera.</param>
        void Draw(GameTime gameTime, float viewHeight);
    }
}