namespace Macabresoft.Macabre2D.Editor.Library.ViewModels {
    using System;
    using System.Reactive;
    using System.Windows.Input;
    using Macabresoft.Macabre2D.Editor.Library.Services;
    using ReactiveUI;

    /// <summary>
    /// The view model for the main window.
    /// </summary>
    public class MainWindowViewModel : ViewModelBase {
        private readonly ReactiveCommand<Unit, Unit> _undoCommand;
        private readonly ReactiveCommand<Unit, Unit> _redoCommand;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindowViewModel" /> class.
        /// </summary>
        /// <param name="undoService">The undo service.</param>
        public MainWindowViewModel(IUndoService undoService) {
            if (undoService == null) {
                throw new ArgumentNullException(nameof(undoService));
            }
            
            this._undoCommand = ReactiveCommand.Create(
                undoService.Undo,
                undoService.WhenAnyValue(x => x.CanUndo));
            this._redoCommand = ReactiveCommand.Create(
                undoService.Redo,
                undoService.WhenAnyValue(x => x.CanRedo));
        }

        /// <summary>
        /// Gets the command to undo the previous operation.
        /// </summary>
        public ICommand UndoCommand => this._undoCommand;

        /// <summary>
        /// Gets the command to redo a previously undone operation.
        /// </summary>
        public ICommand RedoCommand => this._redoCommand;
    }
}