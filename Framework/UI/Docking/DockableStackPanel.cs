namespace Macabresoft.Macabre2D.Framework;

using System.Runtime.Serialization;

/// <summary>
/// The direction a stack pan
/// </summary>
public enum StackPanelOrientation {
    Horizontal,
    Vertical
}

/// <summary>
/// A <see cref="DockableWrapper"/> that stacks entries horizontally or vertically.
/// </summary>
public class DockableStackPanel : DockableWrapper {
    private StackPanelOrientation _orientation;

    /// <summary>
    /// Gets or sets the orientation.
    /// </summary>
    [DataMember]
    public StackPanelOrientation Orientation {
        get => this._orientation;
        set {
            if (this.Set(ref this._orientation, value) && !this.IsTransforming) {
                this.OnChildBoundingAreaChanged();
            }
        }
    }
    
    // <inheritdoc />
    protected override void OnChildBoundingAreaChanged() {
        base.OnChildBoundingAreaChanged();
        
        // TODO: arrange boundable children
        try {
            this.IsTransforming = true;
            
            this.RequestRearrangeFromParent();
        }
        finally {
            this.IsTransforming = false;
        }
    }

    private void PlaceBoundablesHorizontally() {
        
    }

    private void PlaceBoundablesVertically() {
        
    }

    // <inheritdoc />
    protected override BoundingArea AggregateBoundingAreas() {
        // TODO: combine them
        return BoundingArea.Empty;
    }
}