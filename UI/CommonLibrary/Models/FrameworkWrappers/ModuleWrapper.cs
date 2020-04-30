namespace Macabre2D.UI.CommonLibrary.Models.FrameworkWrappers {

    using Macabre2D.Framework;
    using Macabre2D.UI.CommonLibrary.Common;

    public sealed class ModuleWrapper : NotifyPropertyChanged {

        public ModuleWrapper(BaseModule module) {
            this.Module = module;
        }

        public BaseModule Module { get; }

        public string Name {
            get {
                return this.Module.Name;
            }

            set {
                // TODO: this is bad, undo service not being called
                this.Module.Name = value;
                this.RaisePropertyChanged();
            }
        }

        public void UpdateProperty(string pathToProperty, object newValue) {
            this.Module.SetProperty(pathToProperty, newValue);
            this.RaisePropertyChanged(pathToProperty);
        }
    }
}