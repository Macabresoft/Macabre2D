namespace Macabresoft.Macabre2D.Framework {
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Runtime.Serialization;
    using Macabresoft.Core;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// A component which will render a single sprite.
    /// </summary>
    [Display(Name = "Sprite Renderer")]
    public class SpriteRenderComponent : GameRenderableComponent, IRotatable {
        private readonly ResettableLazy<BoundingArea> _boundingArea;
        private readonly ResettableLazy<Transform> _pixelTransform;
        private readonly ResettableLazy<Transform> _rotatableTransform;

        [DataMember(Order = 0)]
        [Display(Name = "Sprite")]
        private readonly AssetReference<Sprite> _spriteReference = new();

        private Color _color = Color.White;
        private float _rotation;
        private bool _snapToPixels;

        /// <summary>
        /// Initializes a new instance of the <see cref="SpriteRenderComponent" /> class.
        /// </summary>
        public SpriteRenderComponent() {
            this._boundingArea = new ResettableLazy<BoundingArea>(this.CreateBoundingArea);
            this._pixelTransform = new ResettableLazy<Transform>(this.CreatePixelTransform);
            this._rotatableTransform = new ResettableLazy<Transform>(this.CreateRotatableTransform);
            this._spriteReference.PropertyChanged += this.SpriteReference_PropertyChanged;
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

        /// <inheritdoc />
        public override void Initialize(IGameEntity entity) {
            base.Initialize(entity);
            AssetManager.Instance.ResolveAsset<Sprite, Texture2D>(this._spriteReference);
            this.RenderSettings.PropertyChanged += this.RenderSettings_PropertyChanged;
            this.RenderSettings.Initialize(this.CreateSize);
        }

        /// <inheritdoc />
        public override void Render(FrameTime frameTime, BoundingArea viewBoundingArea) {
            if (this._spriteReference.Asset is Sprite sprite) {
                this.Entity.Scene.Game.SpriteBatch?.Draw(
                    sprite,
                    this._snapToPixels ? this._pixelTransform.Value : this._rotatableTransform.Value,
                    this.Color,
                    this.RenderSettings.Orientation);
            }
        }

        protected override void OnEntityPropertyChanged(PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(IGameEntity.Transform)) {
                this.RenderSettings.InvalidateSize();
                this._boundingArea.Reset();
                this._pixelTransform.Reset();
                this._rotatableTransform.Reset();
            }
            else if (e.PropertyName == nameof(IGameEntity.IsEnabled)) {
                this.RaisePropertyChanged(nameof(this.IsVisible));
            }
        }

        private BoundingArea CreateBoundingArea() {
            BoundingArea result;
            if (this._spriteReference.Asset is Sprite sprite) {
                var width = sprite.Size.X * GameSettings.Instance.InversePixelsPerUnit;
                var height = sprite.Size.Y * GameSettings.Instance.InversePixelsPerUnit;
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
            if (this._spriteReference.Asset is Sprite sprite) {
                return new Vector2(sprite.Size.X, sprite.Size.Y);
            }

            return result;
        }

        private void RenderSettings_PropertyChanged(object? sender, PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(this.RenderSettings.Offset)) {
                this._pixelTransform.Reset();
                this._rotatableTransform.Reset();
                this._boundingArea.Reset();
            }
        }

        private void SpriteReference_PropertyChanged(object? sender, PropertyChangedEventArgs e) {
            if (sender is Sprite && e.PropertyName == nameof(Sprite.Location)) {
                this._boundingArea.Reset();
                this.RenderSettings.InvalidateSize();
            }
        }
    }
}