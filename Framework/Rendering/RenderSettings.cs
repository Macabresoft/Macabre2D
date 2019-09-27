namespace Macabre2D.Framework {

    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents an offset in a rectangular area.
    /// </summary>
    public enum PixelOffsetType {
        Bottom = 0,
        BottomLeft = 1,
        BottomRight = 2,
        Center = 3,
        Custom = 4,
        Left = 5,
        Right = 6,
        Top = 7,
        TopLeft = 8,
        TopRight = 9
    }

    /// <summary>
    /// Render settings for rendering a sprite or text. Handles offset and flipping of the sprite.
    /// </summary>
    [DataContract]
    public sealed class RenderSettings {
        private bool _flipHorizontal;
        private bool _flipVertical;
        private Vector2 _offset;
        private ResettableLazy<Vector2> _size;
        private PixelOffsetType _type;

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderSettings"/> class.
        /// </summary>
        public RenderSettings() : this(Vector2.Zero, PixelOffsetType.Custom) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderSettings"/> class.
        /// </summary>
        /// <param name="amount">The amount.</param>
        /// <param name="type">The type.</param>
        public RenderSettings(Vector2 amount, PixelOffsetType type) {
            this._offset = amount;
            this._type = type;
        }

        /// <summary>
        /// Occurs when <see cref="Offset"/> changes.
        /// </summary>
        public event EventHandler OffsetChanged;

        /// <summary>
        /// Gets or sets a value indicating whether the render should be flipped horizontally.
        /// </summary>
        /// <value><c>true</c> if the render should be flipped horizontally; otherwise, <c>false</c>.</value>
        [DataMember]
        public bool FlipHorizontal {
            get {
                return this._flipHorizontal;
            }

            set {
                this._flipHorizontal = value;

                if (this._flipHorizontal) {
                    this.Orientation |= SpriteEffects.FlipHorizontally;
                }
                else {
                    this.Orientation &= ~SpriteEffects.FlipHorizontally;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the render should be flipped vertically.
        /// </summary>
        /// <value><c>true</c> if the render should be flipped vertically; otherwise, <c>false</c>.</value>
        [DataMember]
        public bool FlipVertical {
            get {
                return this._flipVertical;
            }

            set {
                this._flipVertical = value;

                // Since the default state is to flip vertically, this looks a little backwards, but
                // just trust it!
                if (this._flipVertical) {
                    this.Orientation &= ~SpriteEffects.FlipVertically;
                }
                else {
                    this.Orientation |= SpriteEffects.FlipVertically;
                }
            }
        }

        /// <summary>
        /// Gets or sets the offset amount. This size is in pixels.
        /// </summary>
        /// <remarks>
        /// The reason this is in pixels is because if <see cref="GameSettings.PixelsPerUnit"/>
        /// changes, a pixel value for offset will still be valid. Otherwise this would need to reset
        /// every time <see cref="GameSettings.PixelsPerUnit"/> changes, which is not something the
        /// engine really handles. We allow this pixel value to be a <see cref="float"/> because it
        /// provides greater accuracy when converting it to engine units.
        /// </remarks>
        /// <value>The amount.</value>
        [DataMember(Order = 1)]
        public Vector2 Offset {
            get {
                return this._offset;
            }

            set {
                if (value != this._offset) {
                    this._offset = value;
                    this._type = PixelOffsetType.Custom;
                    this.OffsetChanged.SafeInvoke(this);
                }
            }
        }

        /// <summary>
        /// Gets or sets the orientation.
        /// </summary>
        /// <value>The orientation.</value>
        public SpriteEffects Orientation { get; private set; } = SpriteEffects.FlipVertically;

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        [DataMember(Order = 0)]
        public PixelOffsetType Type {
            get {
                return this._type;
            }

            set {
                if (value != this._type) {
                    this._type = value;
                    this.ResetOffset();
                }
            }
        }

        /// <summary>
        /// Initializes the specified size factory.
        /// </summary>
        /// <param name="sizeFactory">The size factory.</param>
        public void Initialize(Func<Vector2> sizeFactory) {
            if (this._size == null) {
                this._size = new ResettableLazy<Vector2>(sizeFactory ?? new Func<Vector2>(() => Vector2.Zero));
                this.ResetOffset();
            }
        }

        /// <summary>
        /// Resets the <see cref="Offset"/> property according to the <see cref="Type"/> property.
        /// </summary>
        /// <remarks>
        /// Should be called manually when the size factory will produce a new value. Will be called
        /// automatically when <see cref="Type"/> changes.
        /// </remarks>
        public void ResetOffset() {
            if (this._size != null && this._type != PixelOffsetType.Custom) {
                var size = this._size.Value;

                if (size == Vector2.Zero) {
                    this._offset = size;
                }
                else {
                    switch (this.Type) {
                        case PixelOffsetType.Bottom:
                            this._offset = new Vector2(-size.X * 0.5f, 0f);
                            break;

                        case PixelOffsetType.BottomLeft:
                            this._offset = Vector2.Zero;
                            break;

                        case PixelOffsetType.BottomRight:
                            this._offset = new Vector2(-size.X, 0f);
                            break;

                        case PixelOffsetType.Center:
                            this._offset = new Vector2(-size.X * 0.5f, -size.Y * 0.5f);
                            break;

                        case PixelOffsetType.Left:
                            this._offset = new Vector2(0f, -size.Y * 0.5f);
                            break;

                        case PixelOffsetType.Right:
                            this._offset = new Vector2(-size.X, -size.Y * 0.5f);
                            break;

                        case PixelOffsetType.Top:
                            this._offset = new Vector2(-size.X * 0.5f, -size.Y);
                            break;

                        case PixelOffsetType.TopLeft:
                            this._offset = new Vector2(0f, -size.Y);
                            break;

                        case PixelOffsetType.TopRight:
                            this._offset = new Vector2(-size.X, -size.Y);
                            break;
                    }
                }

                this.OffsetChanged.SafeInvoke(this);
            }
        }
    }
}