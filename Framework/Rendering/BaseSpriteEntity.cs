namespace Macabresoft.Macabre2D.Framework {
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.Serialization;
    using Macabresoft.Core;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// An abstract base entity that renders a single sprite, given a sprite sheet and a sprite index.
    /// </summary>
    [Category("Render")]
    public abstract class BaseSpriteEntity : RenderableEntity, IRotatable {
        private readonly ResettableLazy<BoundingArea> _boundingArea;
        private readonly ResettableLazy<Transform> _pixelTransform;
        private readonly ResettableLazy<Transform> _rotatableTransform;
        private Color _color = Color.White;
        private float _rotation;
        private bool _snapToPixels;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseSpriteEntity" /> class.
        /// </summary>
        protected BaseSpriteEntity() {
            this._boundingArea = new ResettableLazy<BoundingArea>(this.CreateBoundingArea);
            this._pixelTransform = new ResettableLazy<Transform>(this.CreatePixelTransform);
            this._rotatableTransform = new ResettableLazy<Transform>(this.CreateRotatableTransform);
        }

        /// <inheritdoc />
        public override BoundingArea BoundingArea => this._boundingArea.Value;

        /// <summary>
        /// Gets the sprite index.
        /// </summary>
        public abstract byte? SpriteIndex { get; }

        /// <summary>
        /// Gets the sprite sheet.
        /// </summary>
        public abstract SpriteSheet? SpriteSheet { get; }

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

        /// <inheritdoc />
        public override void Initialize(IScene scene, IEntity parent) {
            base.Initialize(scene, parent);
            
            this.RenderSettings.PropertyChanged += this.RenderSettings_PropertyChanged;
            this.RenderSettings.Initialize(this.CreateSize);
        }

        /// <inheritdoc />
        public override void Render(FrameTime frameTime, BoundingArea viewBoundingArea) {
            if (this.SpriteIndex.HasValue && this.Scene.Game.SpriteBatch is SpriteBatch spriteBatch && this.SpriteSheet is SpriteSheet spriteSheet) {
                spriteSheet.Draw(
                    spriteBatch,
                    this.Scene.Game.Project.Settings.PixelsPerUnit,
                    this.SpriteIndex.Value,
                    this.GetRenderTransform(),
                    this.Color,
                    this.RenderSettings.Orientation);
            }
        }

        /// <inheritdoc />
        protected override void OnPropertyChanged(object? sender, PropertyChangedEventArgs e) {
            base.OnPropertyChanged(sender, e);
            
            if (e.PropertyName == nameof(IEntity.Transform)) {
                this.Reset();
            }
            else if (e.PropertyName == nameof(IEntity.IsEnabled) && this.IsEnabled) {
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
                var inversePixelsPerUnit = this.Scene.Game.Project.Settings.InversePixelsPerUnit;
                var width = spriteSheet.SpriteSize.X * inversePixelsPerUnit;
                var height = spriteSheet.SpriteSize.Y * inversePixelsPerUnit;
                var offset = this.RenderSettings.Offset * inversePixelsPerUnit;

                var points = new List<Vector2> {
                    this.GetWorldTransform(offset, this.Rotation).Position,
                    this.GetWorldTransform(offset + new Vector2(width, 0f), this.Rotation).Position,
                    this.GetWorldTransform(offset + new Vector2(width, height), this.Rotation).Position,
                    this.GetWorldTransform(offset + new Vector2(0f, height), this.Rotation).Position
                };

                var minimumX = points.Min(x => x.X);
                var minimumY = points.Min(x => x.Y);
                var maximumX = points.Max(x => x.X);
                var maximumY = points.Max(x => x.Y);

                if (this.SnapToPixels) {
                    minimumX = minimumX.ToPixelSnappedValue(this.Scene.Game.Project.Settings);
                    minimumY = minimumY.ToPixelSnappedValue(this.Scene.Game.Project.Settings);
                    maximumX = maximumX.ToPixelSnappedValue(this.Scene.Game.Project.Settings);
                    maximumY = maximumY.ToPixelSnappedValue(this.Scene.Game.Project.Settings);
                }

                result = new BoundingArea(new Vector2(minimumX, minimumY), new Vector2(maximumX, maximumY));
            }
            else {
                result = new BoundingArea();
            }

            return result;
        }

        private Transform CreatePixelTransform() {
            var worldTransform = 
                this.GetWorldTransform(this.RenderSettings.Offset * this.Scene.Game.Project.Settings.InversePixelsPerUnit)
                .ToPixelSnappedValue(this.Scene.Game.Project.Settings);
            return worldTransform;
        }

        private Transform CreateRotatableTransform() {
            return this.GetWorldTransform(this.RenderSettings.Offset * this.Scene.Game.Project.Settings.InversePixelsPerUnit, this.Rotation);
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