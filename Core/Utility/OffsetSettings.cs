namespace Macabresoft.MonoGame.Core {

    using Macabresoft.Core;
    using Microsoft.Xna.Framework;
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Settings that define offset.
    /// </summary>
    [DataContract]
    public class OffsetSettings : NotifyPropertyChanged {
        private Vector2 _offset;
        private ResettableLazy<Vector2> _size = new ResettableLazy<Vector2>(new Func<Vector2>(() => Vector2.Zero));
        private PixelOffsetType _type;

        /// <summary>
        /// Initializes a new instance of the <see cref="OffsetSettings" /> class.
        /// </summary>
        public OffsetSettings() : this(Vector2.Zero, PixelOffsetType.Custom) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OffsetSettings" /> class.
        /// </summary>
        /// <param name="offset">The offset.</param>
        /// <param name="type">The type.</param>
        public OffsetSettings(Vector2 offset, PixelOffsetType type) {
            this._offset = offset;
            this._type = type;
        }

        /// <summary>
        /// Gets or sets the offset amount. This size is in pixels.
        /// </summary>
        /// <remarks>
        /// The reason this is in pixels is because if <see cref="GameSettings.PixelsPerUnit" />
        /// changes, a pixel value for offset will still be valid. Otherwise this would need to
        /// reset every time <see cref="GameSettings.PixelsPerUnit" /> changes, which is not
        /// something the engine really handles. We allow this pixel value to be a <see cref="float"
        /// /> because it provides greater accuracy when converting it to engine units.
        /// </remarks>
        /// <value>The amount.</value>
        [DataMember(Order = 1)]
        public Vector2 Offset {
            get {
                return this._offset;
            }

            set {
                if (this.Set(ref this._offset, value)) {
                    this._type = PixelOffsetType.Custom;
                    this.RaisePropertyChanged(true, nameof(this.OffsetType));
                }
            }
        }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        [DataMember(Order = 0, Name = "Offset Type")]
        public PixelOffsetType OffsetType {
            get {
                return this._type;
            }

            set {
                if (this.Set(ref this._type, value)) {
                    this.ResetOffset();
                }
            }
        }

        /// <summary>
        /// Gets the size.
        /// </summary>
        /// <value>The size.</value>
        public Vector2 Size {
            get {
                return this._size?.Value ?? Vector2.Zero;
            }
        }

        /// <summary>
        /// Initializes the specified size factory.
        /// </summary>
        /// <param name="sizeFactory">The size factory.</param>
        public void Initialize(Func<Vector2> sizeFactory) {
            this._size = new ResettableLazy<Vector2>(sizeFactory ?? new Func<Vector2>(() => Vector2.Zero));
            this.ResetOffset();
        }

        /// <summary>
        /// Invalidates the size.
        /// </summary>
        public void InvalidateSize() {
            this._size.Reset();
            this.ResetOffset();
        }

        /// <summary>
        /// Resets the <see cref="Offset" /> property according to the <see cref="OffsetType" /> property.
        /// </summary>
        /// <remarks>
        /// Should be called manually when the size factory will produce a new value. Will be called
        /// automatically when <see cref="OffsetType" /> changes.
        /// </remarks>
        public void ResetOffset() {
            if (this._type != PixelOffsetType.Custom) {
                var size = this._size.Value;

                if (size == Vector2.Zero) {
                    this._offset = size;
                }
                else {
                    switch (this.OffsetType) {
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

                this.RaisePropertyChanged(true, nameof(this.Offset));
            }
        }
    }
}