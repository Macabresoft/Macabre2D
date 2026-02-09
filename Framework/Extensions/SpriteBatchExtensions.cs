namespace Macabre2D.Framework;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

/// <summary>
/// Extensions for <see cref="SpriteBatch" />.
/// </summary>
public static class SpriteBatchExtensions {
    /// <param name="spriteBatch">The sprite batch.</param>
    extension(SpriteBatch spriteBatch) {
        /// <summary>
        /// Draws the specified sprite.
        /// </summary>
        /// <param name="pixelsPerUnit">The pixels per unit.</param>
        /// <param name="spriteSheet">The sprite sheet.</param>
        /// <param name="spriteIndex">The sprite index.</param>
        /// <param name="position">The position.</param>
        /// <param name="color">The color.</param>
        /// <param name="orientation">The orientation.</param>
        public void Draw(
            ushort pixelsPerUnit,
            SpriteSheet spriteSheet,
            byte spriteIndex,
            Vector2 position,
            Color color,
            SpriteEffects orientation) {
            spriteBatch.Draw(pixelsPerUnit, spriteSheet, spriteIndex, position, Vector2.One, color, orientation);
        }

        /// <summary>
        /// Draws the specified sprite.
        /// </summary>
        /// <param name="pixelsPerUnit">The pixels per unit.</param>
        /// <param name="spriteSheet">The sprite sheet.</param>
        /// <param name="spriteIndex">The sprite index.</param>
        /// <param name="position">The position.</param>
        /// <param name="scale">The scale.</param>
        /// <param name="color">The color.</param>
        /// <param name="orientation">The orientation.</param>
        public void Draw(
            ushort pixelsPerUnit,
            SpriteSheet spriteSheet,
            byte spriteIndex,
            Vector2 position,
            Vector2 scale,
            Color color,
            SpriteEffects orientation) {
            if (spriteSheet.Content != null && spriteSheet.SpriteSize != Point.Zero) {
                spriteBatch.Draw(
                    spriteSheet.Content,
                    position * pixelsPerUnit,
                    new Rectangle(spriteSheet.GetSpriteLocation(spriteIndex), spriteSheet.SpriteSize),
                    color,
                    0f,
                    Vector2.Zero,
                    scale,
                    orientation,
                    0f);
            }
        }

        /// <summary>
        /// Draws the specified <see cref="Texture2D" />.
        /// </summary>
        /// <param name="pixelsPerUnit">The pixels per unit.</param>
        /// <param name="texture">The texture.</param>
        /// <param name="position">The position.</param>
        /// <param name="scale">The scale.</param>
        /// <param name="color">The color.</param>
        /// <param name="orientation">The orientation.</param>
        public void Draw(
            ushort pixelsPerUnit,
            Texture2D? texture,
            Vector2 position,
            Vector2 scale,
            Color color,
            SpriteEffects orientation = SpriteEffects.None) {
            if (texture is { Bounds.IsEmpty: false }) {
                spriteBatch.Draw(
                    texture,
                    position * pixelsPerUnit,
                    texture.Bounds,
                    color,
                    0f,
                    Vector2.Zero,
                    scale,
                    orientation,
                    0f);
            }
        }

        /// <summary>
        /// Draws the specified <see cref="Texture2D" />.
        /// </summary>
        /// <param name="pixelsPerUnit">The pixels per unit.</param>
        /// <param name="texture">The texture.</param>
        /// <param name="position">The position.</param>
        /// <param name="color">The color.</param>
        /// <param name="orientation">The orientation.</param>
        public void Draw(
            ushort pixelsPerUnit,
            Texture2D? texture,
            Vector2 position,
            Color color,
            SpriteEffects orientation = SpriteEffects.None) {
            spriteBatch.Draw(pixelsPerUnit, texture, position, Vector2.One, color, orientation);
        }

        /// <summary>
        /// Draws the specified sprite.
        /// </summary>
        /// <param name="pixelsPerUnit">The pixels per unit.</param>
        /// <param name="spriteSheet">The sprite sheet.</param>
        /// <param name="spriteIndex">The sprite index.</param>
        /// <param name="position">The world position.</param>
        /// <param name="color">The color.</param>
        public void Draw(
            ushort pixelsPerUnit,
            SpriteSheet spriteSheet,
            byte spriteIndex,
            Vector2 position,
            Color color) {
            spriteBatch.Draw(pixelsPerUnit, spriteSheet, spriteIndex, position, color, SpriteEffects.FlipVertically);
        }

        /// <summary>
        /// Draws the specified sprite.
        /// </summary>
        /// <param name="pixelsPerUnit">The pixels per unit.</param>
        /// <param name="spriteSheet">The sprite sheet.</param>
        /// <param name="spriteIndex">The sprite index.</param>
        /// <param name="position">The position.</param>
        /// <param name="scale">The scale.</param>
        /// <param name="color">The color.</param>
        public void Draw(
            ushort pixelsPerUnit,
            SpriteSheet spriteSheet,
            byte spriteIndex,
            Vector2 position,
            Vector2 scale,
            Color color) {
            spriteBatch.Draw(pixelsPerUnit, spriteSheet, spriteIndex, position, scale, color, SpriteEffects.FlipVertically);
        }

        /// <summary>
        /// Draws the text with the provided font.
        /// </summary>
        /// <param name="pixelsPerUnit">The pixels per unit.</param>
        /// <param name="font">The font.</param>
        /// <param name="text">The text.</param>
        /// <param name="position">The position.</param>
        /// <param name="scale">The scale.</param>
        /// <param name="color">The color.</param>
        /// <param name="orientation">The orientation.</param>
        public void Draw(
            ushort pixelsPerUnit,
            FontAsset font,
            string text,
            Vector2 position,
            Vector2 scale,
            Color color,
            SpriteEffects orientation) {
            if (font.Content is { } spriteFont) {
                spriteBatch.DrawString(
                    spriteFont,
                    text,
                    position * pixelsPerUnit,
                    color,
                    0f,
                    Vector2.Zero,
                    scale,
                    orientation,
                    0f);
            }
        }

        /// <summary>
        /// Draws the text with the provided font.
        /// </summary>
        /// <param name="pixelsPerUnit">The pixels per unit.</param>
        /// <param name="font">The font.</param>
        /// <param name="text">The text.</param>
        /// <param name="position">The world transform.</param>
        /// <param name="color">The color.</param>
        /// <param name="orientation">The orientation.</param>
        public void Draw(
            ushort pixelsPerUnit,
            FontAsset font,
            string text,
            Vector2 position,
            Color color,
            SpriteEffects orientation) {
            spriteBatch.Draw(pixelsPerUnit, font, text, position, Vector2.One, color, orientation);
        }

        /// <summary>
        /// Draws the text with the provided font.
        /// </summary>
        /// <param name="pixelsPerUnit">The pixels per unit.</param>
        /// <param name="font">The font.</param>
        /// <param name="text">The text.</param>
        /// <param name="position">The world transform.</param>
        /// <param name="color">The color.</param>
        public void Draw(
            ushort pixelsPerUnit,
            FontAsset font,
            string text,
            Vector2 position,
            Color color) {
            spriteBatch.Draw(pixelsPerUnit, font, text, position, Vector2.One, color, SpriteEffects.FlipVertically);
        }
    }
}