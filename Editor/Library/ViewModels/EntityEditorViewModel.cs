namespace Macabresoft.Macabre2D.Editor.Library.ViewModels {
    using Macabresoft.Macabre2D.Editor.Library.Services;
    using Unity;

    public class EntityEditorViewModel : ViewModelBase {

        public EntityEditorViewModel() {
        }

        [InjectionConstructor]
        public EntityEditorViewModel(ISelectionService selectionService) {
            this.SelectionService = selectionService;
        }
        
        public ISelectionService SelectionService { get; }
    }
}