namespace Macabre2D.Framework {

    using Microsoft.Xna.Framework;
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
    /// A class representing and handling offset for a rectangle area.
    /// </summary>
    [DataContract]
    public sealed class PixelOffset {
        private Vector2 _amount;
        private ResettableLazy<Vector2> _size;
        private PixelOffsetType _type;

        /// <summary>
        /// Initializes a new instance of the <see cref="PixelOffset"/> class.
        /// </summary>
        public PixelOffset() : this(Vector2.Zero, PixelOffsetType.Custom) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PixelOffset"/> class.
        /// </summary>
        /// <param name="amount">The amount.</param>
        /// <param name="type">The type.</param>
        public PixelOffset(Vector2 amount, PixelOffsetType type) {
            this._amount = amount;
            this._type = type;
        }

        /// <summary>
        /// Occurs when <see cref="Amount"/> changes.
        /// </summary>
        public event EventHandler AmountChanged;

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
        public Vector2 Amount {
            get {
                return this._amount;
            }

            set {
                if (value != this._amount) {
                    this._amount = value;
                    this._type = PixelOffsetType.Custom;
                    this.AmountChanged.SafeInvoke(this);
                }
            }
        }

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
                    this.Reset();
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
                this.Reset();
            }
        }

        /// <summary>
        /// Resets the <see cref="Amount"/> property according to the <see cref="Type"/> property.
        /// </summary>
        /// <remarks>
        /// Should be called manually when the size factory will produce a new value. Will be called
        /// automatically when <see cref="Type"/> changes.
        /// </remarks>
        public void Reset() {
            if (this._size != null && this._type != PixelOffsetType.Custom) {
                var size = this._size.Value;

                if (size == Vector2.Zero) {
                    this._amount = size;
                }
                else {
                    switch (this.Type) {
                        case PixelOffsetType.Bottom:
                            this._amount = new Vector2(-size.X * 0.5f, 0f);
                            break;

                        case PixelOffsetType.BottomLeft:
                            this._amount = Vector2.Zero;
                            break;

                        case PixelOffsetType.BottomRight:
                            this._amount = new Vector2(-size.X, 0f);
                            break;

                        case PixelOffsetType.Center:
                            this._amount = new Vector2(-size.X * 0.5f, -size.Y * 0.5f);
                            break;

                        case PixelOffsetType.Left:
                            this._amount = new Vector2(0f, -size.Y * 0.5f);
                            break;

                        case PixelOffsetType.Right:
                            this._amount = new Vector2(-size.X, -size.Y * 0.5f);
                            break;

                        case PixelOffsetType.Top:
                            this._amount = new Vector2(-size.X * 0.5f, -size.Y);
                            break;

                        case PixelOffsetType.TopLeft:
                            this._amount = new Vector2(0f, -size.Y);
                            break;

                        case PixelOffsetType.TopRight:
                            this._amount = new Vector2(-size.X, -size.Y);
                            break;
                    }
                }

                this.AmountChanged.SafeInvoke(this);
            }
        }
    }
}