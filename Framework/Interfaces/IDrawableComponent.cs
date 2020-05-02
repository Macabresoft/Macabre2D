namespace Macabre2D.Framework {

    /// <summary>
    /// Drawable component.
    /// </summary>
    public interface IDrawableComponent : IBaseComponent, IBoundable {

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
        /// <param name="frameTime">The frame time.</param>
        /// <param name="viewBoundingArea">The view bounding area.</param>
        void Draw(FrameTime frameTime, BoundingArea viewBoundingArea);
    }
}