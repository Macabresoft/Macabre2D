namespace Macabresoft.Macabre2D.UI.Editor {
    using Macabresoft.Macabre2D.UI.Common;

    /// <summary>
    /// View model for project editing.
    /// </summary>
    public class ProjectEditorViewModel : BaseViewModel {
        
        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectEditorViewModel" /> class.
        /// </summary>
        /// <param name="projectService">The project service.</param>
        public ProjectEditorViewModel(IProjectService projectService) : base() {
            this.ProjectService = projectService;
        }
        
        /// <summary>
        /// Gets the selection service.
        /// </summary>
        public IProjectService ProjectService { get; }
    }
}