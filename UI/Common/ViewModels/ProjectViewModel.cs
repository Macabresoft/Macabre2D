namespace Macabresoft.Macabre2D.UI.Common.ViewModels {
    using Macabresoft.Macabre2D.UI.Common.Services;
    using Unity;

    /// <summary>
    /// View model for editing the project.
    /// </summary>
    public class ProjectViewModel : BaseViewModel {

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectViewModel" /> class.
        /// </summary>
        /// <remarks>This constructor only exists for design time XAML.</remarks>
        public ProjectViewModel() : base() {
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectViewModel" /> class.
        /// </summary>
        /// <param name="projectService">The project service.</param>
        [InjectionConstructor]
        public ProjectViewModel(IProjectService projectService) : base() {
            this.ProjectService = projectService;
        }
        
        /// <summary>
        /// Gets the project service.
        /// </summary>
        public IProjectService ProjectService { get; }
    }
}