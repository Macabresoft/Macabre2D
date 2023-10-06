namespace Macabresoft.Macabre2D.Framework;

using System;
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
    public float Height {
        get => this._height;
        set {
            this._height = value;
            this.RequestRefresh();
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
    public float Width {
        get => this._width;
        set {
            this._width = value;
            this.RequestRefresh();
        }
    }

    /// <inheritdoc />
    public override void Initialize(IScene scene, IEntity parent) {
        base.Initialize(scene, parent);

        this.OffsetOptions.Initialize(this.CreateSize);
    }

    /// <inheritdoc />
    protected override void OnTransformChanged() {
        base.OnTransformChanged();
        this.RequestRefresh();
    }

    private BoundingArea CreateBoundingArea() {
        return this.OffsetOptions.CreateBoundingArea(this);
    }

    private Vector2 CreateSize() {
        return new Vector2(this.Width * this.Project.PixelsPerUnit, this.Height * this.Project.PixelsPerUnit);
    }

    private void RequestRefresh() {
        if (this.IsInitialized) {
            this.ResetSize();
        }
    }

    private void ResetSize() {
        this.OffsetOptions.InvalidateSize();
        this._boundingArea.Reset();
        this.BoundingAreaChanged.SafeInvoke(this);
    }
}