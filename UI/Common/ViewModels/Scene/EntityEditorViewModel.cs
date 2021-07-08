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
        /// <param name="entitySelectionService">The selection service.</param>
        [InjectionConstructor]
        public EntityEditorViewModel(IEntitySelectionService entitySelectionService) {
            this.EntitySelectionService = entitySelectionService;
        }

        /// <summary>
        /// Gets the selection service.
        /// </summary>
        public IEntitySelectionService EntitySelectionService { get; }
    }
}