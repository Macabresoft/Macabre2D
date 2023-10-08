namespace Macabresoft.Macabre2D.Framework;

using System;
using System.Linq;
using Macabresoft.Core;
using Microsoft.Xna.Framework;

/// <summary>
/// Interface for a container that holds <see cref="IDockable" /> entities.
/// </summary>
public interface IDockingContainer {
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
/// This should have a <see cref="IBoundable" /> parent (maybe a <see cref="Camera" />).
/// </remarks>
public class DockingContainer : BaseDockable, IBoundable, IDockingContainer {
    /// <inheritdoc />
    public override event EventHandler? BoundingAreaChanged;

    /// <inheritdoc />
    public override BoundingArea BoundingArea => this.Parent is IBoundable boundable ? boundable.BoundingArea : BoundingArea.Empty;

    /// <inheritdoc />
    public override void Initialize(IScene scene, IEntity parent) {
        if (this.Parent is IBoundable oldBoundable) {
            oldBoundable.BoundingAreaChanged -= this.Parent_BoundingAreaChanged;
        }

        base.Initialize(scene, parent);

        this.RearrangeAll();

        if (this.Parent is IBoundable boundable) {
            boundable.BoundingAreaChanged += this.Parent_BoundingAreaChanged;
        }
    }

    /// <inheritdoc />
    public void RequestRearrange(IDockable dockable) {
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
    protected override void OnAddChild(IEntity child) {
        base.OnAddChild(child);

        if (child is IDockable dockable) {
            // TODO: this might need a Scene.Invoke?
            this.RequestRearrange(dockable);
        }
    }

    private float GetAmountToCenterX(IBoundable dockable) {
        return this.BoundingArea.Maximum.X - 0.5f * this.BoundingArea.Width - (dockable.BoundingArea.Maximum.X - 0.5f * dockable.BoundingArea.Width);
    }

    private float GetAmountToCenterY(IBoundable dockable) {
        return this.BoundingArea.Maximum.Y - 0.5f * this.BoundingArea.Height - (dockable.BoundingArea.Maximum.Y - 0.5f * dockable.BoundingArea.Height);
    }

    private void Parent_BoundingAreaChanged(object? sender, EventArgs e) {
        this.BoundingAreaChanged.SafeInvoke(this);
        this.RearrangeAll();
    }

    private void RearrangeAll() {
        foreach (var child in this.Children.OfType<IDockable>()) {
            this.RequestRearrange(child);
        }
    }
}