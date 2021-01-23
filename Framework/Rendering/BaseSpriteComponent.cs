namespace Macabresoft.Macabre2D.Framework {
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.Serialization;
    using Macabresoft.Core;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// An abstract base component that renders a single sprite, given a sprite sheet and a sprite index.
    /// </summary>
    public abstract class BaseSpriteComponent : GameRenderableComponent, IRotatable {
        private readonly ResettableLazy<BoundingArea> _boundingArea;
        private readonly ResettableLazy<Transform> _pixelTransform;
        private readonly ResettableLazy<Transform> _rotatableTransform;
        private Color _color = Color.White;
        private float _rotation;
        private bool _snapToPixels;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseSpriteComponent" /> class.
        /// </summary>
        protected BaseSpriteComponent() {
            this._boundingArea = new ResettableLazy<BoundingArea>(this.CreateBoundingArea);
            this._pixelTransform = new ResettableLazy<Transform>(this.CreatePixelTransform);
            this._rotatableTransform = new ResettableLazy<Transform>(this.CreateRotatableTransform);
        }

        /// <inheritdoc />
        public override BoundingArea BoundingArea => this._boundingArea.Value;

        /// <summary>
        /// Gets or sets the color.
        /// </summary>
        /// <value>The color.</value>
        [DataMember(Order = 1)]
        public Color Color {
            get => this._color;
            set => this.Set(ref this._color, value);
        }

        /// <summary>
        /// Gets or sets the render settings.
        /// </summary>
        /// <value>The render settings.</value>
        [DataMember(Order = 4, Name = "Render Settings")]
        public RenderSettings RenderSettings { get; private set; } = new();

        /// <inheritdoc />
        [DataMember(Order = 3)]
        public float Rotation {
            get => this._snapToPixels ? 0f : this._rotation;

            set {
                if (this.Set(ref this._rotation, value.NormalizeAngle())) {
                    if (!this._snapToPixels) {
                        this._boundingArea.Reset();
                        this._rotatableTransform.Reset();
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this sprite renderer should snap to the pixel
        /// ratio defined in <see cref="IGameSettings" />.
        /// </summary>
        /// <remarks>Snapping to pixels will disable rotations on this renderer.</remarks>
        /// <value><c>true</c> if this should snap to pixels; otherwise, <c>false</c>.</value>
        [DataMember(Order = 2, Name = "Snap to Pixels")]
        public bool SnapToPixels {
            get => this._snapToPixels;

            set {
                if (this.Set(ref this._snapToPixels, value)) {
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
        /// Gets the sprite index.
        /// </summary>
        protected abstract byte SpriteIndex { get; }

        /// <summary>
        /// Gets the sprite sheet.
        /// </summary>
        protected abstract SpriteSheet? SpriteSheet { get; }

        /// <inheritdoc />
        public override void Initialize(IGameEntity entity) {
            base.Initialize(entity);
            this.RenderSettings.PropertyChanged += this.RenderSettings_PropertyChanged;
            this.RenderSettings.Initialize(this.CreateSize);
        }

        /// <inheritdoc />
        public override void Render(FrameTime frameTime, BoundingArea viewBoundingArea) {
            if (this.Entity.Scene.Game.SpriteBatch is SpriteBatch spriteBatch && this.SpriteSheet is SpriteSheet spriteSheet) {
                spriteSheet.Draw(
                    spriteBatch,
                    this.SpriteIndex,
                    this.GetRenderTransform(),
                    this.Color,
                    this.RenderSettings.Orientation);
            }
        }

        /// <inheritdoc />
        protected override void OnEntityPropertyChanged(PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(IGameEntity.Transform)) {
                this.Reset();
            }
            else if (e.PropertyName == nameof(IGameEntity.IsEnabled)) {
                this.RaisePropertyChanged(nameof(this.IsVisible));
            }
        }

        /// <summary>
        /// Resets the render settings, bounding area, and render transform.
        /// </summary>
        protected void Reset() {
            this.RenderSettings.InvalidateSize();
            this._boundingArea.Reset();
            this._pixelTransform.Reset();
            this._rotatableTransform.Reset();
        }

        private BoundingArea CreateBoundingArea() {
            BoundingArea result;
            if (this.SpriteSheet is SpriteSheet spriteSheet) {
                var width = spriteSheet.SpriteSize.X * GameSettings.Instance.InversePixelsPerUnit;
                var height = spriteSheet.SpriteSize.Y * GameSettings.Instance.InversePixelsPerUnit;
                var offset = this.RenderSettings.Offset * GameSettings.Instance.InversePixelsPerUnit;

                var points = new List<Vector2> {
                    this.Entity.GetWorldTransform(offset, this.Rotation).Position,
                    this.Entity.GetWorldTransform(offset + new Vector2(width, 0f), this.Rotation).Position,
                    this.Entity.GetWorldTransform(offset + new Vector2(width, height), this.Rotation).Position,
                    this.Entity.GetWorldTransform(offset + new Vector2(0f, height), this.Rotation).Position
                };

                var minimumX = points.Min(x => x.X);
                var minimumY = points.Min(x => x.Y);
                var maximumX = points.Max(x => x.X);
                var maximumY = points.Max(x => x.Y);

                if (this.SnapToPixels) {
                    minimumX = minimumX.ToPixelSnappedValue();
                    minimumY = minimumY.ToPixelSnappedValue();
                    maximumX = maximumX.ToPixelSnappedValue();
                    maximumY = maximumY.ToPixelSnappedValue();
                }

                result = new BoundingArea(new Vector2(minimumX, minimumY), new Vector2(maximumX, maximumY));
            }
            else {
                result = new BoundingArea();
            }

            return result;
        }

        private Transform CreatePixelTransform() {
            var worldTransform = this.Entity.GetWorldTransform(this.RenderSettings.Offset * GameSettings.Instance.InversePixelsPerUnit).ToPixelSnappedValue();
            return worldTransform;
        }

        private Transform CreateRotatableTransform() {
            return this.Entity.GetWorldTransform(this.RenderSettings.Offset * GameSettings.Instance.InversePixelsPerUnit, this.Rotation);
        }

        private Vector2 CreateSize() {
            var result = Vector2.Zero;
            if (this.SpriteSheet is SpriteSheet spriteSheet) {
                return new Vector2(spriteSheet.SpriteSize.X, spriteSheet.SpriteSize.Y);
            }

            return result;
        }

        private Transform GetRenderTransform() {
            return this.SnapToPixels ? this._pixelTransform.Value : this._rotatableTransform.Value;
        }

        private void RenderSettings_PropertyChanged(object? sender, PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(this.RenderSettings.Offset)) {
                this._pixelTransform.Reset();
                this._rotatableTransform.Reset();
                this._boundingArea.Reset();
            }
        }
    }
}