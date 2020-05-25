namespace Macabre2D.Framework.Tests {

    using Macabre2D.Framework;
    using System.Threading;

    internal class TestUpdateableComponent : TestComponent, IUpdateableComponent {

        public int UpdateCount {
            get;
            private set;
        }

        public void Update(FrameTime frameTime, InputState inputState) {
            this.UpdateCount++;
            Thread.Sleep(10);
        }
    }
}