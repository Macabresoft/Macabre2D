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
    /// A component which will render the specified text.
    /// </summary>
    [Display(Name = "Text Renderer")]
    public class TextRenderComponent : GameRenderableComponent, IRotatable {
        private readonly ResettableLazy<BoundingArea> _boundingArea;
        private readonly ResettableLazy<Transform> _pixelTransform;
        private readonly ResettableLazy<Transform> _rotatableTransform;
        private Color _color = Color.Black;
        private float _rotation;
        private bool _snapToPixels;
        private string _text = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextRenderComponent" /> class.
        /// </summary>
        public TextRenderComponent() {
            this._boundingArea = new ResettableLazy<BoundingArea>(this.CreateBoundingArea);
            this._pixelTransform = new ResettableLazy<Transform>(this.CreatePixelTransform);
            this._rotatableTransform = new ResettableLazy<Transform>(this.CreateRotatableTransform);
        }

        /// <inheritdoc />
        public override BoundingArea BoundingArea => this._boundingArea.Value;

        /// <summary>
        /// Gets the font reference.
        /// </summary>
        [DataMember(Order = 0)]
        public AssetReference<Font> FontReference { get; } = new();

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
        /// Gets the render settings.
        /// </summary>
        /// <value>The render settings.</value>
        [DataMember(Order = 4, Name = "Render Settings")]
        public RenderSettings RenderSettings { get; private set; } = new();

        /// <inheritdoc />
        [DataMember(Order = 5)]
        public float Rotation {
            get => this._snapToPixels ? 0f : this._rotation;

            set {
                if (this.Set(ref this._rotation, value.NormalizeAngle()) && !this._snapToPixels) {
                    this._boundingArea.Reset();
                    this._rotatableTransform.Reset();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this text renderer should snap to the pixel
        /// ratio defined in <see cref="IGameSettings" />.
        /// </summary>
        /// <remarks>Snapping to pixels will disable rotations on this renderer.</remarks>
        /// <value><c>true</c> if this should snap to pixels; otherwise, <c>false</c>.</value>
        [DataMember(Order = 3)]
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
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        [DataMember(Order = 2)]
        public string Text {
            get => this._text;

            set {
                if (this.Set(ref this._text, value)) {
                    this._boundingArea.Reset();
                    this.RenderSettings.InvalidateSize();
                }
            }
        }

        /// <inheritdoc />
        public override void Initialize(IGameEntity entity) {
            base.Initialize(entity);
            this.Entity.Scene.Game.Assets.ResolveAsset<Font, SpriteFont>(this.FontReference);
            this.RenderSettings.PropertyChanged += this.RenderSettings_PropertyChanged;
            this.RenderSettings.Initialize(this.CreateSize);
        }

        /// <inheritdoc />
        public override void Render(FrameTime frameTime, BoundingArea viewBoundingArea) {
            if (!string.IsNullOrEmpty(this.Text) && this.FontReference.Asset is Font font && this.Entity.Scene.Game.SpriteBatch is SpriteBatch spriteBatch) {
                spriteBatch.Draw(
                    this.Entity.Scene.Game.Project.Settings.PixelsPerUnit,
                    font,
                    this.Text,
                    this.SnapToPixels ? this._pixelTransform.Value : this._rotatableTransform.Value,
                    this.Color,
                    this.RenderSettings.Orientation);
            }
        }

        /// <inheritdoc />
        protected override void OnEntityPropertyChanged(PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(IGameEntity.Transform)) {
                this.Reset();
            }
        }

        private BoundingArea CreateBoundingArea() {
            BoundingArea result;
            if (this.Entity.LocalScale.X != 0f && this.Entity.LocalScale.Y != 0f) {
                var inversePixelsPerUnit = this.Entity.Scene.Game.Project.Settings.InversePixelsPerUnit;
                var (x, y) = this.RenderSettings.Size;
                var width = x * inversePixelsPerUnit;
                var height = y * inversePixelsPerUnit;
                var offset = this.RenderSettings.Offset * inversePixelsPerUnit;
                var points = new List<Vector2> {
                    this.Entity.GetWorldTransform(offset, this.Rotation).Position,
                    this.Entity.GetWorldTransform(offset + new Vector2(width, 0f), this.Rotation).Position,
                    this.Entity.GetWorldTransform(offset + new Vector2(width, height), this.Rotation).Position,
                    this.Entity.GetWorldTransform(offset + new Vector2(0f, height), this.Rotation).Position
                };

                var minimumX = points.Min(point => point.X);
                var minimumY = points.Min(point => point.Y);
                var maximumX = points.Max(point => point.X);
                var maximumY = points.Max(point => point.Y);

                if (this.SnapToPixels) {
                    minimumX = minimumX.ToPixelSnappedValue(this.Entity.Scene.Game.Project.Settings);
                    minimumY = minimumY.ToPixelSnappedValue(this.Entity.Scene.Game.Project.Settings);
                    maximumX = maximumX.ToPixelSnappedValue(this.Entity.Scene.Game.Project.Settings);
                    maximumY = maximumY.ToPixelSnappedValue(this.Entity.Scene.Game.Project.Settings);
                }

                result = new BoundingArea(new Vector2(minimumX, minimumY), new Vector2(maximumX, maximumY));
            }
            else {
                result = new BoundingArea();
            }

            return result;
        }

        private Transform CreatePixelTransform() {
            var worldTransform = this.Entity.GetWorldTransform(this.RenderSettings.Offset * this.Entity.Scene.Game.Project.Settings.InversePixelsPerUnit);
            return worldTransform.ToPixelSnappedValue(this.Entity.Scene.Game.Project.Settings);
        }

        private Transform CreateRotatableTransform() {
            return this.Entity.GetWorldTransform(this.RenderSettings.Offset * this.Entity.Scene.Game.Project.Settings.InversePixelsPerUnit, this.Rotation);
        }

        private Vector2 CreateSize() {
            return this.FontReference.Asset?.Content?.MeasureString(this.Text) ?? Vector2.Zero;
        }

        private void RenderSettings_PropertyChanged(object? sender, PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(this.RenderSettings.Offset)) {
                this.Reset();
            }
        }

        private void Reset() {
            this._pixelTransform.Reset();
            this._rotatableTransform.Reset();
            this._boundingArea.Reset();
        }
    }
}