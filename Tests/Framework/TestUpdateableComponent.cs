namespace Macabresoft.Macabre2D.Tests.Framework {

    using Macabresoft.Macabre2D.Framework;
    using System.Threading;

    internal class TestUpdateableComponent : GameUpdateableComponent {

        public int UpdateCount {
            get;
            private set;
        }

        public override void Update(FrameTime frameTime, InputState inputState) {
            this.UpdateCount++;
            Thread.Sleep(10);
        }
    }
}