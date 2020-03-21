namespace Macabre2D.Engine.Windows.ViewModels {

    using Macabre2D.Framework;
    using Macabre2D.Engine.Windows.ServiceInterfaces;

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