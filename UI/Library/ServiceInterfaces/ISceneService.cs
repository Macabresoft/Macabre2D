namespace Macabre2D.UI.Library.ServiceInterfaces {

    using Macabre2D.UI.Library.Models;
    using System.ComponentModel;
    using System.Threading.Tasks;

    public interface ISceneService : INotifyPropertyChanged {
        SceneAsset CurrentScene { get; }

        Task<SceneAsset> CreateScene(FolderAsset parentAsset, string name);

        Task<SceneAsset> LoadScene(Project project, SceneAsset asset);
    }
}