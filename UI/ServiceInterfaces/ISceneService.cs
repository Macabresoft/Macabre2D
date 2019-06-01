namespace Macabre2D.UI.ServiceInterfaces {

    using Macabre2D.UI.Models;
    using Macabre2D.UI.Models.FrameworkWrappers;
    using System.ComponentModel;
    using System.Threading.Tasks;

    public interface ISceneService : INotifyPropertyChanged {
        SceneWrapper CurrentScene { get; }

        bool HasChanges { get; set; }

        Task<SceneWrapper> CreateScene(FolderAsset parentAsset, string name);

        Task<SceneWrapper> LoadScene(Project project, SceneAsset asset);

        Task<bool> SaveCurrentScene(Project project);
    }
}