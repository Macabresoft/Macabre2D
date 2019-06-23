namespace Macabre2D.Framework.Rendering {

    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;

    /// <summary>
    /// A component which will render the specified text.
    /// </summary>
    public sealed class TextRenderer : BaseComponent, IDrawableComponent, IAssetComponent<Font>, IRotatable {
        private readonly ResettableLazy<BoundingArea> _boundingArea;
        private readonly ResettableLazy<RotatableTransform> _rotatableTransform;
        private readonly ResettableLazy<Vector2> _size;
        private Font _font;
        private Vector2 _offset;
        private OffsetType _offsetType = OffsetType.Custom;
        private string _text = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextRenderer"/> class.
        /// </summary>
        public TextRenderer() {
            this._boundingArea = new ResettableLazy<BoundingArea>(this.CreateBoundingArea);
            this._size = new ResettableLazy<Vector2>(this.CreateSize);
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
        public Color Color { get; set; } = Color.Black;

        /// <summary>
        /// Gets or sets the font.
        /// </summary>
        /// <value>The font.</value>
        [DataMember]
        public Font Font {
            get {
                return this._font;
            }

            set {
                this._font = value;
                this.LoadContent();

                if (this.IsInitialized) {
                    this._boundingArea.Reset();
                    this._size.Reset();
                }
            }
        }

        /// <summary>
        /// Gets or sets the offset. If OFfsetType is anything other than Custom, this will be
        /// overridden when LoadContent(...) is called. This value is in pixels.
        /// </summary>
        /// <value>The offset.</value>
        [DataMember]
        public Vector2 Offset {
            get {
                return this._offset;
            }
            set {
                this.SetOffset(value);
            }
        }

        /// <summary>
        /// Gets or sets the type of the offset.
        /// </summary>
        /// <value>The type of the offset.</value>
        [DataMember]
        public OffsetType OffsetType {
            get {
                return this._offsetType;
            }
            set {
                if (value != this._offsetType) {
                    this._offsetType = value;

                    if (this.IsInitialized) {
                        this.SetOffset();
                    }
                }
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public Rotation Rotation { get; private set; } = new Rotation();

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        [DataMember]
        public string Text {
            get {
                return this._text;
            }

            set {
                if (value == null) {
                    value = string.Empty;
                }

                this._text = value;

                if (this.IsInitialized) {
                    this._boundingArea.Reset();
                    this._size.Reset();
                }
            }
        }

        /// <inheritdoc/>
        public void Draw(GameTime gameTime, BoundingArea viewBoundingArea) {
            if (this.Font?.SpriteFont != null && this.Text != null) {
                var transform = this._rotatableTransform.Value;
                MacabreGame.Instance.SpriteBatch.DrawString(
                    this.Font.SpriteFont,
                    this.Text,
                    transform.Position * GameSettings.Instance.PixelsPerUnit,
                    this.Color,
                    transform.Rotation.Angle,
                    Vector2.Zero,
                    transform.Scale,
                    SpriteEffects.FlipVertically,
                    0f);
            }
        }

        /// <inheritdoc/>
        public IEnumerable<Guid> GetOwnedAssetIds() {
            return this.Font != null ? new[] { this.Font.Id } : new Guid[0];
        }

        /// <inheritdoc/>
        public bool HasAsset(Guid id) {
            return this._font?.Id == id;
        }

        /// <inheritdoc/>
        public override void LoadContent() {
            if (this.Scene.IsInitialized && this.Font != null) {
                this.Font.LoadSpriteFont();
            }

            base.LoadContent();
        }

        /// <inheritdoc/>
        public void RefreshAsset(Font newInstance) {
            if (this.Font == null || this.Font.Id == newInstance?.Id) {
                this.Font = newInstance;
            }
        }

        /// <inheritdoc/>
        public bool RemoveAsset(Guid id) {
            var result = this.HasAsset(id);
            if (result) {
                this.Font = null;
            }

            return result;
        }

        /// <inheritdoc/>
        public bool TryGetAsset(Guid id, out Font asset) {
            var result = this.Font != null && this.Font.Id == id;
            asset = result ? this.Font : null;
            return result;
        }

        /// <inheritdoc/>
        protected override void Initialize() {
            this.TransformChanged += this.Self_TransformChanged;

            if (this.Rotation == null) {
                this.Rotation = new Rotation();
            }

            this.Rotation.AngleChanged += this.Self_TransformChanged;
        }

        private BoundingArea CreateBoundingArea() {
            BoundingArea result;
            if (this.Font != null && this.LocalScale.X != 0f && this.LocalScale.Y != 0f) {
                var inversePixelDensity = GameSettings.Instance.InversePixelsPerUnit;

                var size = this._size.Value * inversePixelDensity;
                var offset = this.Offset * inversePixelDensity;
                var rotationAngle = this.Rotation.Angle;
                var points = new List<Vector2> {
                    this.GetWorldTransform(offset, rotationAngle).Position,
                    this.GetWorldTransform(offset + new Vector2(size.X, 0f), rotationAngle).Position,
                    this.GetWorldTransform(offset + new Vector2(size.X, size.Y), rotationAngle).Position,
                    this.GetWorldTransform(offset + new Vector2(0f, size.Y), rotationAngle).Position
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

        private RotatableTransform CreateRotatableTransform() {
            return this.GetWorldTransform(this.Offset * GameSettings.Instance.InversePixelsPerUnit, this.Rotation.Angle);
        }

        private Vector2 CreateSize() {
            var size = this.Font.SpriteFont.MeasureString(this.Text);

            var width = size.X;
            var height = size.Y;

            if (this.LocalScale.X < 0f) {
                width *= -1f;
            }

            if (this.LocalScale.Y < 0f) {
                height *= -1f;
            }

            return new Vector2(width, height);
        }

        private void Self_TransformChanged(object sender, EventArgs e) {
            this._boundingArea.Reset();
            this._rotatableTransform.Reset();
            this._size.Reset();
        }

        private void SetOffset(Vector2 newOffset) {
            this._offset = newOffset;
            this._rotatableTransform.Reset();
            this._boundingArea.Reset();
        }

        private void SetOffset() {
            if (this.Font == null || string.IsNullOrEmpty(this.Text) || this.OffsetType == OffsetType.Custom) {
                return;
            }

            var size = this._size.Value;
            switch (this.OffsetType) {
                case OffsetType.Bottom:
                    this.SetOffset(new Vector2(-size.X * 0.5f, 0f));
                    break;

                case OffsetType.BottomLeft:
                    this.SetOffset(Vector2.Zero);
                    break;

                case OffsetType.BottomRight:
                    this.SetOffset(new Vector2(-size.X, 0f));
                    break;

                case OffsetType.Center:
                    this.SetOffset(new Vector2(-size.X * 0.5f, -size.Y * 0.5f));
                    break;

                case OffsetType.Left:
                    this.SetOffset(new Vector2(0f, -size.Y * 0.5f));
                    break;

                case OffsetType.Right:
                    this.SetOffset(new Vector2(-size.X, -size.Y * 0.5f));
                    break;

                case OffsetType.Top:
                    this.SetOffset(new Vector2(-size.X * 0.5f, -size.Y));
                    break;

                case OffsetType.TopLeft:
                    this.SetOffset(new Vector2(0f, -size.Y));
                    break;

                case OffsetType.TopRight:
                    this.SetOffset(new Vector2(-size.X, -size.Y));
                    break;
            }
        }
    }
}