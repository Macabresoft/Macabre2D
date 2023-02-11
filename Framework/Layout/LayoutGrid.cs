namespace Macabresoft.Macabre2D.Framework.Layout;

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

/// <summary>
/// A grid for layout purposes.
/// </summary>
public class LayoutGrid : Entity, ILayoutArranger {
    [DataMember]
    private readonly List<LayoutDimension> _columns = new();

    [DataMember]
    private readonly List<LayoutDimension> _rows = new();

    private IBoundable? _boundable;

    /// <inheritdoc />
    public BoundingArea GetBoundingArea(byte row, byte column) {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public override void Initialize(IScene scene, IEntity parent) {
        base.Initialize(scene, parent);

        if (this._boundable != null) {
            this._boundable.BoundingAreaChanged -= this.Boundable_BoundingAreaChanged;
        }

        if (this.TryGetParentEntity(out this._boundable)) {
            this._boundable.BoundingAreaChanged += this.Boundable_BoundingAreaChanged;
        }

        this.Rearrange();
    }

    /// <inheritdoc />
    public void RequestRearrange() {
        if (this.IsInitialized) {
            this.Rearrange();
        }
    }

    private void Boundable_BoundingAreaChanged(object? sender, EventArgs e) {
        this.Rearrange();
    }

    private void Rearrange() {
        throw new NotImplementedException();
    }
}