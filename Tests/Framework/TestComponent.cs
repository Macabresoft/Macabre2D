namespace Macabre2D.Tests.Framework {

    using Macabre2D.Framework;

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