namespace Macabre2D.Framework;

using System;
using Macabresoft.Core;
using Microsoft.Xna.Framework;

/// <summary>
/// Draws bounding areas from colliders for debugging purposes.
/// </summary>
public class BoundingAreaDrawer : BaseDrawer, IUpdateableEntity {
    private BoundingArea _boundingArea;

    /// <inheritdoc />
    public override event EventHandler? BoundingAreaChanged;

    /// <inheritdoc />
    public override BoundingArea BoundingArea => this._boundingArea;

    /// <inheritdoc />
    public event EventHandler? ShouldUpdateChanged;

    /// <inheritdoc />
    public bool ShouldUpdate => true;

    /// <inheritdoc />
    public event EventHandler? UpdateOrderChanged;

    /// <inheritdoc />
    public int UpdateOrder => 0;

    /// <inheritdoc />
    public override void Render(FrameTime frameTime, BoundingArea viewBoundingArea) {
        this.Render(frameTime, viewBoundingArea, this.Color);
    }

    /// <inheritdoc />
    public override void Render(FrameTime frameTime, BoundingArea viewBoundingArea, Color colorOverride) {
        if (this.PrimitiveDrawer == null || this.LineThickness <= 0f) {
            return;
        }

        if (this.SpriteBatch is { } spriteBatch && !this._boundingArea.IsEmpty) {
            var thickness = this.GetLineThickness(viewBoundingArea.Height);
            this.PrimitiveDrawer.DrawBoundingArea(spriteBatch, this.Project.PixelsPerUnit, this._boundingArea, colorOverride, thickness);
        }
    }

    /// <inheritdoc />
    public void Update(FrameTime frameTime, InputState inputState) {
        if (this.Parent is IBoundableEntity boundable) {
            if (boundable.BoundingArea != this._boundingArea) {
                this._boundingArea = boundable.BoundingArea;
                this.BoundingAreaChanged.SafeInvoke(this);
            }
        }
        else {
            this._boundingArea = BoundingArea.Empty;
        }
    }
}