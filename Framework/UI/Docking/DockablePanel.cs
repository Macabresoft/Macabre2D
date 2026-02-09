namespace Macabre2D.Framework;

using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using Macabresoft.Core;
using Microsoft.Xna.Framework;

/// <summary>
/// A panel with defined height and width that can be docked. Useful for children that might not have bounding areas.
/// </summary>
public class DockablePanel : BaseDockable, IDockable {
    private readonly ResettableLazy<BoundingArea> _boundingArea;
    private float _height;
    private float _width;

    /// <inheritdoc />
    public override event EventHandler? BoundingAreaChanged;

    /// <summary>
    /// Initializes a new instance of the <see cref="DockablePanel" /> class.
    /// </summary>
    public DockablePanel() {
        this._boundingArea = new ResettableLazy<BoundingArea>(this.CreateBoundingArea);
    }

    /// <inheritdoc />
    public override BoundingArea BoundingArea => this._boundingArea.Value;

    /// <summary>
    /// Gets or sets the height.
    /// </summary>
    [DataMember]
    [Category(DockingCategoryName)]
    public float Height {
        get => this._height;
        set {
            this._height = value;
            this.RequestReset();
        }
    }

    /// <summary>
    /// Gets or sets the render options.
    /// </summary>
    /// <value>The render options.</value>
    [DataMember]
    public OffsetOptions OffsetOptions { get; private set; } = new();

    /// <summary>
    /// Gets or sets the width.
    /// </summary>
    [DataMember]
    [Category(DockingCategoryName)]
    public float Width {
        get => this._width;
        set {
            this._width = value;
            this.RequestReset();
        }
    }

    /// <inheritdoc />
    public override void Initialize(IScene scene, IEntity parent) {
        base.Initialize(scene, parent);

        this.OffsetOptions.Initialize(this.CreateSizeForOffset);
        this.ResetSize();
    }

    /// <inheritdoc />
    protected override void OnTransformChanged() {
        base.OnTransformChanged();
        this.RequestReset();
    }
    
    /// <summary>
    /// Creates the bounding area for this instance.
    /// </summary>
    /// <returns>The bounding area.</returns>
    protected virtual BoundingArea CreateBoundingArea() {
        return this.OffsetOptions.CreateBoundingArea(this);
    }

    /// <summary>
    /// Creates the size for the offset of this instance. This is in pixels.
    /// </summary>
    /// <returns>The size.</returns>
    protected virtual Vector2 CreateSizeForOffset() {
        return new Vector2(this.Width * this.Project.PixelsPerUnit, this.Height * this.Project.PixelsPerUnit);
    }

    /// <summary>
    /// Tries to reset the bounding area and size.
    /// </summary>
    protected void RequestReset() {
        if (this.IsInitialized) {
            this.PerformReset();
        }
    }

    /// <summary>
    /// Resets the bounding area and size.
    /// </summary>
    protected virtual void PerformReset() {
        this.ResetSize();
    }

    /// <summary>
    /// Invokes <see cref="BoundingAreaChanged"/>.
    /// </summary>
    protected void InvokeBoundingAreaChanged() {
        this.BoundingAreaChanged.SafeInvoke(this);
    }

    private void ResetSize() {
        this.OffsetOptions.InvalidateSize();
        this._boundingArea.Reset();
        this.BoundingAreaChanged.SafeInvoke(this);
    }
}