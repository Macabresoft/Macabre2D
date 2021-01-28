namespace Macabresoft.Macabre2D.Framework {
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Runtime.Serialization;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// A sprite sheet tied to a single <see cref="Texture2D" /> which also defines sprites, animations, and tile sets.
    /// </summary>
    [DataContract]
    public class SpriteSheet :
        BaseAsset,
        IAssetPackage<SpriteAnimation, Texture2D>,
        IAssetPackage<AutoTileSet, Texture2D> {
        private readonly Dictionary<byte, Point> _spriteIndexToLocation = new();

        [DataMember]
        private ObservableCollection<AutoTileSet> _autoTileSets = new();

        private byte _columns = 1;
        private Texture2D? _content;
        private bool _isInitialized;

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

        /// <inheritdoc />
        public Texture2D? Content {
            get => this._content;
            private set {
                if (this.Set(ref this._content, value)) {
                    this._spriteIndexToLocation.Clear();
                    this.ResetColumnWidth();
                    this.ResetRowHeight();
                }
            }
        }

        /// <inheritdoc />
        [DataMember]
        public Guid ContentId { get; set; } = Guid.NewGuid();

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

        /// <inheritdoc />
        IReadOnlyCollection<AutoTileSet> IAssetPackage<AutoTileSet>.Assets => this._autoTileSets;

        /// <inheritdoc />
        IReadOnlyCollection<SpriteAnimation> IAssetPackage<SpriteAnimation>.Assets => this._spriteAnimations;

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
        public void Initialize() {
            if (!this._isInitialized) {
                foreach (var spriteAnimation in this._spriteAnimations) {
                    spriteAnimation.Initialize(this);
                }

                foreach (var autoTileSet in this._autoTileSets) {
                    autoTileSet.Initialize(this);
                }

                this._isInitialized = true;
            }
        }

        /// <inheritdoc />
        public void LoadContent(Texture2D content) {
            this.Content = content;
        }

        /// <summary>
        /// Unloads the content.
        /// </summary>
        public void UnloadContent() {
            this.Content = null;
        }

        /// <inheritdoc />
        public bool RemoveAsset(AutoTileSet asset) {
            return this._autoTileSets.Remove(asset);
        }

        /// <inheritdoc />
        public bool RemoveAsset(SpriteAnimation asset) {
            return this._spriteAnimations.Remove(asset);
        }

        /// <inheritdoc />
        public bool TryGetAsset(Guid id, out AutoTileSet? asset) {
            asset = this._autoTileSets.FirstOrDefault(x => x.AssetId == id);
            return asset != null;
        }

        /// <inheritdoc />
        public bool TryGetAsset(Guid id, out SpriteAnimation? asset) {
            asset = this._spriteAnimations.FirstOrDefault(x => x.AssetId == id);
            return asset != null;
        }

        /// <inheritdoc />
        AutoTileSet IAssetPackage<AutoTileSet>.AddAsset() {
            var asset = new AutoTileSet();
            this._autoTileSets.Add(asset);
            asset.Initialize(this);
            return asset;
        }

        /// <inheritdoc />
        SpriteAnimation IAssetPackage<SpriteAnimation>.AddAsset() {
            var asset = new SpriteAnimation();
            this._spriteAnimations.Add(asset);
            asset.Initialize(this);
            return asset;
        }

        /// <inheritdoc />
        bool IAssetPackage<AutoTileSet>.RemoveAsset(Guid assetId) {
            var result = false;
            if (this._autoTileSets.FirstOrDefault(x => x.AssetId == assetId) is AutoTileSet asset) {
                result = this._autoTileSets.Remove(asset);
            }

            return result;
        }

        /// <inheritdoc />
        bool IAssetPackage<SpriteAnimation>.RemoveAsset(Guid assetId) {
            var result = false;
            if (this._spriteAnimations.FirstOrDefault(x => x.AssetId == assetId) is SpriteAnimation asset) {
                result = this._spriteAnimations.Remove(asset);
            }

            return result;
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