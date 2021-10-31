namespace Macabresoft.Macabre2D.UI.Editor {
    using Macabresoft.Macabre2D.UI.Common;

    /// <summary>
    /// View model for project editing.
    /// </summary>
    public class ProjectEditorViewModel : BaseViewModel {
        
        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectEditorViewModel" /> class.
        /// </summary>
        /// <param name="selectionService">The selection service.</param>
        public ProjectEditorViewModel(IProjectSelectionService selectionService) : base() {
            this.SelectionService = selectionService;
        }
        
        /// <summary>
        /// Gets the selection service.
        /// </summary>
        public IProjectSelectionService SelectionService { get; }
    }
}