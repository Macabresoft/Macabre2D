namespace Macabresoft.AvaloniaEx;

using System.Windows.Input;
using ReactiveUI;

/// <summary>
/// A base dialog view model.
/// </summary>
public class BaseDialogViewModel : UndoBaseViewModel {
    private bool _isOkEnabled = true;

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseDialogViewModel" /> class.
    /// </summary>
    /// <param name="undoService">The undo service.</param>
    protected BaseDialogViewModel(IUndoService undoService) : base(undoService) {
        this.OkCommand = ReactiveCommand.Create<IWindow>(
            this.Ok,
            this.WhenAny(x => x.IsOkEnabled, y => y.Value));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseDialogViewModel" /> class.
    /// </summary>
    protected BaseDialogViewModel() : base() {
        this.OkCommand = ReactiveCommand.Create<IWindow>(
            this.Ok,
            this.WhenAny(x => x.IsOkEnabled, y => y.Value));
    }

    /// <summary>
    /// Gets a command which provides the results of the dialog.
    /// </summary>
    public ICommand OkCommand { get; }

    /// <summary>
    /// Gets a value indicating whether or not the 'Ok' button is enabled.
    /// </summary>
    public bool IsOkEnabled {
        get => this._isOkEnabled;
        set => this.RaiseAndSetIfChanged(ref this._isOkEnabled, value);
    }

    /// <summary>
    /// Called when the user smashes that OK button.
    /// </summary>
    protected virtual void OnOk() {
    }

    private void Ok(IWindow window) {
        this.OnOk();
        window.Close(true);
    }
}