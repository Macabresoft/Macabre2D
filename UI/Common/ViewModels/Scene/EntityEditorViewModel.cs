namespace Macabresoft.Macabre2D.UI.Common.ViewModels.Scene {
    using Macabresoft.Macabre2D.UI.Common.Services;
    using Unity;

    public class EntityEditorViewModel : ViewModelBase {
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityEditorViewModel" /> class.
        /// </summary>
        public EntityEditorViewModel() {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityEditorViewModel" /> class.
        /// </summary>
        /// <param name="selectionService">The selection service.</param>
        [InjectionConstructor]
        public EntityEditorViewModel(ISelectionService selectionService) {
            this.SelectionService = selectionService;
        }

        /// <summary>
        /// Gets the selection service.
        /// </summary>
        public ISelectionService SelectionService { get; }
    }
}