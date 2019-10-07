namespace Macabre2D.UI.ServiceInterfaces {

    using Macabre2D.UI.Models;
    using System.ComponentModel;
    using System.Threading.Tasks;

    public interface ISceneService : INotifyPropertyChanged {
        SceneAsset CurrentScene { get; }

        bool HasChanges { get; set; }

        Task<SceneAsset> CreateScene(FolderAsset parentAsset, string name);

        Task<SceneAsset> LoadScene(Project project, SceneAsset asset);
    }
}