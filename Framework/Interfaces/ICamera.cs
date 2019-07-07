namespace Macabre2D.Framework {

    using Microsoft.Xna.Framework;
    using System;

    /// <summary>
    /// Generic interface for cameras. Provides everything that the engine technically needs to use a camera.
    /// </summary>
    public interface ICamera : IBoundable {

        /// <summary>
        /// Occurs when [enabled changed].
        /// </summary>
        event EventHandler IsEnabledChanged;

        /// <summary>
        /// Occurs when [render order changed].
        /// </summary>
        event EventHandler RenderOrderChanged;

        /// <summary>
        /// Occurs when [view height changed].
        /// </summary>
        event EventHandler ViewHeightChanged;

        /// <summary>
        /// Gets a value indicating whether this <see cref="ICamera"/> is enabled.
        /// </summary>
        /// <value><c>true</c> if enabled; otherwise, <c>false</c>.</value>
        bool IsEnabled { get; }

        /// <summary>
        /// Gets the layers to render.
        /// </summary>
        /// <value>The layers to render.</value>
        Layers LayersToRender { get; }

        /// <summary>
        /// Gets the render order. A lower number will be rendered first.
        /// </summary>
        /// <value>The render order.</value>
        int RenderOrder { get; }

        /// <summary>
        /// Gets the height of the view.
        /// </summary>
        /// <value>The height of the view.</value>
        float ViewHeight { get; }

        /// <summary>
        /// Gets the view matrix.
        /// </summary>
        /// <value>The view matrix.</value>
        Matrix ViewMatrix { get; }

        /// <summary>
        /// Converts the point from screen space to world space.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns>The world space location of the point.</returns>
        Vector2 ConvertPointFromScreenSpaceToWorldSpace(Point point);

        /// <summary>
        /// Zooms to a world position.
        /// </summary>
        /// <param name="worldPosition">The world position.</param>
        /// <param name="zoomAmount">The zoom amount.</param>
        void ZoomTo(Vector2 worldPosition, float zoomAmount);

        /// <summary>
        /// Zooms to a screen position.
        /// </summary>
        /// <param name="screenPosition">The screen position.</param>
        /// <param name="zoomAmount">The zoom amount.</param>
        void ZoomTo(Point screenPosition, float zoomAmount);

        /// <summary>
        /// Zooms to a boundable component, fitting it into frame.
        /// </summary>
        /// <param name="boundable">The boundable.</param>
        void ZoomTo(IBoundable boundable);
    }
}