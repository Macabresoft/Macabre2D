namespace Macabre2D.UI.ViewModels {

    using Macabre2D.Framework;
    using Macabre2D.UI.Models.FrameworkWrappers;
    using Macabre2D.UI.ServiceInterfaces;

    public sealed class ComponentsViewModel : NotifyPropertyChanged {
        private readonly ISelectionService _selectionService;
        private ComponentWrapper _selectedComponent;

        public ComponentsViewModel(ISceneService sceneService, ISelectionService selectionService) {
            this.SceneService = sceneService;
            this._selectionService = selectionService;
        }

        public ISceneService SceneService { get; }

        public ComponentWrapper SelectedComponent {
            get {
                return this._selectedComponent;
            }
            set {
                if (this.Set(ref this._selectedComponent, value)) {
                    this._selectionService.SelectedItem = this._selectedComponent;
                }
            }
        }
    }
}