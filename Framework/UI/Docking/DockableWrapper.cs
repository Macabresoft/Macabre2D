﻿namespace Macabresoft.Macabre2D.Framework;

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
    private bool _isTransforming;
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
    /// Gets the types to ignore by this boundable wrapper.
    /// </summary>
    protected virtual IEnumerable<Type> TypesToIgnore { get; } = [];

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

    /// <summary>
    /// Resets the bounding area of this entity.
    /// </summary>
    private void Reset() {
        this._boundingArea.Reset();
        this.BoundingAreaChanged.SafeInvoke(this);
    }

    private void Child_BoundingAreaChanged(object? sender, EventArgs e) {
        if (!this._isTransforming) {
            this.Reset();
        }
    }

    private BoundingArea CreateBoundingArea() {
        if (this.IsCollapsed) {
            return BoundingArea.Empty;
        }
        
        var boundingArea = this._boundables.Aggregate(BoundingArea.Empty, (current, dockable) => current.Combine(dockable.BoundingArea));

        if (!boundingArea.IsEmpty && this._margin != Vector2.Zero) {
            boundingArea = new BoundingArea(boundingArea.Minimum - this.Margin, boundingArea.Maximum + this.Margin);
        }

        return boundingArea;
    }
}