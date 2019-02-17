namespace Macabre2D.Framework.Rendering {

    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;

    /// <summary>
    /// A component which will render a single sprite.
    /// </summary>
    /// <seealso cref="BaseComponent"/>
    /// <seealso cref="IDrawableComponent"/>
    /// <seealso cref="IDisposable"/>
    /// <seealso cref="BaseComponent"/>
    public sealed class SpriteRenderer : BaseComponent, IDrawableComponent {
        private Lazy<BoundingArea> _boundingArea;
        private Vector2 _offset;

        [DataMember]
        private OffsetType _offsetType = OffsetType.Custom;

        private Sprite _sprite;
        private Lazy<Transform> _spriteTransform;

        /// <summary>
        /// Initializes a new instance of the <see cref="SpriteRenderer"/> class.
        /// </summary>
        public SpriteRenderer() {
            this._boundingArea = new Lazy<BoundingArea>(this.CreateBoundingArea);
            this._spriteTransform = new Lazy<Transform>(this.CreateTransform);
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
                this._offset = value;
                this._spriteTransform = this._spriteTransform.Reset(this.CreateTransform);
                this._boundingArea = this._boundingArea.Reset(this.CreateBoundingArea);
            }
        }

        /// <summary>
        /// Gets or sets the type of the offset.
        /// </summary>
        /// <value>The type of the offset.</value>
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
                    this._boundingArea = this._boundingArea.Reset(this.CreateBoundingArea);

                    if (this.IsInitialized) {
                        this.SetOffset();
                    }
                }
            }
        }

        /// <summary>
        /// Gets the transform.
        /// </summary>
        /// <value>The transform.</value>
        private Transform SpriteTransform {
            get {
                return this._spriteTransform.Value;
            }
        }

        /// <inheritdoc/>
        public void Draw(GameTime gameTime, float viewHeight) {
            if (this.Sprite == null || this.Sprite.Texture == null || this._scene?.Game?.GameSettings == null) {
                return;
            }

            var transform = this.SpriteTransform;
            this._scene.Game.SpriteBatch.Draw(
                this.Sprite.Texture,
                transform.Position * this._scene.Game.GameSettings.PixelsPerUnit,
                new Rectangle(this.Sprite.Location, this.Sprite.Size),
                this.Color,
                transform.Rotation.Angle,
                Vector2.Zero,
                transform.Scale,
                SpriteEffects.FlipVertically,
                0f);
        }

        /// <inheritdoc/>
        public override void LoadContent() {
            if (this.Sprite?.ContentPath != null && this._scene?.Game?.Content != null) {
                this.Sprite.Texture = this._scene.Game.Content.Load<Texture2D>(this.Sprite.ContentPath);
            }

            base.LoadContent();
        }

        /// <inheritdoc/>
        protected override void Initialize() {
            this.SetOffset();
            this.TransformChanged += this.Self_TransformChanged;
        }

        private BoundingArea CreateBoundingArea() {
            if (this.Sprite != null && this._scene?.Game is IGame game) {
                var transform = this.SpriteTransform;
                var pixelDensity = game.GameSettings.PixelsPerUnit;
                var width = this.Sprite.Size.X / pixelDensity;
                var height = this.Sprite.Size.Y / pixelDensity;
                var offset = this.Offset / pixelDensity;

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

                return new BoundingArea(new Vector2(minimumX, minimumY), new Vector2(maximumX, maximumY));
            }

            return new BoundingArea();
        }

        private Transform CreateTransform() {
            var pixelDensity = this._scene?.Game?.GameSettings?.PixelsPerUnit ?? 1f;
            return this.GetWorldTransform(this.Offset / pixelDensity);
        }

        private void Self_TransformChanged(object sender, EventArgs e) {
            this._spriteTransform = this._spriteTransform.Reset(this.CreateTransform);
            this._boundingArea = this._boundingArea.Reset(this.CreateBoundingArea);
        }

        private void SetOffset() {
            if (this.Sprite == null || this.Sprite.Texture == null || this.OffsetType == OffsetType.Custom) {
                return;
            }

            var pixelDensity = this._scene.Game.GameSettings.PixelsPerUnit;

            switch (this.OffsetType) {
                case OffsetType.Bottom:
                    this.Offset = new Vector2(-this.Sprite.Texture.Width * 0.5f, 0f);
                    break;

                case OffsetType.BottomLeft:
                    this.Offset = Vector2.Zero;
                    break;

                case OffsetType.BottomRight:
                    this.Offset = new Vector2(-this.Sprite.Texture.Width, 0f);
                    break;

                case OffsetType.Center:
                    this.Offset = new Vector2(-this.Sprite.Texture.Width * 0.5f, -this.Sprite.Texture.Height * 0.5f);
                    break;

                case OffsetType.Left:
                    this.Offset = new Vector2(0f, -this.Sprite.Texture.Height * 0.5f);
                    break;

                case OffsetType.Right:
                    this.Offset = new Vector2(-this.Sprite.Texture.Width, -this.Sprite.Texture.Height * 0.5f);
                    break;

                case OffsetType.Top:
                    this.Offset = new Vector2(-this.Sprite.Texture.Width * 0.5f, -this.Sprite.Texture.Height);
                    break;

                case OffsetType.TopLeft:
                    this.Offset = new Vector2(0f, -this.Sprite.Texture.Height);
                    break;

                case OffsetType.TopRight:
                    this.Offset = new Vector2(-this.Sprite.Texture.Width, -this.Sprite.Texture.Height);
                    break;
            }
        }
    }
}