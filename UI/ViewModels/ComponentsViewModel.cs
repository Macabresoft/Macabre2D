namespace Macabre2D.UI.ViewModels {

    using Macabre2D.Framework;
    using Macabre2D.UI.ServiceInterfaces;

    public sealed class ComponentsViewModel : NotifyPropertyChanged {

        public ComponentsViewModel(ISceneService sceneService, IComponentService componentService) {
            this.SceneService = sceneService;
            this.ComponentService = componentService;
        }

        public IComponentService ComponentService { get; }
        public ISceneService SceneService { get; }
    }
}