namespace Macabresoft.Macabre2D.UI.Common.ViewModels {
    using System;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using Avalonia;
    using Avalonia.Controls;
    using Avalonia.Platform;
    using Macabresoft.Core;
    using Macabresoft.Macabre2D.UI.Common.Models;
    using Macabresoft.Macabre2D.UI.Common.Services;
    using ReactiveUI;
    using Unity;

    /// <summary>
    /// The view model for the main window.
    /// </summary>
    public class MainWindowViewModel : ViewModelBase {
        private readonly IDialogService _dialogService;

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
        /// <param name="entityService">The selection service.</param>
        /// <param name="saveService">The save service.</param>
        /// <param name="undoService">The undo service.</param>
        [InjectionConstructor]
        public MainWindowViewModel(
            IDialogService dialogService,
            IEntityService entityService,
            ISaveService saveService,
            IUndoService undoService) : base() {
            this._dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            this.EntityService = entityService ?? throw new ArgumentNullException(nameof(entityService));
            this.SaveService = saveService ?? throw new ArgumentNullException(nameof(saveService));

            this.ExitCommand = ReactiveCommand.Create<Window>(Exit);
            this.RedoCommand = ReactiveCommand.Create(
                undoService.Redo,
                undoService.WhenAnyValue(x => x.CanRedo));

            this.SaveCommand = ReactiveCommand.Create(this.SaveService.Save, this.SaveService.WhenAnyValue(x => x.HasChanges));
            this.UndoCommand = ReactiveCommand.Create(
                undoService.Undo,
                undoService.WhenAnyValue(x => x.CanUndo));
            this.ViewSourceCommand = ReactiveCommand.Create(ViewSource);
        }

        /// <summary>
        /// Gets the selection service.
        /// </summary>
        public IEntityService EntityService { get; }

        /// <summary>
        /// Gets the command to exit the application.
        /// </summary>
        public ICommand ExitCommand { get; }

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
        /// Gets a value indicating whether or not the non-native menu should be shown. The native menu is for MacOS only.
        /// </summary>
        public bool ShowNonNativeMenu => AvaloniaLocator.Current.GetService<IRuntimePlatform>().GetRuntimeInfo().OperatingSystem != OperatingSystemType.OSX;

        /// <summary>
        /// Gets the command to undo the previous operation.
        /// </summary>
        public ICommand UndoCommand { get; }

        /// <summary>
        /// Gets the command to view the source code.
        /// </summary>
        public ICommand ViewSourceCommand { get; }

        /// <summary>
        /// Gets a value indicating whether or not the window should close.
        /// </summary>
        /// <returns>A value indicating whether or not the window should close.</returns>
        public async Task<YesNoCancelResult> TryClose() {
            var result = YesNoCancelResult.No;
            if (this.SaveService.HasChanges) {
                result = await this._dialogService.ShowYesNoDialog("Unsaved Changes", "Save changes before closing?", true);

                if (result == YesNoCancelResult.Yes) {
                    this.SaveService.Save();
                }
            }

            return result;
        }

        private static void Exit(Window window) {
            window?.Close();
        }

        private static void ViewSource() {
            WebHelper.OpenInBrowser("https://github.com/Macabresoft/Macabre2D");
        }
    }
}