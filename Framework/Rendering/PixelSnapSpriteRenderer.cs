namespace Macabre2D.Framework.Rendering {

    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;

    /// <summary>
    /// A component which will render a sprite and snap its pixels to the value defined for pixels
    /// per unit in <see cref="IGameSettings"/>.
    /// </summary>
    public sealed class PixelSnapSpriteRenderer : BaseComponent, IDrawableComponent, IAssetComponent<Sprite> {
        private readonly ResettableLazy<BoundingArea> _boundingArea;
        private Sprite _sprite;

        public PixelSnapSpriteRenderer() {
            this._boundingArea = new ResettableLazy<BoundingArea>(this.CreateBoundingArea);
        }

        /// <inheritdoc/>
        public BoundingArea BoundingArea {
            get {
                return this._boundingArea.Value;
            }
        }

        /// <summary>
        /// Gets or sets the color.
        /// </summary>
        /// <value>The color.</value>
        [DataMember]
        public Color Color { get; set; } = Color.White;

        /// <summary>
        /// Gets or sets the offset.
        /// </summary>
        /// <value>The offset.</value>
        [DataMember]
        public PixelOffset Offset { get; private set; } = new PixelOffset();

        /// <summary>
        /// Gets or sets the sprite.
        /// </summary>
        /// <value>The sprite.</value>
        [DataMember]
        public Sprite Sprite {
            get {
                return this._sprite;
            }
            set {
                if (this._sprite != value) {
                    this._sprite = value;
                    this.LoadContent();
                    this._boundingArea.Reset();

                    if (this.IsInitialized) {
                        this.ResetOffset();
                    }
                }
            }
        }

        /// <inheritdoc/>
        public void Draw(GameTime gameTime, BoundingArea viewBoundingArea) {
            var worldTransform = this.WorldTransform;
            var pixelsPerUnit = GameSettings.Instance.PixelsPerUnit;
            var inversePixelsPerUnit = GameSettings.Instance.InversePixelsPerUnit;
            var position = new Vector2((int)Math.Round(worldTransform.Position.X * pixelsPerUnit, 0) * inversePixelsPerUnit, (int)Math.Round(worldTransform.Position.Y * pixelsPerUnit, 0) * inversePixelsPerUnit);
            var scale = new Vector2((int)Math.Round(worldTransform.Scale.X, 0), (int)Math.Round(worldTransform.Scale.Y));
            MacabreGame.Instance.SpriteBatch.Draw(this.Sprite, position, scale, this.Color);
        }

        /// <inheritdoc/>
        public IEnumerable<Guid> GetOwnedAssetIds() {
            return this.Sprite != null ? new[] { this.Sprite.Id } : new Guid[0];
        }

        /// <inheritdoc/>
        public bool HasAsset(Guid id) {
            return this._sprite?.Id == id;
        }

        /// <inheritdoc/>
        public override void LoadContent() {
            if (this.Scene.IsInitialized) {
                this.Sprite?.LoadContent();
            }

            base.LoadContent();
        }

        /// <inheritdoc/>
        public void RefreshAsset(Sprite newInstance) {
            if (newInstance != null && this.Sprite?.Id == newInstance.Id) {
                this.Sprite = newInstance;
            }
        }

        /// <inheritdoc/>
        public bool RemoveAsset(Guid id) {
            var result = this.HasAsset(id);
            if (result) {
                this.Sprite = null;
            }

            return result;
        }

        /// <inheritdoc/>
        public bool TryGetAsset(Guid id, out Sprite asset) {
            var result = this.Sprite != null && this.Sprite.Id == id;
            asset = result ? this.Sprite : null;
            return result;
        }

        /// <inheritdoc/>
        protected override void Initialize() {
            this.TransformChanged += this.Self_TransformChanged;
            this.Offset.AmountChanged += this.Offset_AmountChanged;
            this.Offset.Initialize(this.CreateSize);
            this.ResetOffset();
        }

        private BoundingArea CreateBoundingArea() {
            // TODO: consider making this correspond to the pixel position and pixel size instead of its actual location. This could cause issues for physics which also uses bounding areas.
            BoundingArea result;
            if (this.Sprite != null) {
                var width = this.Sprite.Size.X * GameSettings.Instance.InversePixelsPerUnit;
                var height = this.Sprite.Size.Y * GameSettings.Instance.InversePixelsPerUnit;
                var offset = this.Offset.Amount * GameSettings.Instance.InversePixelsPerUnit;

                var points = new List<Vector2> {
                    this.GetWorldTransform(offset).Position,
                    this.GetWorldTransform(offset + new Vector2(width, 0f)).Position,
                    this.GetWorldTransform(offset + new Vector2(width, height)).Position,
                    this.GetWorldTransform(offset + new Vector2(0f, height)).Position
                };

                var minimumX = points.Min(x => x.X);
                var minimumY = points.Min(x => x.Y);
                var maximumX = points.Max(x => x.X);
                var maximumY = points.Max(x => x.Y);

                result = new BoundingArea(new Vector2(minimumX, minimumY), new Vector2(maximumX, maximumY));
            }
            else {
                result = new BoundingArea();
            }

            return result;
        }

        private Vector2 CreateSize() {
            var result = Vector2.Zero;
            if (this.Sprite?.Texture is Texture2D texture) {
                return new Vector2(texture.Width, texture.Height);
            }

            return result;
        }

        private void Offset_AmountChanged(object sender, EventArgs e) {
            this._boundingArea.Reset();
        }

        private void ResetOffset() {
            if (this.IsInitialized && this.Sprite?.Texture != null) {
                this.Offset.Reset();
            }
        }

        private void Self_TransformChanged(object sender, EventArgs e) {
            this._boundingArea.Reset();
        }
    }
}