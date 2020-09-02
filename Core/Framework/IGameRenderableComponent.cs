namespace Macabresoft.MonoGame.Core {

    /// <summary>
    /// Interface for a component which can be rendered.
    /// </summary>
    public interface IGameRenderableComponent : IBoundable, IGameComponent {

        /// <summary>
        /// Gets a value indicating whether this instance is visible.
        /// </summary>
        /// <value><c>true</c> if this instance is visible; otherwise, <c>false</c>.</value>
        bool IsVisible { get; }

        /// <summary>
        /// Gets the render order.
        /// </summary>
        /// <value>The render order.</value>
        int RenderOrder { get; }

        /// <summary>
        /// Renders this instance.
        /// </summary>
        /// <param name="frameTime">The frame time.</param>
        /// <param name="viewBoundingArea">The view bounding area.</param>
        void Render(FrameTime frameTime, BoundingArea viewBoundingArea);
    }
}