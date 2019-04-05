namespace Macabre2D.UI.ViewModels {

    using Macabre2D.Framework;
    using Macabre2D.UI.ServiceInterfaces;

    public sealed class ComponentsViewModel : NotifyPropertyChanged {

        public ComponentsViewModel(ISceneService sceneService, IComponentSelectionService selectionService) {
            this.SceneService = sceneService;
            this.SelectionService = selectionService;
        }

        public ISceneService SceneService { get; }

        public IComponentSelectionService SelectionService { get; }
    }
}