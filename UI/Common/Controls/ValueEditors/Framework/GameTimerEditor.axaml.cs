namespace Macabre2D.UI.Common;

using Avalonia;
using Avalonia.Markup.Xaml;
using Macabresoft.AvaloniaEx;
using Macabre2D.Framework;
using Unity;

public partial class GameTimerEditor : ValueEditorControl<GameTimer> {
    public static readonly DirectProperty<GameTimerEditor, float> TimeLimitProperty =
        AvaloniaProperty.RegisterDirect<GameTimerEditor, float>(
            nameof(TimeLimit),
            editor => editor.TimeLimit,
            (editor, value) => editor.TimeLimit = value);

    private readonly IUndoService _undoService;
    private float _timeLimit;

    public GameTimerEditor() : this(null, Resolver.Resolve<IUndoService>()) {
    }

    [InjectionConstructor]
    public GameTimerEditor(ValueControlDependencies dependencies, IUndoService undoService) : base(dependencies) {
        this._undoService = undoService;
        this.InitializeComponent();
    }

    public float TimeLimit {
        get => this._timeLimit;
        set => this.SetAndRaise(TimeLimitProperty, ref this._timeLimit, value);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change) {
        base.OnPropertyChanged(change);

        if (change.Property.Name == nameof(this.Value)) {
            this.UpdateDisplayValues();
        }
        else if (change.Property.Name == nameof(this.TimeLimit)) {
            var originalTimeLimit = this.Value.TimeLimit;
            var newTimeLimit = this.TimeLimit;
            if (originalTimeLimit != this.TimeLimit) {
                this._undoService.Do(() =>
                {
                    this.Value.TimeLimit = newTimeLimit;
                    this.UpdateDisplayValues();
                }, () =>
                {
                    this.Value.TimeLimit = originalTimeLimit;
                    this.UpdateDisplayValues();
                });
            }
        }
    }

    private void UpdateDisplayValues() {
        if (this.Value != null) {
            this.TimeLimit = this.Value.TimeLimit;
        }
    }
}