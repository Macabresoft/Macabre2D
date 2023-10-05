namespace Macabresoft.Macabre2D.Framework;

using System;
using Macabresoft.Core;

public class DockableWrapper : BaseDockable, IDockable {
    private readonly ResettableLazy<BoundingArea> _boundingArea;
    public override event EventHandler? BoundingAreaChanged;


    public DockableWrapper() : base() {
        this._boundingArea = new ResettableLazy<BoundingArea>(this.CreateBoundingArea);
    }

    // TODO: should sum up the bounding areas of its children
    public override BoundingArea BoundingArea => this._boundingArea.Value;

    private BoundingArea CreateBoundingArea() {
        throw new NotImplementedException();
    }
}