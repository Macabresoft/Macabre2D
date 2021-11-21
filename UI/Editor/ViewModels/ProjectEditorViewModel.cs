namespace Macabresoft.Macabre2D.UI.Editor {
    using Avalonia;
    using Macabresoft.Macabre2D.UI.AvaloniaInterop;
    using Macabresoft.Macabre2D.UI.Common;

    /// <summary>
    /// View model for project editing.
    /// </summary>
    public class ProjectEditorViewModel : MonoGameViewModel {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectEditorViewModel" /> class.
        /// </summary>
        /// <param name="projectEditor">The project editor.</param>
        /// <param name="projectService">The project service.</param>
        public ProjectEditorViewModel(
            IProjectEditorGame projectEditor,
            IProjectService projectService) : base(projectEditor) {
            this.ProjectService = projectService;
        }
        
        /// <summary>
        /// Gets the selection service.
        /// </summary>
        public IProjectService ProjectService { get; }
    }
}