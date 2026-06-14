namespace Macabre2D.Framework;

using System;
using System.Runtime.Serialization;
using Macabresoft.Core;
using Microsoft.Xna.Framework;

/// <summary>
/// A basic entity with a bounding area based on width and height that implements <see cref="IBoundableEntity" />.
/// </summary>
public class BoundableEntity : Entity, IBoundableEntity {
    private readonly ResettableLazy<BoundingArea> _boundingArea;

    /// <inheritdoc />
    public event EventHandler? BoundingAreaChanged;

    /// <summary>
    /// Initializes a new instance of the <see cref="BoundableEntity" /> class.
    /// </summary>
    public BoundableEntity() {
        this._boundingArea = new ResettableLazy<BoundingArea>(this.CreateBoundingArea);
    }

    /// <inheritdoc />
    public BoundingArea BoundingArea => this._boundingArea.Value;

    /// <summary>
    /// Gets the offset.
    /// </summary>
    [DataMember]
    public OffsetOptions OffsetOptions { get; } = new();

    /// <summary>
    /// Gets or sets the height.
    /// </summary>
    [DataMember]
    public float Height {
        get;
        set {
            field = value;
            this.OnSizeChanged();
        }
    }


    /// <summary>
    /// Gets or sets the width.
    /// </summary>
    [DataMember]
    public float Width {
        get;
        set {
            field = value;
            this.OnSizeChanged();
        }
    }

    /// <inheritdoc />
    public override void Initialize(IScene scene, IEntity parent) {
        base.Initialize(scene, parent);
        this.OffsetOptions.Initialize(this.CreateSize);
    }

    /// <summary>
    /// Creates the bounding area.
    /// </summary>
    /// <returns>The bounding area for this to use.</returns>
    protected virtual BoundingArea CreateBoundingArea() => this.OffsetOptions.CreateBoundingArea(this);

    /// <inheritdoc />
    protected override void OnTransformChanged() {
        base.OnTransformChanged();

        if (this.IsInitialized) {
            this.OnSizeChanged();
        }
    }

    private Vector2 CreateSize() => new Vector2(this.Width, this.Height) * this.Measurements.PixelsPerUnit;

    private void OnSizeChanged() {
        this.OffsetOptions.InvalidateSize();
        this._boundingArea.Reset();
        this.BoundingAreaChanged.SafeInvoke(this);
    }
}