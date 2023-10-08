namespace Macabresoft.Macabre2D.Framework;

using System;
using System.Collections.Generic;
using System.Linq;
using Macabresoft.Core;

/// <summary>
/// A dockable wrapper that wraps all of its direct children's bounding areas into one.
/// </summary>
public class DockableWrapper : BaseDockable, IDockable {
    private readonly List<IBoundable> _boundables = new();
    private readonly ResettableLazy<BoundingArea> _boundingArea;
    private bool _isTransforming;

    /// <inheritdoc />
    public override event EventHandler? BoundingAreaChanged;

    /// <summary>
    /// Initializes a new instance of the <see cref="DockableWrapper" /> class.
    /// </summary>
    public DockableWrapper() : base() {
        this._boundingArea = new ResettableLazy<BoundingArea>(this.CreateBoundingArea);
    }

    /// <inheritdoc />
    public override BoundingArea BoundingArea => this._boundingArea.Value;

    /// <inheritdoc />
    public override void Initialize(IScene scene, IEntity parent) {
        base.Initialize(scene, parent);

        foreach (var dockable in this._boundables) {
            dockable.BoundingAreaChanged -= this.Child_BoundingAreaChanged;
        }

        this._boundables.Clear();

        foreach (var child in this.Children.OfType<IBoundable>()) {
            this._boundables.Add(child);
            child.BoundingAreaChanged += this.Child_BoundingAreaChanged;
        }

        this.Reset();
    }

    /// <inheritdoc />
    protected override void OnTransformChanged() {
        try {
            this._isTransforming = true;
            base.OnTransformChanged();
            this.Reset();
        }
        finally {
            this._isTransforming = false;
        }
    }

    private void Child_BoundingAreaChanged(object? sender, EventArgs e) {
        if (!this._isTransforming) {
            this.Reset();
        }
    }

    private BoundingArea CreateBoundingArea() {
        var boundingArea = BoundingArea.Empty;
        foreach (var dockable in this._boundables) {
            boundingArea = boundingArea.Combine(dockable.BoundingArea);
        }

        return boundingArea;
    }

    private void Reset() {
        this._boundingArea.Reset();
        this.BoundingAreaChanged.SafeInvoke(this);
    }
}