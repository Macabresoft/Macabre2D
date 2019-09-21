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
        /// <param name="rotationAngle">The rotation angle.</param>
        /// <param name="color">The color.</param>
        public static void Draw(this SpriteBatch spriteBatch, Sprite sprite, Transform transform, float rotationAngle, Color color) {
            if (sprite?.Texture != null && transform != null) {
                spriteBatch.Draw(
                    sprite.Texture,
                    transform.Position * GameSettings.Instance.PixelsPerUnit,
                    new Rectangle(sprite.Location, sprite.Size),
                    color,
                    rotationAngle,
                    Vector2.Zero,
                    transform.Scale,
                    SpriteEffects.FlipVertically,
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
        /// <param name="rotationAngle">The rotation angle.</param>
        /// <param name="color">The color.</param>
        public static void Draw(this SpriteBatch spriteBatch, Sprite sprite, Vector2 position, Vector2 scale, float rotationAngle, Color color) {
            if (sprite?.Texture != null) {
                spriteBatch.Draw(
                    sprite.Texture,
                    position * GameSettings.Instance.PixelsPerUnit,
                    new Rectangle(sprite.Location, sprite.Size),
                    color,
                    rotationAngle,
                    Vector2.Zero,
                    scale,
                    SpriteEffects.FlipVertically,
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
            spriteBatch.Draw(sprite, position, scale, 0f, color);
        }

        /// <summary>
        /// Draws the specified sprite.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch.</param>
        /// <param name="sprite">The sprite.</param>
        /// <param name="transform">The transform.</param>
        /// <param name="color">The color.</param>
        public static void Draw(this SpriteBatch spriteBatch, Sprite sprite, RotatableTransform transform, Color color) {
            spriteBatch.Draw(sprite, transform, transform.Rotation.Angle, color);
        }

        /// <summary>
        /// Draws the specified sprite.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch.</param>
        /// <param name="sprite">The sprite.</param>
        /// <param name="transform">The transform.</param>
        /// <param name="color">The color.</param>
        public static void Draw(this SpriteBatch spriteBatch, Sprite sprite, Transform transform, Color color) {
            spriteBatch.Draw(sprite, transform, 0f, color);
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
        public static void Draw(this SpriteBatch spriteBatch, Font font, string text, Vector2 position, Vector2 scale, float rotationAngle, Color color) {
            if (font?.SpriteFont != null) {
                spriteBatch.DrawString(
                  font.SpriteFont,
                  text,
                  position * GameSettings.Instance.PixelsPerUnit,
                  color,
                  rotationAngle,
                  Vector2.Zero,
                  scale,
                  SpriteEffects.FlipVertically,
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
        public static void Draw(this SpriteBatch spriteBatch, Font font, string text, RotatableTransform rotatableTransform, Color color) {
            spriteBatch.Draw(font, text, rotatableTransform.Position, rotatableTransform.Scale, rotatableTransform.Rotation.Angle, color);
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
            spriteBatch.Draw(font, text, transform.Position, transform.Scale, 0f, color);
        }
    }
}