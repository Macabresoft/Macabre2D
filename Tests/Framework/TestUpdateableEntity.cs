namespace Macabre2D.Tests.Framework;

using System.Threading;
using Macabre2D.Framework;

internal class TestUpdateableEntity : UpdateableEntity {
    public int SleepAmountInMilliseconds { get; set; } = 10;
    public int UpdateCount { get; private set; }

    public override void Update(FrameTime frameTime, InputState inputState) {
        this.UpdateCount++;

        if (this.SleepAmountInMilliseconds > 0) {
            Thread.Sleep(this.SleepAmountInMilliseconds);
        }
    }
}