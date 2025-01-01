namespace Macabresoft.Macabre2D.Framework;

using System.Runtime.Serialization;
using Microsoft.Xna.Framework;

/// <summary>
/// The direction a stack pan
/// </summary>
public enum StackPanelOrientation {
    Horizontal,
    Vertical
}

/// <summary>
/// A <see cref="DockableWrapper" /> that stacks entries horizontally or vertically.
/// </summary>
public class DockableStackPanel : DockableWrapper, IDockingContainer {
    private StackPanelOrientation _orientation = StackPanelOrientation.Vertical;
    private float _spacing;

    /// <summary>
    /// Gets or sets the orientation.
    /// </summary>
    [DataMember]
    public StackPanelOrientation Orientation {
        get => this._orientation;
        set {
            if (this.Set(ref this._orientation, value) && !this.IsTransforming) {
                this.Arrange();
            }
        }
    }

    /// <summary>
    /// Gets or sets the spacing between elements.
    /// </summary>
    [DataMember]
    public float Spacing {
        get => this._spacing;
        set {
            if (this.Set(ref this._spacing, value)) {
                this.Arrange();
            }
        }
    }

    // <inheritdoc />
    public override void Initialize(IScene scene, IEntity parent) {
        base.Initialize(scene, parent);
        this.OnChildBoundingAreaChanged();
    }

    // <inheritdoc />
    public void RequestRearrange(IDockable dockable) {
        this.Arrange();
    }

    // <inheritdoc />
    protected override void OnChildBoundingAreaChanged() {
        // Arrange and then allow the bounding area to reconstruct.
        this.Arrange();

        base.OnChildBoundingAreaChanged();
    }

    private void Arrange() {
        if (!this.IsTransforming && this.IsInitialized) {
            try {
                this.IsTransforming = true;
                if (this.Orientation == StackPanelOrientation.Horizontal) {
                    this.ArrangeHorizontally();
                }
                else {
                    this.ArrangeVertically();
                }
            }
            finally {
                this.IsTransforming = false;
            }

            this.RequestRearrangeFromParent();
        }
    }

    private void ArrangeHorizontally() {
        var currentXPosition = 0f;

        foreach (var boundable in this.BoundableChildren) {
            boundable.LocalPosition = new Vector2(currentXPosition, boundable.LocalPosition.Y);

            if (!boundable.IsEmpty()) {
                currentXPosition += boundable.BoundingArea.Width + this.Spacing;
            }
        }
    }

    private void ArrangeVertically() {
        var currentYPosition = 0f;

        foreach (var boundable in this.BoundableChildren) {
            boundable.LocalPosition = new Vector2(boundable.LocalPosition.X, currentYPosition);

            if (!boundable.IsEmpty()) {
                currentYPosition -= boundable.BoundingArea.Height - this.Spacing;
            }
        }
    }
}