namespace Macabre2D.UI.GameEditor.ViewModels {

    using Macabre2D.Framework;
    using Macabre2D.UI.CommonLibrary.Services;
    using Macabre2D.UI.GameEditorLibrary.Services;

    public sealed class ModulesViewModel : NotifyPropertyChanged {
        private BaseModule _selectedModule;

        public ModulesViewModel(IBusyService busyService, ISceneService sceneService) {
            this.BusyService = busyService;
            this.SceneService = sceneService;
        }

        public IBusyService BusyService { get; }

        public ISceneService SceneService { get; }

        public BaseModule SelectedModule {
            get {
                return this._selectedModule;
            }
            set {
                this.Set(ref this._selectedModule, value);
            }
        }
    }
}