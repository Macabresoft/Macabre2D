namespace Macabresoft.Macabre2D.UI.Common.ViewModels {
    using System;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using Avalonia;
    using Avalonia.Controls;
    using Avalonia.Platform;
    using Macabresoft.Core;
    using Macabresoft.Macabre2D.Framework;
    using Macabresoft.Macabre2D.UI.Common.Models;
    using Macabresoft.Macabre2D.UI.Common.MonoGame;
    using Macabresoft.Macabre2D.UI.Common.Services;
    using ReactiveUI;
    using Unity;

    /// <summary>
    /// The view model for the main window.
    /// </summary>
    public class MainWindowViewModel : ViewModelBase {
        private readonly IDialogService _dialogService;
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
        /// <param name="contentService">The content service.</param>
        /// <param name="dialogService">The dialog service.</param>
        /// <param name="editorService">The editor service.</param>
        /// <param name="entityService">The selection service.</param>
        /// <param name="popupService">The popup service.</param>
        /// <param name="saveService">The save service.</param>
        /// <param name="sceneService">The scene service.</param>
        /// <param name="settingsService">The editor settings service.</param>
        /// <param name="systemService">The system service.</param>
        /// <param name="undoService">The undo service.</param>
        [InjectionConstructor]
        public MainWindowViewModel(
            IContentService contentService,
            IDialogService dialogService,
            IEditorService editorService,
            IEntityService entityService,
            IPopupService popupService,
            ISaveService saveService,
            ISceneService sceneService,
            IEditorSettingsService settingsService,
            ISystemService systemService,
            IUndoService undoService) : base() {
            this.ContentService = contentService ?? throw new ArgumentNullException(nameof(contentService));
            this._dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            this.EditorService = editorService ?? throw new ArgumentNullException(nameof(editorService));
            this.EntityService = entityService ?? throw new ArgumentNullException(nameof(entityService));
            this.PopupService = popupService ?? throw new ArgumentNullException(nameof(popupService));
            this.SaveService = saveService ?? throw new ArgumentNullException(nameof(saveService));
            this._sceneService = sceneService ?? throw new ArgumentNullException(nameof(sceneService));
            this._settingsService = settingsService ?? throw new ArgumentNullException(nameof(settingsService));
            this.SystemService = systemService ?? throw new ArgumentNullException(nameof(systemService));

            this.ClosePopupCommand = ReactiveCommand.Create(this.ClosePopup, this.PopupService.WhenAnyValue(x => x.IsPopupActive));
            this.ExitCommand = ReactiveCommand.Create<Window>(Exit);
            this.OpenSceneCommand = ReactiveCommand.CreateFromTask(this.OpenScene);
            this.RedoCommand = ReactiveCommand.Create(
                undoService.Redo,
                undoService.WhenAnyValue(x => x.CanRedo));
            this.SaveCommand = ReactiveCommand.Create(this.SaveService.Save, this.SaveService.WhenAnyValue(x => x.HasChanges));
            this.SetSelectedGizmoCommand = ReactiveCommand.Create<GizmoKind>(this.SetSelectedGizmo);
            this.UndoCommand = ReactiveCommand.Create(
                undoService.Undo,
                undoService.WhenAnyValue(x => x.CanUndo));
            this.ViewSourceCommand = ReactiveCommand.Create(ViewSource);
        }

        /// <summary>
        /// Gets a command to close a popup.
        /// </summary>
        public ICommand ClosePopupCommand { get; }

        /// <summary>
        /// Gets the content service.
        /// </summary>
        public IContentService ContentService { get; }

        /// <summary>
        /// Gets the editor service.
        /// </summary>
        public IEditorService EditorService { get; }

        /// <summary>
        /// Gets the selection service.
        /// </summary>
        public IEntityService EntityService { get; }

        /// <summary>
        /// Gets the command to exit the application.
        /// </summary>
        public ICommand ExitCommand { get; }

        /// <summary>
        /// Gets the open scene command.
        /// </summary>
        public ICommand OpenSceneCommand { get; }

        /// <summary>
        /// Gets the popup service.
        /// </summary>
        public IPopupService PopupService { get; }

        /// <summary>
        /// Gets the command to redo a previously undone operation.
        /// </summary>
        public ICommand RedoCommand { get; }

        /// <summary>
        /// Gets the command to save the current scene.
        /// </summary>
        public ICommand SaveCommand { get; }

        /// <summary>
        /// Gets the save service.
        /// </summary>
        public ISaveService SaveService { get; }

        /// <summary>
        /// Gets a command to set the selected gizmo.
        /// </summary>
        public ICommand SetSelectedGizmoCommand { get; }

        /// <summary>
        /// Gets a value indicating whether or not the non-native menu should be shown. The native menu is for MacOS only.
        /// </summary>
        public bool ShowNonNativeMenu => AvaloniaLocator.Current.GetService<IRuntimePlatform>().GetRuntimeInfo().OperatingSystem != OperatingSystemType.OSX;

        /// <summary>
        /// Gets the system service.
        /// </summary>
        public ISystemService SystemService { get; }

        /// <summary>
        /// Gets the command to undo the previous operation.
        /// </summary>
        public ICommand UndoCommand { get; }

        /// <summary>
        /// Gets the command to view the source code.
        /// </summary>
        public ICommand ViewSourceCommand { get; }

        /// <summary>
        /// Gets or sets the selected tab.
        /// </summary>
        public EditorTabs SelectedTab {
            get => this._settingsService.Settings.LastTabSelected;
            set {
                this._settingsService.Settings.LastTabSelected = value;
                this.RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not the window should close.
        /// </summary>
        /// <returns>A value indicating whether or not the window should close.</returns>
        public async Task<YesNoCancelResult> TryClose() {
            var result = await this.SaveService.RequestSave();
            if (result != YesNoCancelResult.Cancel) {
                this._settingsService.Save();
            }

            return result;
        }

        private void ClosePopup() {
            this.PopupService.TryCloseCurrentPopup(out _);
        }

        private static void Exit(Window window) {
            window?.Close();
        }

        private async Task OpenScene() {
            var saveResult = await this.SaveService.RequestSave();

            if (saveResult != YesNoCancelResult.Cancel) {
                var result = await this._dialogService.OpenAssetSelectionDialog(typeof(SceneAsset), false);

                if (result != null && !this._sceneService.TryLoadScene(result.Id, out _)) {
                    await this._dialogService.ShowWarningDialog("Error", "The scene could not be loaded");
                }
            }
        }

        private void SetSelectedGizmo(GizmoKind kind) {
            this.EditorService.SelectedGizmo = kind;
        }

        private static void ViewSource() {
            WebHelper.OpenInBrowser("https://github.com/Macabresoft/Macabre2D");
        }
    }
}