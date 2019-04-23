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
    public sealed class TextRenderer : BaseComponent, IDrawableComponent, IIdentifiableContentComponent {
        private Lazy<BoundingArea> _boundingArea;
        private Font _font;
        private string _text = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextRenderer"/> class.
        /// </summary>
        public TextRenderer() {
            this._boundingArea = new Lazy<BoundingArea>(this.CreateBoundingArea);
        }

        /// <inheritdoc/>
        public BoundingArea BoundingArea {
            get {
                return this.Font != null ? this._boundingArea.Value : new BoundingArea();
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

                if (this.IsInitialized && this._boundingArea.IsValueCreated) {
                    this._boundingArea = this._boundingArea.Reset(this.CreateBoundingArea);
                }
            }
        }

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

                if (this.IsInitialized && this._boundingArea.IsValueCreated) {
                    this._boundingArea = this._boundingArea.Reset(this.CreateBoundingArea);
                }
            }
        }

        /// <inheritdoc/>
        public void Draw(GameTime gameTime, float viewHeight) {
            if (this.Font?.SpriteFont != null && this.Text != null) {
                var transform = this.WorldTransform;
                this._scene.Game.SpriteBatch.DrawString(
                    this.Font.SpriteFont,
                    this.Text,
                    transform.Position * this._scene.Game.Settings.PixelsPerUnit,
                    this.Color,
                    transform.Rotation.Angle,
                    Vector2.Zero,
                    transform.Scale.X,
                    SpriteEffects.FlipVertically,
                    0f);
            }
        }

        /// <inheritdoc/>
        public bool HasContent(Guid id) {
            return this._font?.Id == id;
        }

        /// <inheritdoc/>
        public override void LoadContent() {
            if (this._scene?.Game != null && this.Font != null) {
                this.Font.LoadSpriteFont(this._scene.Game.Content);
            }

            base.LoadContent();
        }

        /// <inheritdoc/>
        public void RemoveContent(Guid id) {
            if (this.HasContent(id)) {
                this.Font = null;
            }
        }

        /// <inheritdoc/>
        protected override void Initialize() {
            this.TransformChanged += this.Self_TransformChanged;
        }

        private BoundingArea CreateBoundingArea() {
            var size = this.Font.SpriteFont.MeasureString(this.Text);

            var width = size.X / this._scene.Game.Settings.PixelsPerUnit;
            var height = (size.Y * this.LocalScale.X) / this._scene.Game.Settings.PixelsPerUnit;

            if (this.LocalScale.X < 0f) {
                width *= -1;
                height *= -1;
            }

            var points = new List<Vector2> {
                this.WorldTransform.Position,
                this.GetWorldTransform(new Vector2(width, 0f)).Position,
                this.GetWorldTransform(new Vector2(width, height)).Position,
                this.GetWorldTransform(new Vector2(0f, height)).Position
            };

            var minimumX = points.Min(x => x.X);
            var minimumY = points.Min(x => x.Y);
            var maximumX = points.Max(x => x.X);
            var maximumY = points.Max(x => x.Y);

            return new BoundingArea(new Vector2(minimumX, minimumY), new Vector2(maximumX, maximumY));
        }

        private void Self_TransformChanged(object sender, EventArgs e) {
            this._boundingArea = this._boundingArea.Reset(this.CreateBoundingArea);
        }
    }
}