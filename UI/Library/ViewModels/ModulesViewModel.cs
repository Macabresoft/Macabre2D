namespace Macabre2D.UI.Library.ViewModels {

    using Macabre2D.Framework;
    using Macabre2D.UI.Library.Models.FrameworkWrappers;
    using Macabre2D.UI.Library.Services;

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