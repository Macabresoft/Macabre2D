namespace Macabresoft.MonoGame.Tests.Core {

    using Macabresoft.MonoGame.Core;

    internal sealed class ModuleOwningTestComponent : TestComponent {
        public BaseModule Module { get; private set; }

        protected override void Initialize() {
            base.Initialize();
            this.Module = new SimplePhysicsModule();
            this.Scene.AddModule(this.Module);
        }
    }
}