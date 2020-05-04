namespace Macabre2D.UI.CosmicSynth {

    using Macabre2D.UI.CosmicSynth.Properties;

    public sealed class SettingsManager {

        public void Initialize() {
            //this._autoSaveService.AutoSaveIntervalInMinutes = Settings.Default.AutoSaveIntervalInMinutes;
            //this._autoSaveService.NumberOfAutoSaves = Settings.Default.NumberOfAutoSaves;
        }

        public void Save() {
            //Settings.Default.AutoSaveIntervalInMinutes = this._autoSaveService.AutoSaveIntervalInMinutes;
            //Settings.Default.NumberOfAutoSaves = this._autoSaveService.NumberOfAutoSaves;
            Settings.Default.Save();
        }
    }
}