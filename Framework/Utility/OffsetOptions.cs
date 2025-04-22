namespace Macabresoft.Macabre2D.Framework;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using Macabresoft.Core;
using Microsoft.Xna.Framework;

/// <summary>
/// Options that define offset.
/// </summary>
[DataContract]
[Category(CommonCategories.Offset)]
public class OffsetOptions : PropertyChangedNotifier {
    private static readonly ResettableLazy<Vector2> EmptySizeFactory = new(() => Vector2.Zero);
    private bool _isInitialized;
    private Vector2 _offset;
    private PixelOffsetType _offsetType;
    private ResettableLazy<Vector2> _size = EmptySizeFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="OffsetOptions" /> class.
    /// </summary>
    public OffsetOptions() : this(Vector2.Zero, PixelOffsetType.Custom) {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="OffsetOptions" /> class.
    /// </summary>
    /// <param name="offset">The offset.</param>
    /// <param name="offsetType">The type.</param>
    public OffsetOptions(Vector2 offset, PixelOffsetType offsetType) {
        this._offset = offset;
        this._offsetType = offsetType;
    }

    /// <summary>
    /// Gets the size in pixels.
    /// </summary>
    /// <remarks>
    /// This is a float to make calculations easier, but it comes directly from a sprite's integer height and width.
    /// </remarks>
    /// <value>The size in pixels.</value>
    public Vector2 Size => this._size.Value;

    /// <summary>
    /// Gets or sets the offset amount. This size is in pixels.
    /// </summary>
    /// <remarks>
    /// The reason this is in pixels is because if <see cref="GameProject.PixelsPerUnit" />
    /// changes, a pixel value for offset will still be valid. Otherwise this would need to
    /// reset every time <see cref="GameProject.PixelsPerUnit" /> changes, which is not
    /// something the engine really handles. We allow this pixel value to be a <see cref="float" />
    /// because it provides further granularity when converting it to engine units.
    /// </remarks>
    /// <value>The amount.</value>
    [DataMember(Order = 1, Name = "Offset")]
    public Vector2 Offset {
        get => this._offset;
        set {
            if (value != this._offset) {
                this._offset = value;

                if (this._isInitialized) {
                    this._offsetType = PixelOffsetType.Custom;
                    this.RaisePropertyChanged(nameof(this.OffsetType));
                }
            }
        }
    }

    /// <summary>
    /// Gets or sets the offset type.
    /// </summary>
    [DataMember(Order = 0, Name = "Offset Type")]
    public PixelOffsetType OffsetType {
        get => this._offsetType;
        set {
            if (value != this._offsetType) {
                this._offsetType = value;

                if (this._isInitialized) {
                    this.ResetOffset();
                }
            }
        }
    }

    /// <summary>
    /// Creates a bounding area for an entity.
    /// </summary>
    /// <param name="entity">The entity.</param>
    /// <returns>The bounding area.</returns>
    public BoundingArea CreateBoundingArea(IEntity entity) {
        BoundingArea result;
        if (this.Size != Vector2.Zero) {
            var unitsPerPixel = entity.Project.UnitsPerPixel;
            var (x, y) = this.Size;
            var width = x * unitsPerPixel;
            var height = y * unitsPerPixel;
            var offset = this.Offset * unitsPerPixel;
            var points = new List<Vector2> {
                entity.GetWorldPosition(offset),
                entity.GetWorldPosition(offset + new Vector2(width, 0f)),
                entity.GetWorldPosition(offset + new Vector2(width, height)),
                entity.GetWorldPosition(offset + new Vector2(0f, height))
            };

            var minimumX = points.Min(point => point.X);
            var minimumY = points.Min(point => point.Y);
            var maximumX = points.Max(point => point.X);
            var maximumY = points.Max(point => point.Y);

            result = new BoundingArea(new Vector2(minimumX, minimumY), new Vector2(maximumX, maximumY));
        }
        else {
            result = BoundingArea.Empty;
        }

        return result;
    }

    /// <summary>
    /// Initializes the specified size factory.
    /// </summary>
    /// <param name="sizeFactory">The size factory.</param>
    public void Initialize(Func<Vector2> sizeFactory) {
        this._size = new ResettableLazy<Vector2>(sizeFactory);
        this.ResetOffset();
        this._isInitialized = true;
    }

    /// <summary>
    /// Invalidates the size.
    /// </summary>
    public void InvalidateSize() {
        if (this._isInitialized) {
            this._size.Reset();
            this.ResetOffset(); 
        }
    }

    /// <summary>
    /// Resets the <see cref="Offset" /> property according to the <see cref="OffsetType" /> property.
    /// </summary>
    /// <remarks>
    /// Should be called manually when the size factory will produce a new value. Will be called
    /// automatically when <see cref="OffsetType" /> changes.
    /// </remarks>
    public void ResetOffset() {
        var size = this.Size;

        if (this._offsetType != PixelOffsetType.Custom) {
            if (this.Size != Vector2.Zero) {
                this._offset = this.OffsetType switch {
                    PixelOffsetType.Bottom => new Vector2(-size.X * 0.5f, 0f),
                    PixelOffsetType.BottomLeft => Vector2.Zero,
                    PixelOffsetType.BottomRight => new Vector2(-size.X, 0f),
                    PixelOffsetType.Center => new Vector2(-size.X * 0.5f, -size.Y * 0.5f),
                    PixelOffsetType.Left => new Vector2(0f, -size.Y * 0.5f),
                    PixelOffsetType.Right => new Vector2(-size.X, -size.Y * 0.5f),
                    PixelOffsetType.Top => new Vector2(-size.X * 0.5f, -size.Y),
                    PixelOffsetType.TopLeft => new Vector2(0f, -size.Y),
                    PixelOffsetType.TopRight => new Vector2(-size.X, -size.Y),
                    _ => this._offset
                };
            }
            else {
                this._offset = Vector2.Zero;
            }

            this.RaisePropertyChanged(nameof(this.Offset));
        }
    }
}