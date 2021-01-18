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
        NotifyPropertyChanged,
        IAssetPackage<Sprite, Texture2D>,
        IAssetPackage<SpriteAnimation, Texture2D>,
        IAssetPackage<AutoTileSet, Texture2D>,
        IAssetPackage<RandomTileSet, Texture2D> {
        [DataMember]
        private ObservableCollection<AutoTileSet> _autoTileSets = new();

        private byte _columns = 1;

        private int _columnWidthInPixels;
        private Texture2D? _content;
        private bool _isInitialized;

        [DataMember]
        private ObservableCollection<RandomTileSet> _randomTileSets = new();

        private int _rowHeightInPixels;

        private byte _rows = 1;

        [DataMember]
        private ObservableCollection<SpriteAnimation> _spriteAnimations = new();

        [DataMember]
        private ObservableCollection<Sprite> _sprites = new();

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

                this.CalculateColumnWidth(value);
                this.Set(ref this._columns, value);
            }
        }

        /// <inheritdoc />
        public Texture2D? Content {
            get => this._content;
            private set => this.Set(ref this._content, value);
        }

        /// <inheritdoc />
        [DataMember]
        public Guid ContentId { get; private set; }

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

                this.CalculateRowHeight(value);
                this.Set(ref this._rows, value);
            }
        }

        /// <inheritdoc />
        IReadOnlyCollection<Sprite> IAssetPackage<Sprite>.Assets => this._sprites;

        /// <inheritdoc />
        IReadOnlyCollection<RandomTileSet> IAssetPackage<RandomTileSet>.Assets => this._randomTileSets;

        /// <inheritdoc />
        IReadOnlyCollection<AutoTileSet> IAssetPackage<AutoTileSet>.Assets => this._autoTileSets;

        /// <inheritdoc />
        IReadOnlyCollection<SpriteAnimation> IAssetPackage<SpriteAnimation>.Assets => this._spriteAnimations;

        /// <summary>
        /// Gets the size and location in pixels on this sprite sheet.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <param name="row">The row.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <returns>The size and location in pixels on the <see cref="Texture2D" />.</returns>
        public ( Point Location, Point Size) GetLocationAndSize(byte column, byte row, byte width, byte height) {
            if (this.Content != null) {
                var size = new Point(width * this._columnWidthInPixels, height * this._rowHeightInPixels);
                var location = new Point(column * this._columnWidthInPixels, row * this._rowHeightInPixels);
                return (size, location);
            }

            return (Point.Zero, Point.Zero);
        }

        /// <inheritdoc />
        public void Initialize() {
            if (!this._isInitialized) {
                foreach (var sprite in this._sprites) {
                    sprite.Initialize(this);
                }

                foreach (var randomTileSet in this._randomTileSets) {
                }

                this._isInitialized = true;
            }
        }

        /// <inheritdoc />
        public void LoadContent(Texture2D content) {
            this.Content = content;
        }

        /// <inheritdoc />
        public bool RemoveAsset(RandomTileSet asset) {
            return this._randomTileSets.Remove(asset);
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
        public bool RemoveAsset(Sprite asset) {
            return this._sprites.Remove(asset);
        }

        public bool TryGetAsset(Guid id, out RandomTileSet? asset) {
            asset = this._randomTileSets.FirstOrDefault(x => x.AssetId == id);
            return asset != null;
        }

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
        public bool TryGetAsset(Guid id, out Sprite? asset) {
            asset = this._sprites.FirstOrDefault(x => x.AssetId == id);
            return asset != null;
        }

        /// <inheritdoc />
        RandomTileSet IAssetPackage<RandomTileSet>.AddAsset() {
            var asset = new RandomTileSet();
            this._randomTileSets.Add(asset);
            return asset;
        }

        /// <inheritdoc />
        AutoTileSet IAssetPackage<AutoTileSet>.AddAsset() {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        SpriteAnimation IAssetPackage<SpriteAnimation>.AddAsset() {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        Sprite IAssetPackage<Sprite>.AddAsset() {
            throw new NotImplementedException();
        }

        private void CalculateColumnWidth(byte newNumberOfColumns) {
            if (this.Content != null && newNumberOfColumns != 0) {
                this._columnWidthInPixels = this.Content.Width / newNumberOfColumns;
            }
            else {
                this._columnWidthInPixels = 0;
            }
        }

        private void CalculateRowHeight(byte newNumberOfRows) {
            if (this.Content != null && newNumberOfRows != 0) {
                this._rowHeightInPixels = this.Content.Height / newNumberOfRows;
            }
            else {
                this._rowHeightInPixels = 0;
            }
        }

        /// <inheritdoc />
        bool IAssetPackage<RandomTileSet>.RemoveAsset(Guid assetId) {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        bool IAssetPackage<AutoTileSet>.RemoveAsset(Guid assetId) {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        bool IAssetPackage<SpriteAnimation>.RemoveAsset(Guid assetId) {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        bool IAssetPackage<Sprite>.RemoveAsset(Guid assetId) {
            throw new NotImplementedException();
        }
    }
}