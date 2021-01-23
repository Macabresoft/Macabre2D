namespace Macabresoft.Macabre2D.Framework {
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Runtime.Serialization;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// A tile map component that is either "on" or "off". The on tiles will show the selected sprite.
    /// </summary>
    [Display(Name = "Binary Tile Map")]
    public sealed class BinaryTileMapComponent : RenderableTileMapComponent {
        [DataMember]
        private readonly HashSet<Point> _activeTiles = new();

        [DataMember(Order = 0)]
        [Display(Name = "Sprite")]
        private readonly SpriteReference _spriteReference = new();

        private Color _color = Color.White;
        private Vector2 _tileScale;

        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryTileMapComponent" /> class.
        /// </summary>
        public BinaryTileMapComponent() : base() {
            this._spriteReference.PropertyChanged += this.SpriteReference_PropertyChanged;
        }

        /// <inheritdoc />
        public override IReadOnlyCollection<Point> ActiveTiles => this._activeTiles;

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
            return this._activeTiles.Contains(tilePosition);
        }

        /// <inheritdoc />
        public override void Initialize(IGameEntity entity) {
            base.Initialize(entity);
            this.ResetSpriteScale();
        }

        /// <inheritdoc />
        public override void Render(FrameTime frameTime, BoundingArea viewBoundingArea) {
            if (this.Entity.Scene.Game.SpriteBatch is SpriteBatch spriteBatch && this._spriteReference.Asset is SpriteSheet spriteSheet && this._activeTiles.Any()) {
                foreach (var tile in this._activeTiles) {
                    var boundingArea = this.GetTileBoundingArea(tile);
                    if (boundingArea.Overlaps(viewBoundingArea)) {
                        spriteBatch.Draw(
                            spriteSheet,
                            this._spriteReference.SpriteIndex,
                            boundingArea.Minimum,
                            this._tileScale,
                            this.Color);
                    }
                }
            }
        }

        /// <inheritdoc />
        protected override void ClearActiveTiles() {
            this._activeTiles.Clear();
        }

        /// <inheritdoc />
        protected override Point GetMaximumTile() {
            return new(this._activeTiles.Select(t => t.X).Max(), this._activeTiles.Select(t => t.Y).Max());
        }

        /// <inheritdoc />
        protected override Point GetMinimumTile() {
            return new(this._activeTiles.Select(t => t.X).Min(), this._activeTiles.Select(t => t.Y).Min());
        }

        /// <inheritdoc />
        protected override bool HasActiveTiles() {
            return this._activeTiles.Any();
        }

        protected override void OnEntityPropertyChanged(PropertyChangedEventArgs e) {
            base.OnEntityPropertyChanged(e);

            if (e.PropertyName == nameof(IGameEntity.Transform)) {
                if (this._spriteReference.Asset is SpriteSheet spriteSheet) {
                    this._tileScale = this.GetTileScale(spriteSheet.SpriteSize);
                }
            }
        }

        /// <inheritdoc />
        protected override void ResetBoundingAreas() {
            base.ResetBoundingAreas();
            this.ResetSpriteScale();
        }

        /// <inheritdoc />
        protected override bool TryAddTile(Point tile) {
            return this._activeTiles.Add(tile);
        }

        /// <inheritdoc />
        protected override bool TryRemoveTile(Point tile) {
            return this._activeTiles.Remove(tile);
        }

        private void ResetSpriteScale() {
            if (this._spriteReference.Asset is SpriteSheet spriteSheet) {
                this._tileScale = this.GetTileScale(spriteSheet.SpriteSize);
            }
        }

        private void SpriteReference_PropertyChanged(object sender, PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(SpriteSheet.SpriteSize)) {
                this.ResetBoundingAreas();
            }
        }
    }
}