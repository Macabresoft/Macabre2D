namespace Macabre2D.Framework.Diagnostics {

    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Draws primitive objects.
    /// </summary>
    public sealed class PrimitiveDrawer {
        private readonly Texture2D _pixel;
        private readonly GameSettings _settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="PrimitiveDrawer"/> class.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch.</param>
        /// <param name="gameSettings">The game settings.</param>
        public PrimitiveDrawer(SpriteBatch spriteBatch, GameSettings gameSettings) {
            this._pixel = new Texture2D(spriteBatch.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            this._pixel.SetData(new[] { Color.White });
            this._settings = gameSettings;
        }

        /// <summary>
        /// Draws the circle.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch.</param>
        /// <param name="radius">The radius.</param>
        /// <param name="center">The center.</param>
        /// <param name="complexity">The complexity.</param>
        /// <param name="color">The color.</param>
        /// <param name="thickness">The thickness.</param>
        public void DrawCircle(SpriteBatch spriteBatch, float radius, Vector2 center, int complexity, Color color, float thickness) {
            var points = new Vector2[complexity];
            var step = MathHelper.TwoPi / complexity;
            var theta = 0f;

            for (var i = 0; i < complexity; i++) {
                var x = (float)(radius * Math.Cos(theta));
                var y = (float)(radius * Math.Sin(theta));
                points[i] = new Vector2(x, y) + center;
                theta += step;
            }

            this.DrawPolygon(spriteBatch, color, thickness, points);
        }

        /// <summary>
        /// Draws a line given two points.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch.</param>
        /// <param name="point1">The point1.</param>
        /// <param name="point2">The point2.</param>
        /// <param name="color">The color.</param>
        /// <param name="thickness">The thickness.</param>
        public void DrawLine(SpriteBatch spriteBatch, Vector2 point1, Vector2 point2, Color color, float thickness) {
            var length = Vector2.Distance(point1, point2);
            var angle = (float)Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);
            spriteBatch.Draw(this._pixel, point1 * this._settings.PixelsPerUnit, null, color, angle, Vector2.Zero, new Vector2(length * this._settings.PixelsPerUnit, thickness), SpriteEffects.None, 0);
        }

        /// <summary>
        /// Draws a line strip given a series of points.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch.</param>
        /// <param name="color">The color.</param>
        /// <param name="thickness">The thickness.</param>
        /// <param name="points">The points.</param>
        /// <exception cref="NotSupportedException">
        /// A line strip must contain at least two points.
        /// </exception>
        public void DrawLineStrip(SpriteBatch spriteBatch, Color color, float thickness, params Vector2[] points) {
            if (points.Length < 2) {
                throw new NotSupportedException("A line strip must contain at least two points.");
            }

            for (var i = 1; i < points.Length; i++) {
                this.DrawLine(spriteBatch, points[i - 1], points[i], color, thickness);
            }
        }

        /// <summary>
        /// Draws a polygon given a series of points. The first and last point will connect.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch.</param>
        /// <param name="color">The color.</param>
        /// <param name="thickness">The thickness.</param>
        /// <param name="points">The points.</param>
        public void DrawPolygon(SpriteBatch spriteBatch, Color color, float thickness, IEnumerable<Vector2> points) {
            if (points.Count() < 3) {
                throw new NotSupportedException("A polygon must contain at least three points.");
            }

            var previousPoint = points.LastOrDefault();

            foreach (var point in points) {
                this.DrawLine(spriteBatch, previousPoint, point, color, thickness);
                previousPoint = point;
            }
        }
    }
}