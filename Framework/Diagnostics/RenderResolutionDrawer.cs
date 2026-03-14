namespace Macabre2D.Framework;

using System;
using Macabresoft.Core;
using Microsoft.Xna.Framework;

/// <summary>
/// Draws the render resolution using the view height and internal render resolution obtained from the game's <see cref="IGameProject" /> instance.
/// </summary>
public class RenderResolutionDrawer : BaseDrawer {
    private readonly ResettableLazy<BoundingArea> _boundingArea;

    /// <inheritdoc />
    public override event EventHandler? BoundingAreaChanged;

    public RenderResolutionDrawer() : base() {
        this._boundingArea = new ResettableLazy<BoundingArea>(this.CreateBoundingArea);
    }

    /// <inheritdoc />
    public override BoundingArea BoundingArea => this._boundingArea.Value;


    /// <inheritdoc />
    public override void Render(FrameTime frameTime, BoundingArea viewBoundingArea) {
        this.Render(frameTime, viewBoundingArea, this.Color);
    }

    /// <inheritdoc />
    public override void Render(FrameTime frameTime, BoundingArea viewBoundingArea, Color colorOverride) {
        if (this.PrimitiveDrawer == null || this.LineThickness <= 0f) {
            return;
        }

        if (this.SpriteBatch is { } spriteBatch && !this.BoundingArea.IsEmpty) {
            var thickness = this.GetLineThickness(viewBoundingArea.Height);
            this.PrimitiveDrawer.DrawBoundingArea(spriteBatch, this.Project.PixelsPerUnit, this.BoundingArea, colorOverride, thickness);
        }
    }

    /// <inheritdoc />
    protected override void OnTransformChanged() {
        base.OnTransformChanged();
        this._boundingArea.Reset();
        this.BoundingAreaChanged.SafeInvoke(this);
    }

    private BoundingArea CreateBoundingArea() {
        var height = this.Project.ViewHeight;
        var width = height * this.Project.InternalRenderResolutionRatio;
        return new BoundingArea(this.WorldPosition, width, height);
    }
}