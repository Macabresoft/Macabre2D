namespace Macabresoft.Macabre2D.Editor.Library.ViewModels {
    using System.Collections.Generic;
    using System.ComponentModel;
    using Macabresoft.Macabre2D.Editor.Library.Models;
    using Macabresoft.Macabre2D.Editor.Library.Services;
    using Unity;

    public class EntityEditorViewModel : ViewModelBase {

        public EntityEditorViewModel() {
        }

        [InjectionConstructor]
        public EntityEditorViewModel(ISelectionService selectionService) {
            this.SelectionService = selectionService;
            this.SelectionService.PropertyChanged += SelectionService_PropertyChanged;
        }

        private void SelectionService_PropertyChanged(object sender, PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(ISelectionService.SelectedEntity)) {
                
            }
        }

        public ISelectionService SelectionService { get; }

        public IReadOnlyCollection<ValueEditorCollection> ComponentEditors { get; }
    }
}