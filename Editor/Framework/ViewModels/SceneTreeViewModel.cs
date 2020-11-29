namespace Macabresoft.Macabre2D.Editor.Library.ViewModels {
    using Macabresoft.Macabre2D.Editor.Library.Services;

    /// <summary>
    /// A view model for the scene tree.
    /// </summary>
    public class SceneTreeViewModel : ViewModelBase {
        
        /// <summary>
        /// Initializes a new instance of the <see cref="SceneTreeViewModel" /> class.
        /// </summary>
        /// <param name="sceneService">The scene service.</param>
        public SceneTreeViewModel(ISceneService sceneService) {
            this.SceneService = sceneService;
        }
        
        /// <summary>
        /// Gets the scene service.
        /// </summary>
        public ISceneService SceneService { get; }
    }
}