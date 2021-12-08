namespace Macabresoft.Macabre2D.Tests.Framework;

using System.Threading;
using Macabresoft.Macabre2D.Framework;

internal class TestUpdateableEntity : UpdateableEntity {
    public int UpdateCount { get; private set; }

    public override void Update(FrameTime frameTime, InputState inputState) {
        this.UpdateCount++;
        Thread.Sleep(10);
    }
}