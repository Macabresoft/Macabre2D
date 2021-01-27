namespace Macabresoft.Macabre2D.Framework {

    using Macabresoft.Core;
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

        /// <summary>
        /// Initializes a new instance of the <see cref="PrimitiveDrawer" /> class.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch.</param>
        /// <param name="gameSettings">The game settings.</param>
        public PrimitiveDrawer(SpriteBatch spriteBatch) {
            this._pixel = new Texture2D(spriteBatch.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            this._pixel.SetData(new[] { Color.White });
        }

        /// <summary>
        /// Creates the circle sprite.
        /// </summary>
        /// <param name="graphicsDevice">The graphics device.</param>
        /// <param name="size">The size.</param>
        /// <returns>A filled circle sprite.</returns>
        public static Texture2D CreateCircleSprite(GraphicsDevice graphicsDevice, int size) {
            return PrimitiveDrawer.CreateCircleSprite(graphicsDevice, size, Color.White);
        }

        /// <summary>
        /// Creates the circle sprite.
        /// </summary>
        /// <param name="graphicsDevice">The graphics device.</param>
        /// <param name="size">The size.</param>
        /// <param name="color">The color.</param>
        /// <returns>A filled circle sprite.</returns>
        public static Texture2D CreateCircleSprite(GraphicsDevice graphicsDevice, int size, Color color) {
            var texture = new Texture2D(graphicsDevice, size, size);
            var pixels = new Color[size * size];
            var radius = size / 2;
            var center = new Vector2(radius - 1);

            var counter = 0;
            for (var x = 0; x < size; x++) {
                for (var y = 0; y < size; y++) {
                    if (Vector2.Distance(new Vector2(x, y), center) < radius) {
                        pixels[counter] = color;
                    }
                    else {
                        pixels[counter] = Color.Transparent;
                    }

                    counter++;
                }
            }

            texture.SetData(pixels);
            return texture;
        }

        /// <summary>
        /// Creates the arrow sprite. This will be pointing forwards (to the right by default).
        /// </summary>
        /// <param name="graphicsDevice">The graphics device.</param>
        /// <param name="size">The size.</param>
        /// <param name="color">The color.</param>
        /// <returns>The arrow sprite.</returns>
        public static Texture2D CreateForwardArrowSprite(GraphicsDevice graphicsDevice, int size) {
            return PrimitiveDrawer.CreateForwardArrowSprite(graphicsDevice, size, Color.White);
        }

        /// <summary>
        /// Creates the arrow sprite. This will be pointing forwards (to the right by default).
        /// </summary>
        /// <param name="graphicsDevice">The graphics device.</param>
        /// <param name="size">The size.</param>
        /// <param name="color">The color.</param>
        /// <returns>The arrow sprite.</returns>
        public static Texture2D CreateForwardArrowSprite(GraphicsDevice graphicsDevice, int size, Color color) {
            var texture = new Texture2D(graphicsDevice, size, size);
            var pixels = new Color[size * size];

            var index = 0;
            for (var y = size - 1; y >= 0; y--) {
                var buffer = Math.Abs(size - (y * 2f));
                var fill = size - buffer;

                //var difference = size - (buffer * 2 + fill);
                //fill += difference;

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
            return texture;
        }

        /// <summary>
        /// Creates the quad sprite.
        /// </summary>
        /// <remarks>This call will make all the pixels of the sprite white.</remarks>
        /// <param name="graphicsDevice">The graphics device.</param>
        /// <param name="size">The size.</param>
        /// <param name="color">The color.</param>
        /// <returns>The quad sprite.</returns>
        public static Texture2D CreateQuadSprite(GraphicsDevice graphicsDevice, Point size) {
            return PrimitiveDrawer.CreateQuadSprite(graphicsDevice, size, Color.White);
        }

        /// <summary>
        /// Creates the quad sprite.
        /// </summary>
        /// <param name="graphicsDevice">The graphics device.</param>
        /// <param name="size">The size.</param>
        /// <param name="color">The color.</param>
        /// <returns>The quad sprite.</returns>
        public static Texture2D CreateQuadSprite(GraphicsDevice graphicsDevice, Point size, Color color) {
            var texture = new Texture2D(graphicsDevice, size.X, size.Y);
            var pixels = new Color[size.X * size.Y];
            pixels.Populate(color);
            texture.SetData(pixels);
            return texture;
        }

        /// <summary>
        /// Creates the right triangle sprite. The right angle will be in the top left corner of the sprite.
        /// </summary>
        /// <param name="graphicsDevice">The graphics device.</param>
        /// <param name="size">The size.</param>
        /// <returns>The right triangle sprite.</returns>
        public static Texture2D CreateTopLeftRightTriangleSprite(GraphicsDevice graphicsDevice, Point size) {
            return PrimitiveDrawer.CreateTopLeftRightTriangleSprite(graphicsDevice, size, Color.White);
        }

        /// <summary>
        /// Creates the right triangle sprite. The right angle will be in the top left corner of the sprite.
        /// </summary>
        /// <param name="graphicsDevice">The graphics device.</param>
        /// <param name="size">The size.</param>
        /// <param name="color">The color.</param>
        /// <returns>The right triangle sprite.</returns>
        public static Texture2D CreateTopLeftRightTriangleSprite(GraphicsDevice graphicsDevice, Point size, Color color) {
            var texture = new Texture2D(graphicsDevice, size.X, size.Y);
            var pixels = new Color[size.X * size.Y];

            var counter = 0;
            for (var y = 0; y < size.Y; y++) {
                var xOffset = Convert.ToInt32(size.X * ((y + 1f) / size.Y));

                for (var x = 0; x <= size.X - xOffset; x++) {
                    pixels[counter] = color;
                    counter++;
                }

                for (var x = 0; x < xOffset - 1; x++) {
                    pixels[counter] = Color.Transparent;
                    counter++;
                }
            }

            texture.SetData(pixels);
            return texture;
        }

        /// <summary>
        /// Creates the arrow sprite. This will be pointing upwards, so rotate appropriately.
        /// </summary>
        /// <remarks>This call will make all the pixels of the sprite white.</remarks>
        /// <param name="graphicsDevice">The graphics device.</param>
        /// <param name="size">The size.</param>
        /// <returns>The arrow sprite.</returns>
        public static Texture2D CreateUpwardsArrowSprite(GraphicsDevice graphicsDevice, int size) {
            return PrimitiveDrawer.CreateUpwardsArrowSprite(graphicsDevice, size, Color.White);
        }

        /// <summary>
        /// Creates the arrow sprite. This will be pointing upwards.
        /// </summary>
        /// <param name="graphicsDevice">The graphics device.</param>
        /// <param name="size">The size.</param>
        /// <param name="color">The color.</param>
        /// <returns>The arrow sprite.</returns>
        public static Texture2D CreateUpwardsArrowSprite(GraphicsDevice graphicsDevice, int size, Color color) {
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
            return texture;
        }

        /// <summary>
        /// Draws the circle.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch.</param>
        /// <param name="pixelsPerUnit">The pixels per unit.</param>
        /// <param name="radius">The radius.</param>
        /// <param name="center">The center.</param>
        /// <param name="complexity">The complexity.</param>
        /// <param name="color">The color.</param>
        /// <param name="thickness">The thickness.</param>
        public void DrawCircle(SpriteBatch spriteBatch, ushort pixelsPerUnit, float radius, Vector2 center, int complexity, Color color, float thickness) {
            var points = new Vector2[complexity];
            var step = MathHelper.TwoPi / complexity;
            var theta = 0f;

            for (var i = 0; i < complexity; i++) {
                var x = (float)(radius * Math.Cos(theta));
                var y = (float)(radius * Math.Sin(theta));
                points[i] = new Vector2(x, y) + center;
                theta += step;
            }

            this.DrawPolygon(spriteBatch, pixelsPerUnit, color, thickness, points);
        }

        /// <summary>
        /// Draws a line given two points.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch.</param>
        /// <param name="pixelsPerUnit">The pixels per unit.</param>
        /// <param name="point1">The point1.</param>
        /// <param name="point2">The point2.</param>
        /// <param name="color">The color.</param>
        /// <param name="thickness">The thickness.</param>
        public void DrawLine(SpriteBatch spriteBatch, ushort pixelsPerUnit, Vector2 point1, Vector2 point2, Color color, float thickness) {
            var length = Vector2.Distance(point1, point2);
            var angle = (float)Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);
            spriteBatch.Draw(
                this._pixel,
                point1 * pixelsPerUnit,
                null,
                color,
                angle,
                Vector2.Zero,
                new Vector2(length * pixelsPerUnit, thickness),
                SpriteEffects.None,
                0);
        }

        /// <summary>
        /// Draws a line strip given a series of points.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch.</param>
        /// <param name="pixelsPerUnit">The pixels per unit.</param>
        /// <param name="color">The color.</param>
        /// <param name="thickness">The thickness.</param>
        /// <param name="points">The points.</param>
        /// <exception cref="NotSupportedException">
        /// A line strip must contain at least two points.
        /// </exception>
        public void DrawLineStrip(SpriteBatch spriteBatch, ushort pixelsPerUnit, Color color, float thickness, params Vector2[] points) {
            if (points.Length < 2) {
                throw new NotSupportedException("A line strip must contain at least two points.");
            }

            for (var i = 1; i < points.Length; i++) {
                this.DrawLine(spriteBatch, pixelsPerUnit, points[i - 1], points[i], color, thickness);
            }
        }

        /// <summary>
        /// Draws a polygon given a series of points. The first and last point will connect.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch.</param>
        /// <param name="pixelsPerUnit">The pixels per unit.</param>
        /// <param name="color">The color.</param>
        /// <param name="thickness">The thickness.</param>
        /// <param name="points">The points.</param>
        public void DrawPolygon(SpriteBatch spriteBatch, ushort pixelsPerUnit, Color color, float thickness, IEnumerable<Vector2> points) {
            if (points.Count() < 3) {
                throw new NotSupportedException("A polygon must contain at least three points.");
            }

            var previousPoint = points.LastOrDefault();

            foreach (var point in points) {
                this.DrawLine(spriteBatch, pixelsPerUnit, previousPoint, point, color, thickness);
                previousPoint = point;
            }
        }
    }
}