namespace Macabresoft.Macabre2D.Tests.Framework;

using System;
using System.Threading;
using Macabresoft.Macabre2D.Framework;

internal class TestRenderableEntity : RenderableEntity {
    private BoundingArea _boundingArea = BoundingArea.MaximumSize;
    public override event EventHandler BoundingAreaChanged;

    public override BoundingArea BoundingArea => this._boundingArea;
    public int RenderCount { get; private set; }

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
}