namespace Macabre2D.UI.Editor {

    using Macabre2D.UI.Editor.Properties;
    using Macabre2D.UI.ServiceInterfaces;
    using System.IO;
    using System.Threading.Tasks;

    public sealed class SettingsManager {
        private const string LastProjectSettingName = "LastProject";

        private readonly IMonoGameService _monoGameService;
        private readonly IProjectService _projectService;
        private readonly ISceneService _sceneService;

        public SettingsManager(IMonoGameService monoGameService, IProjectService projectService, ISceneService sceneService) {
            this._monoGameService = monoGameService;
            this._projectService = projectService;
            this._sceneService = sceneService;
        }

        public string GetLastOpenTabName() {
            return Settings.Default.LastTab;
        }

        public void Initialize() {
            this._monoGameService.ShowGrid = Settings.Default.ShowGrid;
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
            Settings.Default.Save();
        }
    }
}