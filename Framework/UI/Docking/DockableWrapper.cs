namespace Macabresoft.Macabre2D.Framework;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using Macabresoft.Core;
using Microsoft.Xna.Framework;

/// <summary>
/// A dockable wrapper that wraps all of its direct children's bounding areas into one.
/// </summary>
public class DockableWrapper : BaseDockable, IDockable {
    private readonly List<IBoundable> _boundables = new();
    private readonly ResettableLazy<BoundingArea> _boundingArea;
    private bool _isCollapsed;
    private Vector2 _margin = Vector2.Zero;

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

    /// <summary>
    /// Gets or sets a value indicating whether this wrapper is collapsed and should not be considered when stacking elements.
    /// </summary>
    [DataMember]
    public bool IsCollapsed {
        get => this._isCollapsed;
        set {
            if (this.Set(ref this._isCollapsed, value)) {
                this.Reset();
            }
        }
    }

    /// <summary>
    /// A margin to apply to the bounding area.
    /// </summary>
    [DataMember]
    [Category(DockingCategoryName)]
    public Vector2 Margin {
        get => this._margin;
        set {
            this._margin = value;
            this.RequestRearrangeFromParent();
        }
    }

    /// <summary>
    /// Gets the boundable children contained within this wrapper.
    /// </summary>
    protected IReadOnlyCollection<IBoundable> BoundableChildren => this._boundables;

    /// <summary>
    /// Gets the types to ignore by this boundable wrapper.
    /// </summary>
    protected virtual IEnumerable<Type> TypesToIgnore { get; } = [];

    /// <summary>
    /// Gets or sets a value indicating whether this is transforming or not.
    /// </summary>
    protected bool IsTransforming { get; set; }

    /// <inheritdoc />
    public override void Initialize(IScene scene, IEntity parent) {
        base.Initialize(scene, parent);

        foreach (var dockable in this._boundables) {
            dockable.BoundingAreaChanged -= this.Child_BoundingAreaChanged;
        }

        this._boundables.Clear();

        foreach (var child in this.Children.OfType<IBoundable>().Where(x => !this.TypesToIgnore.Any(y => x.GetType().IsAssignableTo(y)))) {
            this._boundables.Add(child);
            child.BoundingAreaChanged += this.Child_BoundingAreaChanged;
        }

        this.Reset();
    }

    /// <summary>
    /// Aggregates the child bounding areas into one.
    /// </summary>
    /// <returns>The aggregated bounding area.</returns>
    protected virtual BoundingArea AggregateBoundingAreas() {
        return this._boundables.Aggregate(BoundingArea.Empty, (current, dockable) => current.Combine(dockable.BoundingArea));
    }

    /// <summary>
    /// Called when a child <see cref="IBoundable" /> changes in size. Is not called if <see cref="IsTransforming" /> is currently set to true.
    /// </summary>
    protected virtual void OnChildBoundingAreaChanged() {
        this.Reset();
    }

    /// <inheritdoc />
    protected override void OnTransformChanged() {
        try {
            this.IsTransforming = true;
            base.OnTransformChanged();
            this.Reset();
        }
        finally {
            this.IsTransforming = false;
        }
    }

    private void Child_BoundingAreaChanged(object? sender, EventArgs e) {
        if (!this.IsTransforming) {
            this.OnChildBoundingAreaChanged();
        }
    }

    private BoundingArea CreateBoundingArea() {
        if (this.IsCollapsed) {
            return BoundingArea.Empty;
        }

        var boundingArea = this.AggregateBoundingAreas();

        if (!boundingArea.IsEmpty && this._margin != Vector2.Zero) {
            boundingArea = new BoundingArea(boundingArea.Minimum - this.Margin, boundingArea.Maximum + this.Margin);
        }

        return boundingArea;
    }

    private void Reset() {
        this._boundingArea.Reset();
        this.BoundingAreaChanged.SafeInvoke(this);
    }
}