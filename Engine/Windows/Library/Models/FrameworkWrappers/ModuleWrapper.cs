namespace Macabre2D.Engine.Windows.Models.FrameworkWrappers {

    using Macabre2D.Framework;
    using Macabre2D.Engine.Windows.Common;

    public sealed class ModuleWrapper : NotifyPropertyChanged {

        public ModuleWrapper(BaseModule module) {
            this.Module = module;
        }

        public string FullName {
            get {
                return this.Module.GetType().FullName;
            }
        }

        public BaseModule Module { get; }

        public string Name {
            get {
                return this.Module.GetType().Name;
            }
        }

        public void UpdateProperty(string pathToProperty, object newValue) {
            this.Module.SetProperty(pathToProperty, newValue);
            this.RaisePropertyChanged(pathToProperty);
        }
    }
}