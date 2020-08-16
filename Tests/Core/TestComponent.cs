namespace Macabresoft.MonoGame.Tests.Core {

    using Macabresoft.MonoGame.Core;

    internal class TestComponent : BaseComponent {

        public TestComponent() {
        }

        public TestComponent(string name) {
            this.Name = name;
        }

        protected override void Initialize() {
            return;
        }
    }
}