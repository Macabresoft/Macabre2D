namespace Macabre2D.Tests {

    using Macabre2D.Framework;
    using System.Threading;

    internal class TestUpdateableComponent : TestComponent, IUpdateableComponent {

        public int UpdateCount {
            get;
            private set;
        }

        public void Update(FrameTime frameTime) {
            this.UpdateCount++;
            Thread.Sleep(10);
        }
    }
}