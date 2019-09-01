namespace Macabre2D.UI.ServiceInterfaces {

    using Macabre2D.UI.Models;
    using System.ComponentModel;
    using System.Threading.Tasks;

    public interface IProjectService : INotifyPropertyChanged {
        Project CurrentProject { get; }

        bool HasChanges { get; set; }

        Task<bool> BuildAllAssets(BuildMode mode);

        string GetPathToProject();

        Task<Project> LoadProject();

        void NavigateToProjectLocation();

        Task<bool> SaveProject();
    }
}