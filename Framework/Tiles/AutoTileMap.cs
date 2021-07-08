namespace Macabresoft.Macabre2D.Framework {
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Runtime.Serialization;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Newtonsoft.Json;

    /// <summary>
    /// An entity which maps <see cref="AutoTileSet" /> onto a <see cref="TileGrid" />.
    /// </summary>
    [Display(Name = "Auto Tile Map")]
    public sealed class AutoTileMap : RenderableTileMap {
        [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Replace)]
        private readonly Dictionary<Point, byte> _activeTileToIndex = new();

        private Color _color = Color.White;
        private Vector2 _previousWorldScale;
        private Vector2 _spriteScale = Vector2.One;

        /// <summary>
        /// Initializes a new instance of the <see cref="AutoTileMap" /> class.
        /// </summary>
        public AutoTileMap() : base() {
            this.TileSetReference.PropertyChanged += this.TileSetReference_PropertyChanged;
        }

        /// <inheritdoc />
        public override IReadOnlyCollection<Point> ActiveTiles => this._activeTileToIndex.Keys;

        /// <summary>
        /// Gets the animation reference.
        /// </summary>
        [DataMember(Order = 0, Name = "Tile Set")]
        public SpriteSheetAssetReference<AutoTileSet> TileSetReference { get; } = new();

        /// <summary>
        /// Gets or sets the color.
        /// </summary>
        /// <value>The color.</value>
        [DataMember(Order = 1)]
        public Color Color {
            get => this._color;
            set => this.Set(ref this._color, value);
        }

        /// <inheritdoc />
        public override bool HasActiveTileAt(Point tilePosition) {
            return this._activeTileToIndex.ContainsKey(tilePosition);
        }

        public override void Initialize(IScene scene, IEntity parent) {
            base.Initialize(scene, parent);

            this._previousWorldScale = this.Transform.Scale;
            this.ReevaluateIndexes();
            this.ResetSpriteScale();
        }

        /// <inheritdoc />
        public override void Render(FrameTime frameTime, BoundingArea viewBoundingArea) {
            if (this.Scene.Game.SpriteBatch is SpriteBatch spriteBatch && this.TileSetReference.PackagedAsset is AutoTileSet tileSet && this.TileSetReference.Asset is SpriteSheet spriteSheet) {
                foreach (var (activeTile, tileIndex) in this._activeTileToIndex) {
                    var boundingArea = this.GetTileBoundingArea(activeTile);
                    if (boundingArea.Overlaps(viewBoundingArea)) {
                        var spriteIndex = tileSet.GetSpriteIndex(tileIndex);
                        spriteBatch.Draw(
                            this.Scene.Game.Project.Settings.PixelsPerUnit,
                            spriteSheet,
                            spriteIndex,
                            boundingArea.Minimum,
                            this._spriteScale,
                            this.Color);
                    }
                }
            }
        }

        /// <inheritdoc />
        protected override void ClearActiveTiles() {
            this._activeTileToIndex.Clear();
        }

        /// <inheritdoc />
        protected override Point GetMaximumTile() {
            return new(this._activeTileToIndex.Keys.Select(t => t.X).Max(), this._activeTileToIndex.Keys.Select(t => t.Y).Max());
        }

        /// <inheritdoc />
        protected override Point GetMinimumTile() {
            return new(this._activeTileToIndex.Keys.Select(t => t.X).Min(), this._activeTileToIndex.Keys.Select(t => t.Y).Min());
        }

        /// <inheritdoc />
        protected override bool HasActiveTiles() {
            return this._activeTileToIndex.Any();
        }

        /// <inheritdoc />
        protected override void OnPropertyChanged(object? sender, PropertyChangedEventArgs e) {
            base.OnPropertyChanged(sender, e);

            if (e.PropertyName == nameof(IEntity.Transform) && this.Transform.Scale != this._previousWorldScale) {
                this._previousWorldScale = this.Transform.Scale;
                this.ResetSpriteScale();
            }
        }

        /// <inheritdoc />
        protected override void ResetBoundingArea() {
            base.ResetBoundingArea();
            this.ResetSpriteScale();
        }

        /// <inheritdoc />
        protected override bool TryAddTile(Point tile) {
            var result = false;
            if (!this._activeTileToIndex.ContainsKey(tile)) {
                result = true;

                this._activeTileToIndex[tile] = this.GetIndex(tile);
                this.ReevaluateSurroundingIndexes(tile);
            }

            return result;
        }

        /// <inheritdoc />
        protected override bool TryRemoveTile(Point tile) {
            var result = this._activeTileToIndex.Remove(tile);
            if (result) {
                this.ReevaluateSurroundingIndexes(tile);
            }

            return result;
        }

        private byte GetIndex(Point tile) {
            byte index;
            if (this.TileSetReference.PackagedAsset?.UseIntermediateDirections == true) {
                var direction = IntermediateDirections.None;
                if (this.HasActiveTileAt(tile + new Point(0, 1))) {
                    direction |= IntermediateDirections.North;
                }

                if (this.HasActiveTileAt(tile + new Point(1, 0))) {
                    direction |= IntermediateDirections.East;
                }

                if (this.HasActiveTileAt(tile - new Point(0, 1))) {
                    direction |= IntermediateDirections.South;
                }

                if (this.HasActiveTileAt(tile - new Point(1, 0))) {
                    direction |= IntermediateDirections.West;
                }

                if (this.HasActiveTileAt(tile + new Point(1, 1))) {
                    direction |= IntermediateDirections.NorthEast;
                }

                if (this.HasActiveTileAt(tile + new Point(-1, 1))) {
                    direction |= IntermediateDirections.NorthWest;
                }

                if (this.HasActiveTileAt(tile - new Point(1, 1))) {
                    direction |= IntermediateDirections.SouthWest;
                }

                if (this.HasActiveTileAt(tile - new Point(-1, 1))) {
                    direction |= IntermediateDirections.SouthEast;
                }

                index = (byte)direction;
            }
            else {
                var direction = CardinalDirections.None;
                if (this.HasActiveTileAt(tile + new Point(0, 1))) {
                    direction |= CardinalDirections.North;
                }

                if (this.HasActiveTileAt(tile + new Point(1, 0))) {
                    direction |= CardinalDirections.East;
                }

                if (this.HasActiveTileAt(tile - new Point(0, 1))) {
                    direction |= CardinalDirections.South;
                }

                if (this.HasActiveTileAt(tile - new Point(1, 0))) {
                    direction |= CardinalDirections.West;
                }

                index = (byte)direction;
            }

            return index;
        }

        private void ReevaluateIndex(Point tile) {
            if (this._activeTileToIndex.ContainsKey(tile)) {
                this._activeTileToIndex[tile] = this.GetIndex(tile);
            }
        }

        private void ReevaluateIndexes() {
            var clonedActiveTiles = this._activeTileToIndex.Keys.ToList();
            foreach (var activeTile in clonedActiveTiles) {
                this.ReevaluateIndex(activeTile);
            }
        }

        private void ReevaluateSurroundingIndexes(Point tile) {
            for (var x = -1; x <= 1; x++) {
                for (var y = -1; y <= 1; y++) {
                    this.ReevaluateIndex(tile + new Point(x, y));
                }
            }
        }

        private void ResetSpriteScale() {
            if (this.TileSetReference.Asset is SpriteSheet spriteSheet) {
                this._spriteScale = this.GetTileScale(spriteSheet.SpriteSize);
            }
        }

        private void TileSetReference_PropertyChanged(object? sender, PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(SpriteSheet.SpriteSize)) {
                this.ResetBoundingArea();
            }
        }

        /// <summary>
        /// Represents eight directions from a single tile.
        /// </summary>
        [Flags]
        private enum IntermediateDirections : byte {
            None = 0,
            NorthWest = 1 << 0,
            North = 1 << 1,
            NorthEast = 1 << 2,
            West = 1 << 3,
            East = 1 << 4,
            SouthWest = 1 << 5,
            South = 1 << 6,
            SouthEast = 1 << 7,
            All = NorthWest | North | NorthEast | West | East | SouthWest | South | SouthEast
        }
    }
}