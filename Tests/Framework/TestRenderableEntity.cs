namespace Macabresoft.Macabre2D.Tests.Framework;

using System.Threading;
using Macabresoft.Macabre2D.Framework;

internal class TestRenderableEntity : RenderableEntity {
    private BoundingArea _boundingArea = BoundingArea.MaximumSize;
    public int RenderCount { get; private set; }
    
    public int SleepAmountInMilliseconds { get; set; } = 10;

    public override BoundingArea BoundingArea => this._boundingArea;

    public void OverrideBoundingArea(BoundingArea newBoundingArea) {
        this._boundingArea = newBoundingArea;
    }

    public override void Render(FrameTime frameTime, BoundingArea viewBoundingArea) {
        this.RenderCount++;
        if (SleepAmountInMilliseconds > 0) {
            Thread.Sleep(SleepAmountInMilliseconds);
        } 
    }
}