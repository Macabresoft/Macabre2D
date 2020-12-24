namespace Macabresoft.Macabre2D.Editor.Library.ViewModels {
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Threading.Tasks;
    using Macabresoft.Macabre2D.Editor.Library.Models;
    using Macabresoft.Macabre2D.Editor.Library.Services;
    using Macabresoft.Macabre2D.Framework;
    using Unity;

    public class EntityEditorViewModel : ViewModelBase {
        private readonly object _editorsLock = new object();

        public EntityEditorViewModel() {
        }

        [InjectionConstructor]
        public EntityEditorViewModel(ISelectionService selectionService) {
            this.SelectionService = selectionService;
            this.SelectionService.PropertyChanged += this.SelectionService_PropertyChanged;
        }

        private async void SelectionService_PropertyChanged(object sender, PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(ISelectionService.SelectedEntity)) {
                await this.ResetComponentEditors();
            }
        }

        public ISelectionService SelectionService { get; }

        public IReadOnlyCollection<ValueEditorCollection> ComponentEditors { get; }

        private async Task ResetComponentEditors() {
            lock (this._editorsLock) {
                if (this.SelectionService.SelectedEntity is IGameScene scene) {
                
                }
                else if (this.SelectionService.SelectedEntity is IGameEntity entity) {
                
                }
            }

        }
    }
}