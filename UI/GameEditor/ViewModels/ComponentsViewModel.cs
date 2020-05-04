namespace Macabre2D.UI.GameEditor.ViewModels {

    using Macabre2D.Framework;
    using Macabre2D.UI.CommonLibrary.Services;
    using Macabre2D.UI.GameEditorLibrary.Services;

    public sealed class ComponentsViewModel : NotifyPropertyChanged {

        public ComponentsViewModel(
            IBusyService busyService,
            IComponentService componentService,
            ISceneService sceneService) {
            this.BusyService = busyService;
            this.ComponentService = componentService;
            this.SceneService = sceneService;
        }

        public IBusyService BusyService { get; }

        public IComponentService ComponentService { get; }

        public ISceneService SceneService { get; }
    }
}