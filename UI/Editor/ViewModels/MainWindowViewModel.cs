namespace Macabresoft.Macabre2D.UI.Editor {
    using System;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using Avalonia.Threading;
    using Macabresoft.Core;
    using Macabresoft.Macabre2D.Framework;
    using Macabresoft.Macabre2D.UI.Common;
    using ReactiveUI;
    using Unity;

    /// <summary>
    /// The view model for the main window.
    /// </summary>
    public class MainWindowViewModel : UndoBaseViewModel {
        private readonly ILocalDialogService _dialogService;
        private readonly ISaveService _saveService;
        private readonly ISceneService _sceneService;
        private readonly IEditorSettingsService _settingsService;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindowViewModel" /> class.
        /// </summary>
        /// <remarks>This constructor only exists for design time XAML.</remarks>
        public MainWindowViewModel() : base() {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindowViewModel" /> class.
        /// </summary>
        /// <param name="dialogService">The dialog service.</param>
        /// <param name="editorService">The editor service.</param>
        /// <param name="saveService">The save service.</param>
        /// <param name="sceneService">The scene service.</param>
        /// <param name="settingsService">The editor settings service.</param>
        /// <param name="undoService">The undo service.</param>
        [InjectionConstructor]
        public MainWindowViewModel(
            ILocalDialogService dialogService,
            IEditorService editorService,
            ISaveService saveService,
            ISceneService sceneService,
            IEditorSettingsService settingsService,
            IUndoService undoService) : base(undoService) {
            this._dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            this.EditorService = editorService ?? throw new ArgumentNullException(nameof(editorService));
            this._saveService = saveService ?? throw new ArgumentNullException(nameof(saveService));
            this._sceneService = sceneService ?? throw new ArgumentNullException(nameof(sceneService));
            this._settingsService = settingsService ?? throw new ArgumentNullException(nameof(settingsService));

            this.ExitCommand = ReactiveCommand.CreateFromTask<IWindow>(this.Exit);
            this.OpenSceneCommand = ReactiveCommand.CreateFromTask(this.OpenScene);
            this.SaveCommand = ReactiveCommand.Create(this._saveService.Save, this._saveService.WhenAnyValue(x => x.HasChanges));
            this.SelectTabCommand = ReactiveCommand.Create<EditorTabs>(this.SelectTab);
            this.ToggleTabCommand = ReactiveCommand.Create(this.ToggleTab);
            this.ViewLicensesCommand = ReactiveCommand.CreateFromTask(this.ViewLicenses);
            this.ViewSourceCommand = ReactiveCommand.Create(ViewSource);
        }

        /// <summary>
        /// Gets the editor service.
        /// </summary>
        public IEditorService EditorService { get; }

        /// <summary>
        /// Gets the command to exit the application.
        /// </summary>
        public ICommand ExitCommand { get; }

        /// <summary>
        /// Gets the open scene command.
        /// </summary>
        public ICommand OpenSceneCommand { get; }

        /// <summary>
        /// Gets the command to save the current scene.
        /// </summary>
        public ICommand SaveCommand { get; }

        /// <summary>
        /// Gets the command to select a tab.
        /// </summary>
        public ICommand SelectTabCommand { get; }

        /// <summary>
        /// Gets the command to toggle the selected tab.
        /// </summary>
        public ICommand ToggleTabCommand { get; }

        /// <summary>
        /// Gets the command to view licenses.
        /// </summary>
        public ICommand ViewLicensesCommand { get; }

        /// <summary>
        /// Gets the command to view the source code.
        /// </summary>
        public ICommand ViewSourceCommand { get; }


        /// <summary>
        /// Gets a value indicating whether or not the window should close.
        /// </summary>
        /// <returns>A value indicating whether or not the window should close.</returns>
        public async Task<YesNoCancelResult> TryClose() {
            var result = await this._saveService.RequestSave();
            if (result != YesNoCancelResult.Cancel) {
                this._settingsService.Save();
            }

            return result;
        }

        private async Task Exit(IWindow window) {
            if (window != null && await this.TryClose() != YesNoCancelResult.Cancel) {
                Dispatcher.UIThread.Post(window.Close);
            }
        }

        private async Task OpenScene() {
            var saveResult = await this._saveService.RequestSave();

            if (saveResult != YesNoCancelResult.Cancel) {
                var result = await this._dialogService.OpenAssetSelectionDialog(typeof(SceneAsset), false);

                if (result != null && !this._sceneService.TryLoadScene(result.Id, out _)) {
                    await this._dialogService.ShowWarningDialog("Error", "The scene could not be loaded");
                }
            }
        }

        private void SelectTab(EditorTabs tab) {
            this.EditorService.SelectedTab = tab;
        }

        private void ToggleTab() {
            this.SelectTab(this.EditorService.SelectedTab == EditorTabs.Scene ? EditorTabs.Project : EditorTabs.Scene);
        }

        private async Task ViewLicenses() {
            await this._dialogService.OpenLicenseDialog();
        }

        private static void ViewSource() {
            WebHelper.OpenInBrowser("https://github.com/Macabresoft/Macabre2D");
        }
    }
}