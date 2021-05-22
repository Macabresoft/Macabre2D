namespace Macabresoft.Macabre2D.Framework {
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// Helper class for lines.
    /// </summary>
    public static class LineHelper {
        /// <summary>
        /// Gets the dynamic line thickness. This scales a static line size to appear the same size regardless of the camera's view
        /// height. Essentially makes a line size of 1 equivalent to 1 pixel in width (I think?)
        /// </summary>
        /// <param name="staticLineThickness">The line thickness before being scaled to the view height.</param>
        /// <param name="viewHeight">Height of the view.</param>
        /// <param name="graphicsDevice">The graphics device.</param>
        /// <param name="settings">The settings.</param>
        /// <returns>The appropriate line thickness for this drawer.</returns>
        public static float GetDynamicLineThickness(float staticLineThickness, float viewHeight, GraphicsDevice graphicsDevice, IGameSettings settings) {
            return staticLineThickness * settings.GetPixelAgnosticRatio(viewHeight, graphicsDevice.Viewport.Height);
        }
    }
}