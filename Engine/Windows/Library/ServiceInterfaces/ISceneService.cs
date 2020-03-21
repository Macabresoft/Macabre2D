namespace Macabre2D.Engine.Windows.ServiceInterfaces {

    using Macabre2D.Engine.Windows.Models;
    using System.ComponentModel;
    using System.Threading.Tasks;

    public interface ISceneService : INotifyPropertyChanged {
        SceneAsset CurrentScene { get; }

        Task<SceneAsset> CreateScene(FolderAsset parentAsset, string name);

        Task<SceneAsset> LoadScene(Project project, SceneAsset asset);
    }
}