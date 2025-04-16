namespace Macabresoft.Macabre2D.Framework;

using System;
using Macabresoft.Core;
using Microsoft.Xna.Framework;

/// <summary>
/// Draws a collider.
/// </summary>
public sealed class ColliderDrawer : BaseDrawer, IUpdateableEntity {
    private IPhysicsBody? _body;

    /// <inheritdoc />
    public override event EventHandler? BoundingAreaChanged;

    /// <inheritdoc />
    public event EventHandler? ShouldUpdateChanged;

    /// <inheritdoc />
    public event EventHandler? UpdateOrderChanged;

    /// <inheritdoc />
    public override BoundingArea BoundingArea => this._body?.BoundingArea ?? BoundingArea.Empty;

    /// <inheritdoc />
    public bool ShouldUpdate => true;

    /// <inheritdoc />
    public int UpdateOrder => 0;

    /// <inheritdoc />
    public override void Initialize(IScene scene, IEntity parent) {
        base.Initialize(scene, parent);
        this.Reset();
    }

    /// <inheritdoc />
    public override void Render(FrameTime frameTime, BoundingArea viewBoundingArea) {
        this.Render(frameTime, viewBoundingArea, this.Color);
    }

    /// <inheritdoc />
    public override void Render(FrameTime frameTime, BoundingArea viewBoundingArea, Color colorOverride) {
        if (this.PrimitiveDrawer != null && this._body != null && this.SpriteBatch is { } spriteBatch) {
            var lineThickness = this.GetLineThickness(viewBoundingArea.Height);
            var colliders = this._body.GetColliders();

            foreach (var collider in colliders) {
                this.PrimitiveDrawer.DrawCollider(
                    collider,
                    spriteBatch,
                    this.Project.PixelsPerUnit,
                    this.Color,
                    lineThickness,
                    Vector2.Zero);
            }
        }
    }

    /// <inheritdoc />
    public void Update(FrameTime frameTime, InputState inputState) {
        this.Reset();
    }

    private void Body_BoundingAreaChanged(object? sender, EventArgs e) {
        this.BoundingAreaChanged.SafeInvoke(this);
    }

    private void Reset() {
        if (this._body != null) {
            this._body.BoundingAreaChanged -= this.Body_BoundingAreaChanged;
        }

        this._body = this.Parent as IPhysicsBody;

        if (this._body != null) {
            this._body.BoundingAreaChanged += this.Body_BoundingAreaChanged;
        }

        this.BoundingAreaChanged.SafeInvoke(this);
    }
}