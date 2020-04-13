namespace Macabre2D.UI.GameEditor.ViewModels {

    using GalaSoft.MvvmLight.CommandWpf;
    using Macabre2D.Framework;
    using Macabre2D.UI.Library.Models;
    using Macabre2D.UI.Library.Services;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Input;

    public enum TabTypes {
        Settings,

        Assets,

        Components,

        Modules
    }

    public sealed class MainWindowViewModel : NotifyPropertyChanged {
        private readonly IDialogService _dialogService;
        private readonly RelayCommand _redoCommand;
        private readonly RelayCommand _undoCommand;
        private readonly IUndoService _undoService;
        private TabTypes _selectedTabType;

        public MainWindowViewModel(
            IBusyService busyService,
            IDialogService dialogService,
            IProjectService projectService,
            ISceneService sceneService,
            IUndoService undoService) {
            this.BusyService = busyService;
            this._dialogService = dialogService;
            this.ProjectService = projectService;
            this.SceneService = sceneService;
            this._undoService = undoService;

            this.RefreshAssembliesCommand = new RelayCommand(this.RefreshAssemblies);
            this.OpenProjectInFileExplorer = new RelayCommand(this.ProjectService.NavigateToProjectLocation);
            this.RefreshAssetsCommand = new RelayCommand(async () => await this.RefreshAssets());
            this.SaveProjectCommand = new RelayCommand(async () => await this.SaveProject(), () => this.ProjectService.HasChanges);
            this._undoCommand = new RelayCommand(() => this._undoService.Undo(), () => this._undoService.CanUndo);
            this._redoCommand = new RelayCommand(() => this._undoService.Redo(), () => this._undoService.CanRedo);
            this._undoService.PropertyChanged += this.UndoService_PropertyChanged;
        }

        public IBusyService BusyService { get; }

        public ICommand OpenProjectInFileExplorer { get; }

        public IProjectService ProjectService { get; }

        public ICommand RedoCommand {
            get {
                return this._redoCommand;
            }
        }

        public ICommand RefreshAssembliesCommand { get; }

        public ICommand RefreshAssetsCommand { get; }

        public ICommand SaveProjectCommand { get; }

        public ISceneService SceneService { get; }

        public TabTypes SelectedTabType {
            get {
                return this._selectedTabType;
            }

            set {
                this.Set(ref this._selectedTabType, value);
            }
        }

        public ICommand UndoCommand {
            get {
                return this._undoCommand;
            }
        }

        private void RefreshAssemblies() {
            var saveResult = this._dialogService.ShowSaveDiscardCancelDialog();
            if (saveResult != SaveDiscardCancelResult.Cancel) {
                Process.Start(Application.ResourceAssembly.Location);
                Application.Current.Shutdown();
            }
        }

        private async Task RefreshAssets() {
            var task = this.ProjectService.BuildAllAssets(BuildMode.Debug);
            await this.BusyService.PerformTask(task, true);
        }

        private async Task SaveProject() {
            var task = this.ProjectService.SaveProject();
            await this.BusyService.PerformTask(task, true);
        }

        private void UndoService_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(this._undoService.CanRedo)) {
                this._redoCommand.RaiseCanExecuteChanged();
            }
            else if (e.PropertyName == nameof(this._undoService.CanUndo)) {
                this._undoCommand.RaiseCanExecuteChanged();
            }
        }
    }
}