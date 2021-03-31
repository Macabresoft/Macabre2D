namespace Macabresoft.Macabre2D.Framework {
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Runtime.Serialization;
    using System.Text;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content.Pipeline;
    using Microsoft.Xna.Framework.Content.Pipeline.Processors;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// A sprite sheet tied to a single <see cref="Texture2D" /> which also defines sprites, animations, and tile sets.
    /// </summary>
    [DataContract]
    public class SpriteSheet : AssetPackage<Texture2D> {
        /// <summary>
        /// The valid file extensions for a <see cref="Texture2D" />.
        /// </summary>
        public static readonly string[] ValidFileExtensions = { ".jpg", ".png" };

        private readonly Dictionary<byte, Point> _spriteIndexToLocation = new();

        [DataMember]
        private ObservableCollection<AutoTileSet> _autoTileSets = new();

        private byte _columns = 1;
        private byte _rows = 1;

        [DataMember]
        private ObservableCollection<SpriteAnimation> _spriteAnimations = new();

        private Point _spriteSize;

        /// <summary>
        /// Gets or sets the number of columns in this sprite sheet.
        /// </summary>
        [DataMember]
        public byte Columns {
            get => this._columns;
            set {
                if (value == 0) {
                    value = 1;
                }

                if (this.Set(ref this._columns, value)) {
                    this._spriteIndexToLocation.Clear();
                    this.ResetColumnWidth();
                }
            }
        }

        /// <summary>
        /// Gets or sets the number of rows in this sprite sheet.
        /// </summary>
        [DataMember]
        public byte Rows {
            get => this._rows;
            set {
                if (value == 0) {
                    value = 1;
                }

                if (this.Set(ref this._rows, value)) {
                    this._spriteIndexToLocation.Clear();
                    this.ResetColumnWidth();
                }
            }
        }

        /// <summary>
        /// Gets the size of a single sprite on this sprite sheet.
        /// </summary>
        public Point SpriteSize {
            get => this._spriteSize;
            private set => this.Set(ref this._spriteSize, value);
        }

        /// <summary>
        /// Draws the specified sprite.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch.</param>
        /// <param name="pixelsPerUnit">The pixels per unit.</param>
        /// <param name="spriteIndex">The sprite index.</param>
        /// <param name="position">The position.</param>
        /// <param name="scale">The scale.</param>
        /// <param name="rotation">The rotation.</param>
        /// <param name="color">The color.</param>
        /// <param name="orientation">The orientation.</param>
        public void Draw(
            SpriteBatch spriteBatch,
            ushort pixelsPerUnit,
            byte spriteIndex,
            Vector2 position,
            Vector2 scale,
            float rotation,
            Color color,
            SpriteEffects orientation) {
            if (this.Content != null && this.SpriteSize != Point.Zero) {
                spriteBatch.Draw(
                    this.Content,
                    position * pixelsPerUnit,
                    new Rectangle(this.GetSpriteLocation(spriteIndex), this.SpriteSize),
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
        /// <param name="pixelsPerUnit">The pixels per unit.</param>
        /// <param name="spriteIndex">The sprite index.</param>
        /// <param name="transform">The transform.</param>
        /// <param name="rotation">The rotation.</param>
        /// <param name="color">The color.</param>
        /// <param name="orientation">The orientation.</param>
        public void Draw(
            SpriteBatch spriteBatch,
            ushort pixelsPerUnit,
            byte spriteIndex,
            Transform transform,
            float rotation,
            Color color,
            SpriteEffects orientation) {
            this.Draw(spriteBatch, pixelsPerUnit, spriteIndex, transform.Position, transform.Scale, rotation, color, orientation);
        }

        /// <summary>
        /// Draws the specified sprite.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch.</param>
        /// <param name="pixelsPerUnit">The pixels per unit.</param>
        /// <param name="spriteIndex">The sprite index.</param>
        /// <param name="transform">The transform.</param>
        /// <param name="color">The color.</param>
        /// <param name="orientation">The orientation.</param>
        public void Draw(
            SpriteBatch spriteBatch,
            ushort pixelsPerUnit,
            byte spriteIndex,
            Transform transform,
            Color color,
            SpriteEffects orientation) {
            this.Draw(spriteBatch, pixelsPerUnit, spriteIndex, transform, transform.Rotation, color, orientation);
        }

        /// <inheritdoc />
        public override string GetContentBuildCommands(string contentPath, string fileExtension) {
            // TODO: allow customization of compilation parameters.
            var contentStringBuilder = new StringBuilder();
            contentStringBuilder.AppendLine($"#begin {contentPath}");
            contentStringBuilder.AppendLine($@"/importer:{nameof(TextureImporter)}");
            contentStringBuilder.AppendLine($@"/processor:{nameof(TextureProcessor)}");
            contentStringBuilder.AppendLine(@"/processorParam:ColorKeyColor=255,0,255,255");
            contentStringBuilder.AppendLine(@"/processorParam:ColorKeyEnabled=True");
            contentStringBuilder.AppendLine(@"/processorParam:GenerateMipmaps=False");
            contentStringBuilder.AppendLine(@"/processorParam:PremultiplyAlpha=True");
            contentStringBuilder.AppendLine(@"/processorParam:ResizeToPowerOfTwo=False");
            contentStringBuilder.AppendLine(@"/processorParam:MakeSquare=False");
            contentStringBuilder.AppendLine(@"/processorParam:TextureFormat=Color");
            contentStringBuilder.AppendLine($@"/build:{contentPath}{fileExtension}");
            contentStringBuilder.AppendLine($"#end {contentPath}");
            return contentStringBuilder.ToString();
        }

        /// <summary>
        /// Gets the sprite location based on its index
        /// </summary>
        /// <param name="spriteIndex">The index of the sprite. Counts from left to right, top to bottom.</param>
        /// <returns>The sprite location.</returns>
        public Point GetSpriteLocation(byte spriteIndex) {
            if (!this._spriteIndexToLocation.TryGetValue(spriteIndex, out var location) && this._columns > 0) {
                var row = Math.DivRem(spriteIndex, this._columns, out var column);
                location = new Point(row * this.SpriteSize.X, column * this.SpriteSize.Y);
                this._spriteIndexToLocation[spriteIndex] = location;
            }

            return location;
        }

        /// <inheritdoc />
        public override void LoadContent(Texture2D content) {
            base.LoadContent(content);
            this._spriteIndexToLocation.Clear();
            this.ResetColumnWidth();
            this.ResetRowHeight();
        }

        /// <summary>
        /// Unloads the content.
        /// </summary>
        public void UnloadContent() {
            this.Content = null;
        }

        private void ResetColumnWidth() {
            if (this.Content != null && this._columns != 0) {
                var columnWidthInPixels = this.Content.Width / this._columns;
                this.SpriteSize = new Point(columnWidthInPixels, this.SpriteSize.Y);
            }
        }

        private void ResetRowHeight() {
            if (this.Content != null && this._rows > 0) {
                var rowHeightInPixels = this.Content.Height / this._rows;
                this.SpriteSize = new Point(this.SpriteSize.X, rowHeightInPixels);
            }
        }
    }
}