namespace Macabre2D.UI.SongEditor {

    using Macabre2D.UI.SongEditor.Properties;
    using Macabre2D.UI.Library.Services;

    public sealed class SettingsManager {
        private readonly IAutoSaveService _autoSaveService;
        private readonly IMonoGameService _monoGameService;

        public SettingsManager(IAutoSaveService autoSaveService, IMonoGameService monoGameService) {
            this._autoSaveService = autoSaveService;
            this._monoGameService = monoGameService;
        }

        public string GetLastOpenTabName() {
            return Settings.Default.LastTab;
        }

        public void Initialize() {
            this._autoSaveService.AutoSaveIntervalInMinutes = Settings.Default.AutoSaveIntervalInMinutes;
            this._autoSaveService.NumberOfAutoSaves = Settings.Default.NumberOfAutoSaves;
            this._monoGameService.ShowGrid = Settings.Default.ShowGrid;
            this._monoGameService.ShowSelection = Settings.Default.ShowSelection;
        }

        public void Save(string openedTabName) {
            Settings.Default.AutoSaveIntervalInMinutes = this._autoSaveService.AutoSaveIntervalInMinutes;
            Settings.Default.NumberOfAutoSaves = this._autoSaveService.NumberOfAutoSaves;
            Settings.Default.LastTab = openedTabName;
            Settings.Default.ShowGrid = this._monoGameService.ShowGrid;
            Settings.Default.ShowSelection = this._monoGameService.ShowSelection;
            Settings.Default.Save();
        }
    }
}