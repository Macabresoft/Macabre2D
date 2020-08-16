namespace Macabresoft.MonoGame.Tests.Core {

    using Macabresoft.MonoGame.Core;
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