namespace Macabresoft.Macabre2D.Framework;

using System;
using System.Linq;
using Macabresoft.Core;

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

        if (this.Parent is IBoundable boundable) {
            boundable.BoundingAreaChanged += this.Parent_BoundingAreaChanged;
        }
    }

    /// <inheritdoc />
    public void RequestRearrange(IDockable dockable) {
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