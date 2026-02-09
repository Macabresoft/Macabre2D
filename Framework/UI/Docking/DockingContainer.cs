namespace Macabre2D.Framework;

using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;

/// <summary>
/// Interface for a container that holds <see cref="IDockable" /> entities.
/// </summary>
public interface IDockingContainer : IBoundableEntity {
    /// <summary>
    /// Requests this container to rearrange the <see cref="IDockable" /> provided.
    /// </summary>
    /// <param name="dockable">The dockable.</param>
    void RequestRearrange(IDockable dockable);
}

/// <summary>
/// A container for <see cref="IDockable" /> entities, which handles arranging them based on its own parent <see cref="BoundingArea" />.
/// </summary>
/// <remarks>
/// This should have a <see cref="IBoundableEntity" /> parent (maybe a <see cref="Camera" />).
/// </remarks>
public class DockingContainer : DockablePanel, IDockingContainer {
    private bool _inheritParentBoundingArea = true;

    /// <summary>
    /// Gets or sets a value indicating whether this should inherit its parents <see cref="BoundingArea" />.
    /// </summary>
    [DataMember]
    [Category(DockingCategoryName)]
    public bool InheritParentBoundingArea {
        get => this._inheritParentBoundingArea;
        set {
            if (value != this._inheritParentBoundingArea) {
                this._inheritParentBoundingArea = value;
                this.RequestReset();
            }
        }
    }

    /// <inheritdoc />
    public override void Initialize(IScene scene, IEntity parent) {
        if (this.Parent is IBoundableEntity oldBoundable) {
            oldBoundable.BoundingAreaChanged -= this.Parent_BoundingAreaChanged;
        }

        base.Initialize(scene, parent);

        this.RearrangeAll();

        if (this.Parent is IBoundableEntity boundable) {
            boundable.BoundingAreaChanged += this.Parent_BoundingAreaChanged;
        }
    }

    /// <inheritdoc />
    public void RequestRearrange(IDockable dockable) {
        dockable.LocalPosition = Vector2.Zero;
        Vector2 amountToMove;
        switch (dockable.Location) {
            case DockLocation.Center:
                amountToMove = new Vector2(this.GetAmountToCenterX(dockable), this.GetAmountToCenterY(dockable));
                break;
            case DockLocation.Left:
                amountToMove = new Vector2(this.BoundingArea.Minimum.X - dockable.BoundingArea.Minimum.X, this.GetAmountToCenterY(dockable));
                break;
            case DockLocation.TopLeft:
                amountToMove = new Vector2(this.BoundingArea.Minimum.X - dockable.BoundingArea.Minimum.X, this.BoundingArea.Maximum.Y - dockable.BoundingArea.Maximum.Y);
                break;
            case DockLocation.Top:
                amountToMove = new Vector2(this.GetAmountToCenterX(dockable), this.BoundingArea.Maximum.Y - dockable.BoundingArea.Maximum.Y);
                break;
            case DockLocation.TopRight:
                amountToMove = new Vector2(this.BoundingArea.Maximum.X - dockable.BoundingArea.Maximum.X, this.BoundingArea.Maximum.Y - dockable.BoundingArea.Maximum.Y);
                break;
            case DockLocation.Right:
                amountToMove = new Vector2(this.BoundingArea.Maximum.X - dockable.BoundingArea.Maximum.X, this.GetAmountToCenterY(dockable));
                break;
            case DockLocation.BottomRight:
                amountToMove = new Vector2(this.BoundingArea.Maximum.X - dockable.BoundingArea.Maximum.X, this.BoundingArea.Minimum.Y - dockable.BoundingArea.Minimum.Y);
                break;
            case DockLocation.Bottom:
                amountToMove = new Vector2(this.GetAmountToCenterX(dockable), this.BoundingArea.Minimum.Y - dockable.BoundingArea.Minimum.Y);
                break;
            case DockLocation.BottomLeft:
                amountToMove = new Vector2(this.BoundingArea.Minimum.X - dockable.BoundingArea.Minimum.X, this.BoundingArea.Minimum.Y - dockable.BoundingArea.Minimum.Y);
                break;
            case DockLocation.None:
                return;
            default:
                throw new ArgumentOutOfRangeException();
        }

        dockable.Move(amountToMove);
    }

    /// <inheritdoc />
    protected override BoundingArea CreateBoundingArea() {
        if (this.InheritParentBoundingArea) {
            return this.Parent is IBoundableEntity boundable ? boundable.BoundingArea : BoundingArea.Empty;
        }

        return base.CreateBoundingArea();
    }

    /// <inheritdoc />
    protected override Vector2 CreateSizeForOffset() => this.InheritParentBoundingArea ? Vector2.Zero : base.CreateSizeForOffset();

    /// <inheritdoc />
    protected override void OnAddChild(IEntity child) {
        base.OnAddChild(child);

        if (child is IDockable dockable) {
            // TODO: this might need a Scene.Invoke?
            this.RequestRearrange(dockable);
        }
    }

    /// <inheritdoc />
    protected override void PerformReset() {
        base.PerformReset();
        this.RearrangeAll();
    }

    private float GetAmountToCenterX(IBoundableEntity dockable) => this.BoundingArea.Maximum.X - 0.5f * this.BoundingArea.Width - (dockable.BoundingArea.Maximum.X - 0.5f * dockable.BoundingArea.Width);

    private float GetAmountToCenterY(IBoundableEntity dockable) => this.BoundingArea.Maximum.Y - 0.5f * this.BoundingArea.Height - (dockable.BoundingArea.Maximum.Y - 0.5f * dockable.BoundingArea.Height);

    private void Parent_BoundingAreaChanged(object? sender, EventArgs e) {
        if (this.InheritParentBoundingArea) {
            this.PerformReset();
        }
    }

    private void RearrangeAll() {
        foreach (var child in this.Children.OfType<IDockable>()) {
            this.RequestRearrange(child);
        }
    }
}