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
        /// Creates the arrow sprite. This will be pointing upwards, so rotate appropriately.
        /// </summary>
        /// <remarks>This call will make all the pixels of the sprite white.</remarks>
        /// <param name="graphicsDevice">The graphics device.</param>
        /// <param name="size">The size.</param>
        /// <returns>The arrow sprite.</returns>
        public static Sprite CreateArrowSprite(GraphicsDevice graphicsDevice, int size) {
            return PrimitiveDrawer.CreateArrowSprite(graphicsDevice, size, Color.White);
        }

        /// <summary>
        /// Creates the arrow sprite. This will be pointing upwards, so rotate appropriately.
        /// </summary>
        /// <param name="graphicsDevice">The graphics device.</param>
        /// <param name="size">The size.</param>
        /// <param name="color">The color.</param>
        /// <returns>The arrow sprite.</returns>
        public static Sprite CreateArrowSprite(GraphicsDevice graphicsDevice, int size, Color color) {
            var texture = new Texture2D(graphicsDevice, size, size);
            var pixels = new Color[size * size];

            var index = 0;
            for (var y = size - 1; y >= 0; y--) {
                var buffer = (size - (size - y)) / 2;
                var fill = size - (buffer * 2);

                var difference = size - (buffer * 2 + fill);
                fill += difference;

                for (var x = 0; x < buffer; x++) {
                    pixels[index] = Color.Transparent;
                    index++;
                }

                for (var x = 0; x < fill; x++) {
                    pixels[index] = color;
                    index++;
                }

                for (var x = 0; x < buffer; x++) {
                    pixels[index] = Color.Transparent;
                    index++;
                }
            }

            texture.SetData(pixels);
            return new Sprite(texture);
        }

        /// <summary>
        /// Creates the quad sprite.
        /// </summary>
        /// <remarks>This call will make all the pixels of the sprite white.</remarks>
        /// <param name="graphicsDevice">The graphics device.</param>
        /// <param name="size">The size.</param>
        /// <param name="color">The color.</param>
        /// <returns>The quad sprite.</returns>
        public static Sprite CreateQuadSprite(GraphicsDevice graphicsDevice, Point size) {
            return PrimitiveDrawer.CreateQuadSprite(graphicsDevice, size, Color.White);
        }

        /// <summary>
        /// Creates the quad sprite.
        /// </summary>
        /// <param name="graphicsDevice">The graphics device.</param>
        /// <param name="size">The size.</param>
        /// <param name="color">The color.</param>
        /// <returns>The quad sprite.</returns>
        public static Sprite CreateQuadSprite(GraphicsDevice graphicsDevice, Point size, Color color) {
            var texture = new Texture2D(graphicsDevice, size.X, size.Y);
            var pixels = new Color[size.X * size.Y];
            pixels.Populate(color);
            texture.SetData(pixels);
            return new Sprite(texture);
        }

        /// <summary>
        /// Creates the right triangle sprite. The right angle will be in the top left corner of the sprite.
        /// </summary>
        /// <param name="graphicsDevice">The graphics device.</param>
        /// <param name="size">The size.</param>
        /// <returns>The right triangle sprite.</returns>
        public static Sprite CreateRightTriangleSprite(GraphicsDevice graphicsDevice, Point size) {
            return PrimitiveDrawer.CreateRightTriangleSprite(graphicsDevice, size, Color.White);
        }

        /// <summary>
        /// Creates the right triangle sprite. The right angle will be in the top left corner of the sprite.
        /// </summary>
        /// <param name="graphicsDevice">The graphics device.</param>
        /// <param name="size">The size.</param>
        /// <param name="color">The color.</param>
        /// <returns>The right triangle sprite.</returns>
        public static Sprite CreateRightTriangleSprite(GraphicsDevice graphicsDevice, Point size, Color color) {
            var texture = new Texture2D(graphicsDevice, size.X, size.Y);
            var pixels = new Color[size.X * size.Y];

            var counter = 0;
            for (var y = 0; y < size.Y; y++) {
                var xOffset = Convert.ToInt32(size.X * ((y + 1f) / size.Y));

                for (var x = 0; x < size.X - xOffset; x++) {
                    pixels[counter] = color;
                    counter++;
                }

                for (var x = 0; x < xOffset; x++) {
                    pixels[counter] = Color.Transparent;
                    counter++;
                }
            }

            texture.SetData(pixels);
            return new Sprite(texture);
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