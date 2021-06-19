namespace Macabresoft.Macabre2D.UI.Library.ViewModels {
    using System;
    using System.Windows.Input;
    using Avalonia;
    using Avalonia.Controls;
    using Avalonia.Platform;
    using Macabresoft.Core;
    using Macabresoft.Macabre2D.UI.Library.Services;
    using ReactiveUI;
    using Unity;

    /// <summary>
    /// The view model for the main window.
    /// </summary>
    public class MainWindowViewModel : ViewModelBase {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindowViewModel" /> class.
        /// </summary>
        /// <remarks>This constructor only exists for design time XAML.</remarks>
        public MainWindowViewModel() : base() {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindowViewModel" /> class.
        /// </summary>
        /// <param name="selectionService">The selection service.</param>
        /// <param name="undoService">The undo service.</param>
        [InjectionConstructor]
        public MainWindowViewModel(ISelectionService selectionService, IUndoService undoService) : base() {
            if (undoService == null) {
                throw new ArgumentNullException(nameof(undoService));
            }

            this.SelectionService = selectionService ?? throw new ArgumentNullException(nameof(selectionService));

            this.ExitCommand = ReactiveCommand.Create<Window>(this.Exit);
            this.RedoCommand = ReactiveCommand.Create(
                undoService.Redo,
                undoService.WhenAnyValue(x => x.CanRedo));
            this.UndoCommand = ReactiveCommand.Create(
                undoService.Undo,
                undoService.WhenAnyValue(x => x.CanUndo));
            this.ViewSourceCommand = ReactiveCommand.Create(this.ViewSource);
        }

        /// <summary>
        /// Gets the command to exit the application.
        /// </summary>
        public ICommand ExitCommand { get; }

        /// <summary>
        /// Gets the command to redo a previously undone operation.
        /// </summary>
        public ICommand RedoCommand { get; }

        /// <summary>
        /// Gets the selection service.
        /// </summary>
        public ISelectionService SelectionService { get; }

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

        private void Exit(Window window) {
            window?.Close();
        }

        private void ViewSource() {
            WebHelper.OpenInBrowser("https://github.com/Macabresoft/Macabre2D");
        }
    }
}