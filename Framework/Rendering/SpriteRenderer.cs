namespace Macabre2D.Framework {

    using Microsoft.Xna.Framework;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;

    /// <summary>
    /// A component which will render a single sprite.
    /// </summary>
    public class SpriteRenderer : BaseComponent, IDrawableComponent, IAssetComponent<Sprite>, IRotatable {
        private readonly ResettableLazy<BoundingArea> _boundingArea;
        private readonly ResettableLazy<Transform> _pixelTransform;
        private readonly ResettableLazy<RotatableTransform> _rotatableTransform;
        private bool _snapToPixels;
        private Sprite _sprite;

        /// <summary>
        /// Initializes a new instance of the <see cref="SpriteRenderer"/> class.
        /// </summary>
        public SpriteRenderer() {
            this._boundingArea = new ResettableLazy<BoundingArea>(this.CreateBoundingArea);
            this._pixelTransform = new ResettableLazy<Transform>(this.CreatePixelTransform);
            this._rotatableTransform = new ResettableLazy<RotatableTransform>(this.CreateRotatableTransform);
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

        /// <inheritdoc/>
        [DataMember]
        public Rotation Rotation { get; private set; } = new Rotation();

        /// <summary>
        /// Gets or sets a value indicating whether this sprite renderer should snap to the pixel
        /// ratio defined in <see cref="IGameSettings"/>.
        /// </summary>
        /// <remarks>Snapping to pixels will disable rotations on this renderer.</remarks>
        /// <value><c>true</c> if this should snap to pixels; otherwise, <c>false</c>.</value>
        [DataMember]
        public bool SnapToPixels {
            get {
                return this._snapToPixels;
            }

            set {
                if (value != this._snapToPixels) {
                    this._snapToPixels = value;
                    if (!this._snapToPixels) {
                        this._rotatableTransform.Reset();
                    }
                    else {
                        this._pixelTransform.Reset();
                    }

                    this._boundingArea.Reset();
                }
            }
        }

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
            if (this._snapToPixels) {
                MacabreGame.Instance.SpriteBatch.Draw(this.Sprite, this._pixelTransform.Value, this.Color);
            }
            else {
                MacabreGame.Instance.SpriteBatch.Draw(this.Sprite, this._rotatableTransform.Value, this.Color);
            }
        }

        /// <inheritdoc/>
        public virtual IEnumerable<Guid> GetOwnedAssetIds() {
            return this.Sprite != null ? new[] { this.Sprite.Id } : new Guid[0];
        }

        /// <inheritdoc/>
        public virtual bool HasAsset(Guid id) {
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
        public virtual void RefreshAsset(Sprite newInstance) {
            if (newInstance != null && this.Sprite?.Id == newInstance.Id) {
                this.Sprite = newInstance;
            }
        }

        /// <inheritdoc/>
        public virtual bool RemoveAsset(Guid id) {
            var result = this.HasAsset(id);
            if (result) {
                this.Sprite = null;
            }

            return result;
        }

        /// <inheritdoc/>
        public virtual bool TryGetAsset(Guid id, out Sprite asset) {
            var result = this.Sprite != null && this.Sprite.Id == id;
            asset = result ? this.Sprite : null;
            return result;
        }

        /// <inheritdoc/>
        protected override void Initialize() {
            this.TransformChanged += this.Self_TransformChanged;
            this.Rotation.AngleChanged += this.Self_TransformChanged;
            this.Offset.AmountChanged += this.Offset_AmountChanged;
            this.Offset.Initialize(this.CreateSize);
            this.ResetOffset();
        }

        private BoundingArea CreateBoundingArea() {
            BoundingArea result;
            if (this.Sprite != null) {
                var width = this.Sprite.Size.X * GameSettings.Instance.InversePixelsPerUnit;
                var height = this.Sprite.Size.Y * GameSettings.Instance.InversePixelsPerUnit;
                var offset = this.Offset.Amount * GameSettings.Instance.InversePixelsPerUnit;
                var rotationAngle = this.Rotation.Angle;

                var points = new List<Vector2> {
                    this.GetWorldTransform(offset, rotationAngle).Position,
                    this.GetWorldTransform(offset + new Vector2(width, 0f), rotationAngle).Position,
                    this.GetWorldTransform(offset + new Vector2(width, height), rotationAngle).Position,
                    this.GetWorldTransform(offset + new Vector2(0f, height), rotationAngle).Position
                };

                if (this.SnapToPixels) {
                    var minimumX = points.Min(x => x.X).ToPixelSnappedValue();
                    var minimumY = points.Min(x => x.Y).ToPixelSnappedValue();
                    var maximumX = points.Max(x => x.X).ToPixelSnappedValue();
                    var maximumY = points.Max(x => x.Y).ToPixelSnappedValue();

                    result = new BoundingArea(new Vector2(minimumX, minimumY), new Vector2(maximumX, maximumY));
                }
                else {
                    result = new BoundingArea(new Vector2(points.Min(x => x.X), points.Min(x => x.Y)), new Vector2(points.Max(x => x.X), points.Max(x => x.Y)));
                }
            }
            else {
                result = new BoundingArea();
            }

            return result;
        }

        private Transform CreatePixelTransform() {
            var worldTransform = this.GetWorldTransform(this.Offset.Amount * GameSettings.Instance.InversePixelsPerUnit);
            var pixelsPerUnit = GameSettings.Instance.PixelsPerUnit;
            var inversePixelsPerUnit = GameSettings.Instance.InversePixelsPerUnit;
            var position = new Vector2((int)Math.Round(worldTransform.Position.X * pixelsPerUnit, 0) * inversePixelsPerUnit, (int)Math.Round(worldTransform.Position.Y * pixelsPerUnit, 0) * inversePixelsPerUnit);
            var scale = new Vector2((int)Math.Round(worldTransform.Scale.X, 0), (int)Math.Round(worldTransform.Scale.Y));
            return new Transform(position, scale);
        }

        private RotatableTransform CreateRotatableTransform() {
            return this.GetWorldTransform(this.Offset.Amount * GameSettings.Instance.InversePixelsPerUnit, this.Rotation.Angle);
        }

        private Vector2 CreateSize() {
            var result = Vector2.Zero;
            if (this.Sprite != null) {
                return new Vector2(this.Sprite.Size.X, this.Sprite.Size.Y);
            }

            return result;
        }

        private void Offset_AmountChanged(object sender, EventArgs e) {
            this._pixelTransform.Reset();
            this._rotatableTransform.Reset();
            this._boundingArea.Reset();
        }

        private void ResetOffset() {
            if (this.IsInitialized && this.Sprite?.Texture != null) {
                this.Offset.Reset();
            }
        }

        private void Self_TransformChanged(object sender, EventArgs e) {
            this._boundingArea.Reset();
            this._pixelTransform.Reset();
            this._rotatableTransform.Reset();
        }
    }
}