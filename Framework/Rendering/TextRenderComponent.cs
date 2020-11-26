namespace Macabresoft.Macabre2D.Framework {

    using Macabresoft.Core;
    using Microsoft.Xna.Framework;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.Serialization;

    /// <summary>
    /// A component which will render the specified text.
    /// </summary>
    public class TextRenderComponent : GameRenderableComponent, IAssetComponent<Font>, IRotatable {
        private readonly ResettableLazy<BoundingArea> _boundingArea;
        private readonly ResettableLazy<Transform> _pixelTransform;
        private readonly ResettableLazy<Transform> _rotatableTransform;
        private Color _color = Color.Black;
        private Font? _font;
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
        public override BoundingArea BoundingArea {
            get {
                return this._boundingArea.Value;
            }
        }

        /// <summary>
        /// Gets or sets the color.
        /// </summary>
        /// <value>The color.</value>
        [DataMember(Order = 1)]
        public Color Color {
            get {
                return this._color;
            }

            set {
                this.Set(ref this._color, value);
            }
        }

        /// <summary>
        /// Gets or sets the font.
        /// </summary>
        /// <value>The font.</value>
        [DataMember(Order = 0)]
        public Font? Font {
            get {
                return this._font;
            }

            set {
                if (this.Set(ref this._font, value)) {
                    this.Font?.Load();
                    this._boundingArea.Reset();
                    this.RenderSettings.InvalidateSize();
                }
            }
        }

        /// <summary>
        /// Gets the render settings.
        /// </summary>
        /// <value>The render settings.</value>
        [DataMember(Order = 4, Name = "Render Settings")]
        public RenderSettings RenderSettings { get; private set; } = new RenderSettings();

        /// <inheritdoc />
        [DataMember(Order = 5)]
        public float Rotation {
            get {
                return this._snapToPixels ? 0f : this._rotation;
            }

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
            get {
                return this._snapToPixels;
            }

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
            get {
                return this._text;
            }

            set {
                if (value == null) {
                    value = string.Empty;
                }

                if (this.Set(ref this._text, value)) {
                    this._boundingArea.Reset();
                    this.RenderSettings.InvalidateSize();
                }
            }
        }

        /// <inheritdoc />
        public IEnumerable<Guid> GetOwnedAssetIds() {
            return this.Font != null ? new[] { this.Font.Id } : new Guid[0];
        }

        /// <inheritdoc />
        public bool HasAsset(Guid id) {
            return this._font?.Id == id;
        }

        /// <inheritdoc />
        public override void Initialize(IGameEntity entity) {
            base.Initialize(entity);
            this.Font?.Load();
            this.RenderSettings.PropertyChanged += this.RenderSettings_PropertyChanged;
            this.RenderSettings.Initialize(this.CreateSize);
        }

        /// <inheritdoc />
        public void RefreshAsset(Font newInstance) {
            if (this.Font == null || this.Font.Id == newInstance?.Id) {
                this.Font = newInstance;
            }
        }

        /// <inheritdoc />
        public bool RemoveAsset(Guid id) {
            var result = this.HasAsset(id);
            if (result) {
                this.Font = null;
            }

            return result;
        }

        /// <inheritdoc />
        public override void Render(FrameTime frameTime, BoundingArea viewBoundingArea) {
            if (this.Font?.SpriteFont != null && this.Text != null) {
                if (this.SnapToPixels) {
                    this.Entity.Scene.Game.SpriteBatch?.Draw(this.Font, this.Text, this._pixelTransform.Value, this.Color, this.RenderSettings.Orientation);
                }
                else {
                    this.Entity.Scene.Game.SpriteBatch?.Draw(this.Font, this.Text, this._rotatableTransform.Value, this.Color, this.RenderSettings.Orientation);
                }
            }
        }

        /// <inheritdoc />
        public bool TryGetAsset(Guid id, out Font? asset) {
            var result = this.Font?.Id == id;
            asset = result ? this.Font : null;
            return result;
        }

        /// <inheritdoc />
        protected override void OnEntityPropertyChanged(PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(IGameEntity.Transform)) {
                this._boundingArea.Reset();
                this._pixelTransform.Reset();
                this._rotatableTransform.Reset();
            }
        }

        private BoundingArea CreateBoundingArea() {
            BoundingArea result;
            if (this.Font != null && this.Entity.LocalScale.X != 0f && this.Entity.LocalScale.Y != 0f) {
                var size = this.RenderSettings.Size;
                var width = size.X * GameSettings.Instance.InversePixelsPerUnit;
                var height = size.Y * GameSettings.Instance.InversePixelsPerUnit;
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
            var worldTransform = this.Entity.GetWorldTransform(this.RenderSettings.Offset * GameSettings.Instance.InversePixelsPerUnit);
            return worldTransform.ToPixelSnappedValue();
        }

        private Transform CreateRotatableTransform() {
            return this.Entity.GetWorldTransform(this.RenderSettings.Offset * GameSettings.Instance.InversePixelsPerUnit, this.Rotation);
        }

        private Vector2 CreateSize() {
            return Font?.SpriteFont?.MeasureString(this.Text) ?? Vector2.Zero;
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