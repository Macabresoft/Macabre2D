namespace Macabre2D.UI.CommonLibrary.Services {

    using Macabre2D.Framework;
    using System;
    using System.Windows.Threading;

    public interface IAutoSaveService {
        byte AutoSaveIntervalInMinutes { get; set; }

        byte NumberOfAutoSaves { get; set; }

        void Initialize(byte numberOfAutoSaves, byte autoSaveIntervalInMinutes);
    }

    public sealed class AutoSaveService : NotifyPropertyChanged, IAutoSaveService {
        private readonly DispatcherTimer _autoSaveTimer = new DispatcherTimer();
        private readonly IProjectService _projectService;
        private byte _autoSaveIntervalInMinutes;
        private bool _isInitialized;
        private byte _numberOfAutoSaves;

        public AutoSaveService(IProjectService projectService) {
            this._projectService = projectService;
        }

        public byte AutoSaveIntervalInMinutes {
            get {
                return this._autoSaveIntervalInMinutes;
            }

            set {
                if (this.Set(ref this._autoSaveIntervalInMinutes, value)) {
                    this.Reset(this._numberOfAutoSaves, this._autoSaveIntervalInMinutes);
                }
            }
        }

        public byte NumberOfAutoSaves {
            get {
                return this._numberOfAutoSaves;
            }

            set {
                if (this.Set(ref this._numberOfAutoSaves, value)) {
                    this.Reset(this._numberOfAutoSaves, this._autoSaveIntervalInMinutes);
                }
            }
        }

        public void Initialize(byte numberOfAutoSaves, byte autoSaveIntervalInMinutes) {
            if (!this._isInitialized) {
                try {
                    this._autoSaveTimer.Tick += this.AutoSaveTimer_Tick;
                    this.Reset(numberOfAutoSaves, autoSaveIntervalInMinutes);
                    this.RaisePropertyChanged(nameof(this.AutoSaveIntervalInMinutes));
                    this.RaisePropertyChanged(nameof(this.NumberOfAutoSaves));
                }
                finally {
                    this._isInitialized = true;
                }
            }
        }

        public void Reset(byte numberOfAutoSaves, byte autoSaveIntervalInMinutes) {
            this._numberOfAutoSaves = numberOfAutoSaves;
            this._autoSaveIntervalInMinutes = autoSaveIntervalInMinutes;
            this._autoSaveTimer.Stop();

            if (autoSaveIntervalInMinutes > 0) {
                this._autoSaveTimer.Interval = new TimeSpan(0, this._autoSaveIntervalInMinutes, 0);
                this._autoSaveTimer.Start();
            }
        }

        private async void AutoSaveTimer_Tick(object sender, EventArgs e) {
            if (this._projectService.CurrentProject != null) {
                await this._projectService.AutoSaveProject(this._numberOfAutoSaves, true);
            }
        }
    }
}