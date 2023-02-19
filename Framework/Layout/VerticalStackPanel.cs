namespace Macabresoft.Macabre2D.Framework.Layout;

using System;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;

/// <summary>
/// A stack panel that orients <see cref="IBoundable" /> children vertically.
/// </summary>
public class VerticalStackPanel : Entity {
    private bool _isRearranging;
    private float _margin;

    /// <summary>
    /// Gets or sets the margin between elements in this stack panel.
    /// </summary>
    [DataMember]
    public float Margin {
        get => this._margin;
        set {
            this._margin = value;
            this.RequestRearrange();
        }
    }

    /// <inheritdoc />
    public override void Initialize(IScene scene, IEntity parent) {
        foreach (var boundable in this.Children.OfType<IBoundable>()) {
            boundable.BoundingAreaChanged -= this.Child_BoundingAreaChanged;
        }

        base.Initialize(scene, parent);

        foreach (var boundable in this.Children.OfType<IBoundable>()) {
            boundable.BoundingAreaChanged += this.Child_BoundingAreaChanged;
        }
    }

    /// <inheritdoc />
    protected override void OnAddChild(IEntity child) {
        base.OnAddChild(child);

        if (this.IsInitialized && child is IBoundable boundable) {
            this.RequestRearrange();
            boundable.BoundingAreaChanged += this.Child_BoundingAreaChanged;
        }
    }

    /// <inheritdoc />
    protected override void OnRemoveChild(IEntity child) {
        base.OnRemoveChild(child);

        if (this.IsInitialized && child is IBoundable boundable) {
            this.RequestRearrange();
            boundable.BoundingAreaChanged -= this.Child_BoundingAreaChanged;
        }
    }

    private void Child_BoundingAreaChanged(object? sender, EventArgs e) {
        this.RequestRearrange();
    }

    private void Rearrange() {
        try {
            this._isRearranging = true;

            var boundables = this.Children.OfType<IBoundable>();
            var currentY = 0f;

            foreach (var boundable in boundables) {
                if (boundable is ITransformable transformable) {
                    transformable.LocalPosition = new Vector2(transformable.LocalPosition.X, currentY);
                    currentY -= boundable.BoundingArea.Height + this.Margin;
                }
            }
        }
        finally {
            this._isRearranging = false;
        }
    }

    private void RequestRearrange() {
        if (this.IsInitialized && !this._isRearranging) {
            this.Scene.Invoke(this.Rearrange);
        }
    }
}