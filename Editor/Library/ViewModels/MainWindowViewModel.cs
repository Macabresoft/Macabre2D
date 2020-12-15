namespace Macabresoft.Macabre2D.Editor.Library.ViewModels {
    using System;
    using System.Reactive;
    using System.Windows.Input;
    using Macabresoft.Macabre2D.Editor.Library.Services;
    using ReactiveUI;
    using Unity;

    /// <summary>
    /// The view model for the main window.
    /// </summary>
    public class MainWindowViewModel : ViewModelBase {
        private readonly ReactiveCommand<Unit, Unit> _undoCommand;
        private readonly ReactiveCommand<Unit, Unit> _redoCommand;

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
            
            this._undoCommand = ReactiveCommand.Create(
                undoService.Undo,
                undoService.WhenAnyValue(x => x.CanUndo));
            this._redoCommand = ReactiveCommand.Create(
                undoService.Redo,
                undoService.WhenAnyValue(x => x.CanRedo));
        }
        
        /// <summary>
        /// Gets the selection service.
        /// </summary>
        public ISelectionService SelectionService { get; }

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