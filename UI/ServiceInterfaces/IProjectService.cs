namespace Macabre2D.UI.ServiceInterfaces {

    using Macabre2D.UI.Models;
    using System.ComponentModel;
    using System.Threading.Tasks;

    public interface IProjectService : INotifyPropertyChanged {
        Project CurrentProject { get; }

        bool HasChanges { get; set; }

        Task<bool> BuildContent(BuildMode mode);

        Task<bool> BuildProject(BuildMode mode);

        Task<Project> CreateProject(string initialDirectory = null);

        string GetBinPath(bool debug);

        string GetSourcePath();

        Task<Project> LoadProject(string location);

        void NavigateToProjectLocation();

        void OpenProjectInCodeEditor();

        Task<bool> SaveProject();

        Task<Project> SelectAndLoadProject(string initialDirectory = null);
    }
}