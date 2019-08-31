namespace Macabre2D.UI.Editor {

    using Macabre2D.UI.Editor.Properties;
    using Macabre2D.UI.ServiceInterfaces;

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

        public void Save(string openedTabName) {
            Settings.Default.LastTab = openedTabName;
            Settings.Default.ShowGrid = this._monoGameService.ShowGrid;
            Settings.Default.ShowSelection = this._monoGameService.ShowSelection;
            Settings.Default.Save();
        }
    }
}