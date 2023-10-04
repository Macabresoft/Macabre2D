namespace Macabresoft.Macabre2D.Framework;

using System;
using System.Linq;
using Macabresoft.Core;

/// <summary>
/// A container for <see cref="IDockable"/> entities, which handles arranging them based on its own parent <see cref="BoundingArea"/>.
/// </summary>
/// <remarks>
/// This should have a <see cref="IBoundable"/> parent (maybe a <see cref="Camera"/>).
/// </remarks>
public class DockingContainer : Entity, IBoundable {
    /// <inheritdoc />
    public BoundingArea BoundingArea => this.Parent is IBoundable boundable ? boundable.BoundingArea : BoundingArea.Empty;
    
    /// <inheritdoc />
    public event EventHandler? BoundingAreaChanged;

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

    public void RequestRearrange(IDockable boundable) {
        
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