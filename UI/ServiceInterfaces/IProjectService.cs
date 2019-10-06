namespace Macabre2D.UI.ServiceInterfaces {

    using Macabre2D.UI.Models;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Threading.Tasks;

    public interface IProjectService : INotifyPropertyChanged {
        Project CurrentProject { get; }

        bool HasChanges { get; set; }

        Task<bool> AutoSaveProject(int maxAutoSaves, bool purgeExcessAutoSaves);

        Task<bool> BuildAllAssets(BuildMode mode);

        IEnumerable<string> GetAutoSaveFiles();

        string GetPathToProject();

        Task<Project> LoadProject(string pathToProject);

        Task<Project> LoadProject();

        void NavigateToProjectLocation();

        Task<bool> SaveProject();
    }
}