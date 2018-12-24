namespace Macabre2D.UI.ViewModels {

    using GalaSoft.MvvmLight.CommandWpf;
    using Macabre2D.Framework;
    using Macabre2D.UI.Common;
    using Macabre2D.UI.Models;
    using Macabre2D.UI.ServiceInterfaces;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Input;

    public enum TabTypes {
        Project,

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
            IMonoGameService monoGameService,
            IProjectService projectService,
            ISceneService sceneService,
            IUndoService undoService) {
            this.BusyService = busyService;
            this._dialogService = dialogService;
            this.MonoGameService = monoGameService;
            this.ProjectService = projectService;
            this.SceneService = sceneService;
            this._undoService = undoService;

            this.RefreshAssembliesCommand = new RelayCommand(this.RefreshAssemblies);
            this.CreateProjectCommand = new RelayCommand(async () => await this.CreateProject());
            this.OpenProjectCommand = new RelayCommand(async () => await this.OpenProject());
            this.OpenProjectInCodeEditorCommand = new RelayCommand(this.ProjectService.OpenProjectInCodeEditor);
            this.OpenProjectInFileExplorer = new RelayCommand(this.ProjectService.NavigateToProjectLocation);
            this.RefreshAssetsCommand = new RelayCommand(async () => await this.RefreshAssets());
            this.SaveProjectCommand = new RelayCommand(async () => await this.SaveProject(), () => this.ProjectService.HasChanges);
            this._undoCommand = new RelayCommand(() => this._undoService.Undo(), () => this._undoService.CanUndo);
            this._redoCommand = new RelayCommand(() => this._undoService.Redo(), () => this._undoService.CanRedo);
            this._undoService.PropertyChanged += this.UndoService_PropertyChanged;
        }

        public IBusyService BusyService { get; }

        public ICommand CreateProjectCommand { get; }

        public IMonoGameService MonoGameService { get; }

        public ICommand OpenProjectCommand { get; }

        public ICommand OpenProjectInCodeEditorCommand { get; }

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

        private async Task CreateProject() {
            await this.BusyService.PerformTask(this.ProjectService.CreateProject(FileHelper.DefaultProjectPath));
        }

        private async Task OpenProject() {
            await this.BusyService.PerformTask(this.ProjectService.SelectAndLoadProject(FileHelper.DefaultProjectPath));
        }

        private void RefreshAssemblies() {
            var saveResult = this._dialogService.ShowSaveDiscardCancelDialog();
            if (saveResult != SaveDiscardCancelResult.Cancel) {
                Process.Start(Application.ResourceAssembly.Location);
                Application.Current.Shutdown();
            }
        }

        private async Task RefreshAssets() {
            await this.BusyService.PerformTask(this.ProjectService.BuildContent());
        }

        private async Task SaveProject() {
            await this.BusyService.PerformTask(this.ProjectService.SaveProject());
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