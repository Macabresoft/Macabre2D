namespace Macabresoft.Macabre2D.Tests.Framework;

using System.Threading;
using Macabresoft.Macabre2D.Framework;

internal class TestUpdateableEntity : UpdateableEntity {
    public int UpdateCount { get; private set; }

    public int SleepAmountInMilliseconds { get; set; } = 10;

    public override void Update(FrameTime frameTime, InputState inputState) {
        this.UpdateCount++;
        
        if (SleepAmountInMilliseconds > 0) {
            Thread.Sleep(SleepAmountInMilliseconds);
        }
    }
}