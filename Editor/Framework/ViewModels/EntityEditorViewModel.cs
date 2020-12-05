namespace Macabresoft.Macabre2D.Editor.Library.ViewModels {
    using Macabresoft.Macabre2D.Editor.Library.Services;
    using Unity;

    public class EntityEditorViewModel : ViewModelBase {
        private readonly ISelectionService _selectionService;

        public EntityEditorViewModel() {
        }

        [InjectionConstructor]
        public EntityEditorViewModel(ISelectionService selectionService) {
            this._selectionService = selectionService;
        }
    }
}