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
        public void Draw(GameTime gameTime, float viewHeight) {
            if (this.Font?.SpriteFont != null && this.Text != null) {
                var transform = this._rotatableTransform.Value;
                this._scene.Game.SpriteBatch.DrawString(
                    this.Font.SpriteFont,
                    this.Text,
                    transform.Position * this._scene.Game.Settings.PixelsPerUnit,
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
            if (this._scene?.Game != null && this.Font != null) {
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
            if (this.Font != null && this.LocalScale.X != 0f && this.LocalScale.Y != 0f && this._scene?.Game is IGame game) {
                var size = this._size.Value;
                var rotationAngle = this.Rotation.Angle;
                var points = new List<Vector2> {
                    this.GetWorldTransform(rotationAngle).Position,
                    this.GetWorldTransform(new Vector2(size.X, 0f), rotationAngle).Position,
                    this.GetWorldTransform(new Vector2(size.X, size.Y), rotationAngle).Position,
                    this.GetWorldTransform(new Vector2(0f, size.Y), rotationAngle).Position
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
            return this.GetWorldTransform(this.Rotation.Angle);
        }

        private Vector2 CreateSize() {
            Vector2 result;
            if (this._scene?.Game is IGame game) {
                var size = this.Font.SpriteFont.MeasureString(this.Text);

                var width = size.X / game.Settings.PixelsPerUnit;
                var height = size.Y / game.Settings.PixelsPerUnit;

                if (this.LocalScale.X < 0f) {
                    width *= -1f;
                }

                if (this.LocalScale.Y < 0f) {
                    height *= -1f;
                }

                result = new Vector2(width, height);
            }
            else {
                result = Vector2.Zero;
            }

            return result;
        }

        private void Self_TransformChanged(object sender, EventArgs e) {
            this._boundingArea.Reset();
            this._rotatableTransform.Reset();
            this._size.Reset();
        }
    }
}