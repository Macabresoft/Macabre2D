namespace Macabre2D.UI.CosmicSynth {

    using Macabre2D.UI.CosmicSynth.Properties;
    using Macabre2D.UI.Library.Services;

    public sealed class SettingsManager {
        private readonly IAutoSaveService _autoSaveService;
        private readonly IMonoGameService _monoGameService;

        public SettingsManager(IAutoSaveService autoSaveService, IMonoGameService monoGameService) {
            this._autoSaveService = autoSaveService;
            this._monoGameService = monoGameService;
        }

        public void Initialize() {
            this._autoSaveService.AutoSaveIntervalInMinutes = Settings.Default.AutoSaveIntervalInMinutes;
            this._autoSaveService.NumberOfAutoSaves = Settings.Default.NumberOfAutoSaves;
        }

        public void Save() {
            Settings.Default.AutoSaveIntervalInMinutes = this._autoSaveService.AutoSaveIntervalInMinutes;
            Settings.Default.NumberOfAutoSaves = this._autoSaveService.NumberOfAutoSaves;
            Settings.Default.Save();
        }
    }
}