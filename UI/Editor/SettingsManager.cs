namespace Macabre2D.UI.Editor {

    using Macabre2D.UI.Editor.Properties;
    using Macabre2D.UI.ServiceInterfaces;
    using System.IO;
    using System.Threading.Tasks;

    public sealed class SettingsManager {
        private readonly IMonoGameService _monoGameService;
        private readonly IProjectService _projectService;

        public SettingsManager(IMonoGameService monoGameService, IProjectService projectService) {
            this._monoGameService = monoGameService;
            this._projectService = projectService;
        }

        public string GetLastOpenTabName() {
            return Settings.Default.LastTab;
        }

        public void Initialize() {
            this._monoGameService.ShowGrid = Settings.Default.ShowGrid;
            this._monoGameService.ShowSelection = Settings.Default.ShowSelection;
        }

        public async Task LoadLastProjectOpened() {
            var path = Settings.Default.LastProject;
            if (!string.IsNullOrWhiteSpace(path)) {
                if (File.Exists(path)) {
                    await this._projectService.LoadProject(path);
                }
            }
        }

        public void Save(string openedTabName) {
            if (this._projectService.CurrentProject != null) {
                Settings.Default.LastProject = this._projectService.CurrentProject.PathToProject;
            }
            else {
                Settings.Default.LastProject = string.Empty;
            }

            Settings.Default.LastTab = openedTabName;
            Settings.Default.ShowGrid = this._monoGameService.ShowGrid;
            Settings.Default.ShowSelection = this._monoGameService.ShowSelection;
            Settings.Default.Save();
        }
    }
}