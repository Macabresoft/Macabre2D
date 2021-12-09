namespace Macabresoft.AvaloniaEx;

using System.Windows.Input;
using ReactiveUI;

/// <summary>
/// A base view model for undoing.
/// </summary>
public class UndoBaseViewModel : BaseViewModel {
    /// <summary>
    /// Initializes a new instance of the <see cref="UndoBaseViewModel" /> class.
    /// </summary>
    protected UndoBaseViewModel() {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UndoBaseViewModel" /> class.
    /// </summary>
    /// <param name="undoService">The undo service.</param>
    protected UndoBaseViewModel(IUndoService undoService) {
        this.UndoService = undoService;
        this.UndoCommand = ReactiveCommand.Create(
            this.UndoService.Undo,
            this.UndoService.WhenAnyValue(x => x.CanUndo));

        this.RedoCommand = ReactiveCommand.Create(
            this.UndoService.Redo,
            this.UndoService.WhenAnyValue(x => x.CanRedo));
    }

    /// <summary>
    /// Gets the command to redo a previously undone operation.
    /// </summary>
    public ICommand RedoCommand { get; }

    /// <summary>
    /// Gets the command to undo the previous operation.
    /// </summary>
    public ICommand UndoCommand { get; }

    /// <summary>
    /// Gets the undo service.
    /// </summary>
    protected IUndoService UndoService { get; }
}