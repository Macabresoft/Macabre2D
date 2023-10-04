namespace Macabresoft.Macabre2D.Framework;

using System;
using Macabresoft.Core;

public class DockableWrapper : Entity, IDockable {
    private readonly ResettableLazy<BoundingArea> _boundingArea;
    public event EventHandler? BoundingAreaChanged;


    public DockableWrapper() : base() {
        this._boundingArea = new ResettableLazy<BoundingArea>(this.CreateBoundingArea);
    }

    // TODO: should sum up the bounding areas of its children
    public BoundingArea BoundingArea => this._boundingArea.Value;
    
    public DockLocation Location { get; set; }
    
    public bool IsDockingEnabled { get; set; }
    
    public DockingMargin Margin { get; set; }

    private BoundingArea CreateBoundingArea() {
        throw new NotImplementedException();
    }
}