namespace Macabre2D.Framework {

    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// Extensions for <see cref="SpriteBatch"/>.
    /// </summary>
    public static class SpriteBatchExtensions {

        /// <summary>
        /// Draws the specified sprite.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch.</param>
        /// <param name="sprite">The sprite.</param>
        /// <param name="transform">The transform.</param>
        /// <param name="rotation">The rotation.</param>
        /// <param name="color">The color.</param>
        /// <param name="orientation">The orientation.</param>
        public static void Draw(this SpriteBatch spriteBatch, Sprite sprite, Transform transform, float rotation, Color color, SpriteEffects orientation) {
            if (sprite?.Texture != null && transform != null) {
                spriteBatch.Draw(
                    sprite.Texture,
                    transform.Position * GameSettings.Instance.PixelsPerUnit,
                    new Rectangle(sprite.Location, sprite.Size),
                    color,
                    rotation,
                    Vector2.Zero,
                    transform.Scale,
                    orientation,
                    0f);
            }
        }

        /// <summary>
        /// Draws the specified sprite.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch.</param>
        /// <param name="sprite">The sprite.</param>
        /// <param name="position">The position.</param>
        /// <param name="scale">The scale.</param>
        /// <param name="rotation">The rotation.</param>
        /// <param name="color">The color.</param>
        /// <param name="orientation">The orientation.</param>
        public static void Draw(this SpriteBatch spriteBatch, Sprite sprite, Vector2 position, Vector2 scale, float rotation, Color color, SpriteEffects orientation) {
            if (sprite?.Texture != null) {
                spriteBatch.Draw(
                    sprite.Texture,
                    position * GameSettings.Instance.PixelsPerUnit,
                    new Rectangle(sprite.Location, sprite.Size),
                    color,
                    rotation,
                    Vector2.Zero,
                    scale,
                    orientation,
                    0f);
            }
        }

        /// <summary>
        /// Draws the specified sprite.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch.</param>
        /// <param name="sprite">The sprite.</param>
        /// <param name="position">The position.</param>
        /// <param name="scale">The scale.</param>
        /// <param name="color">The color.</param>
        public static void Draw(this SpriteBatch spriteBatch, Sprite sprite, Vector2 position, Vector2 scale, Color color) {
            spriteBatch.Draw(sprite, position, scale, 0f, color, SpriteEffects.FlipVertically);
        }

        /// <summary>
        /// Draws the specified sprite.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch.</param>
        /// <param name="sprite">The sprite.</param>
        /// <param name="transform">The transform.</param>
        /// <param name="color">The color.</param>
        public static void Draw(this SpriteBatch spriteBatch, Sprite sprite, Transform transform, Color color) {
            spriteBatch.Draw(sprite, transform, transform.Rotation, color, SpriteEffects.FlipVertically);
        }

        /// <summary>
        /// Draws the specified sprite.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch.</param>
        /// <param name="sprite">The sprite.</param>
        /// <param name="transform">The transform.</param>
        /// <param name="color">The color.</param>
        /// <param name="orientation">The orientation.</param>
        public static void Draw(this SpriteBatch spriteBatch, Sprite sprite, Transform transform, Color color, SpriteEffects orientation) {
            spriteBatch.Draw(sprite, transform, transform.Rotation, color, orientation);
        }

        /// <summary>
        /// Draws the text with the provided font.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch.</param>
        /// <param name="sprite">The sprite.</param>
        /// <param name="position">The position.</param>
        /// <param name="scale">The scale.</param>
        /// <param name="rotationAngle">The rotation angle.</param>
        /// <param name="color">The color.</param>
        public static void Draw(this SpriteBatch spriteBatch, Font font, string text, Vector2 position, Vector2 scale, float rotationAngle, Color color, SpriteEffects orientation) {
            if (font?.SpriteFont != null) {
                spriteBatch.DrawString(
                  font.SpriteFont,
                  text,
                  position * GameSettings.Instance.PixelsPerUnit,
                  color,
                  rotationAngle,
                  Vector2.Zero,
                  scale,
                  orientation,
                  0f);
            }
        }

        /// <summary>
        /// Draws the text with the provided font.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch.</param>
        /// <param name="font">The font.</param>
        /// <param name="text">The text.</param>
        /// <param name="transform">The transform.</param>
        /// <param name="color">The color.</param>
        /// <param name="orientation">The orientation.</param>
        public static void Draw(this SpriteBatch spriteBatch, Font font, string text, Transform transform, Color color, SpriteEffects orientation) {
            spriteBatch.Draw(font, text, transform.Position, transform.Scale, transform.Rotation, color, orientation);
        }

        /// <summary>
        /// Draws the text with the provided font.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch.</param>
        /// <param name="font">The font.</param>
        /// <param name="text">The text.</param>
        /// <param name="transform">The transform.</param>
        /// <param name="color">The color.</param>
        public static void Draw(this SpriteBatch spriteBatch, Font font, string text, Transform transform, Color color) {
            spriteBatch.Draw(font, text, transform.Position, transform.Scale, transform.Rotation, color, SpriteEffects.FlipVertically);
        }
    }
}