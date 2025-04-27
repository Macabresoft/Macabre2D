namespace Macabresoft.Macabre2D.Tests.Framework;

using System;
using System.Threading;
using Macabresoft.Macabre2D.Framework;
using Macabresoft.Macabre2D.Project.Common;
using Microsoft.Xna.Framework;

internal class TestRenderableEntity : RenderableEntity {
    private BoundingArea _boundingArea = BoundingArea.MaximumSize;
    public override event EventHandler BoundingAreaChanged;

    public override BoundingArea BoundingArea => this._boundingArea;
    public int RenderCount { get; private set; }

    /// <inheritdoc />
    public override RenderPriority RenderPriority { get; } = default;

    public int SleepAmountInMilliseconds { get; set; } = 10;

    public void OverrideBoundingArea(BoundingArea newBoundingArea) {
        this._boundingArea = newBoundingArea;
    }

    public override void Render(FrameTime frameTime, BoundingArea viewBoundingArea) {
        this.RenderCount++;
        if (this.SleepAmountInMilliseconds > 0) {
            Thread.Sleep(this.SleepAmountInMilliseconds);
        }
    }

    public override void Render(FrameTime frameTime, BoundingArea viewBoundingArea, Color colorOverride) {
        this.Render(frameTime, viewBoundingArea);
    }
}