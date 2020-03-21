namespace Macabre2D.Engine.Windows.ViewModels {

    using Macabre2D.Framework;
    using Macabre2D.Engine.Windows.Models.FrameworkWrappers;
    using Macabre2D.Engine.Windows.ServiceInterfaces;

    public sealed class ModulesViewModel : NotifyPropertyChanged {
        private ModuleWrapper _selectedModule;

        public ModulesViewModel(IBusyService busyService, ISceneService sceneService) {
            this.BusyService = busyService;
            this.SceneService = sceneService;
        }

        public IBusyService BusyService { get; }

        public ISceneService SceneService { get; }

        public ModuleWrapper SelectedModule {
            get {
                return this._selectedModule;
            }
            set {
                this.Set(ref this._selectedModule, value);
            }
        }
    }
}