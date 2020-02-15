using Macabre2D.Framework;

namespace Macabre2D.Tests {

    internal sealed class ModuleOwningTestComponent : TestComponent {
        public BaseModule Module { get; private set; }

        protected override void Initialize() {
            base.Initialize();
            this.Module = new SimplePhysicsModule();
            this.Scene.AddModule(this.Module);
        }
    }
}