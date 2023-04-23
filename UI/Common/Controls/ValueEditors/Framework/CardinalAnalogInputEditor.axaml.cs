namespace Macabresoft.Macabre2D.UI.Common;

using System;
using Avalonia;
using Avalonia.Markup.Xaml;
using Macabresoft.AvaloniaEx;
using Macabresoft.Macabre2D.Framework;
using Unity;

public class CardinalAnalogInputEditor : ValueEditorControl<CardinalAnalogInput> {
    public static readonly DirectProperty<CardinalAnalogInputEditor, InputAction> DownActionProperty =
        AvaloniaProperty.RegisterDirect<CardinalAnalogInputEditor, InputAction>(
            nameof(DownAction),
            editor => editor.DownAction,
            (editor, value) => editor.DownAction = value);

    public static readonly DirectProperty<CardinalAnalogInputEditor, bool> IsAnalogStickEnabledProperty =
        AvaloniaProperty.RegisterDirect<CardinalAnalogInputEditor, bool>(
            nameof(IsAnalogStickEnabled),
            editor => editor.IsAnalogStickEnabled,
            (editor, value) => editor.IsAnalogStickEnabled = value);

    public static readonly DirectProperty<CardinalAnalogInputEditor, InputAction> LeftActionProperty =
        AvaloniaProperty.RegisterDirect<CardinalAnalogInputEditor, InputAction>(
            nameof(LeftAction),
            editor => editor.LeftAction,
            (editor, value) => editor.LeftAction = value);

    public static readonly DirectProperty<CardinalAnalogInputEditor, InputAction> RightActionProperty =
        AvaloniaProperty.RegisterDirect<CardinalAnalogInputEditor, InputAction>(
            nameof(RightAction),
            editor => editor.RightAction,
            (editor, value) => editor.RightAction = value);

    public static readonly DirectProperty<CardinalAnalogInputEditor, InputAction> UpActionProperty =
        AvaloniaProperty.RegisterDirect<CardinalAnalogInputEditor, InputAction>(
            nameof(UpAction),
            editor => editor.UpAction,
            (editor, value) => editor.UpAction = value);

    private readonly IUndoService _undoService;
    private InputAction _downAction;
    private bool _isAnalogStickEnabled;
    private InputAction _leftAction;
    private InputAction _rightAction;
    private InputAction _upAction;

    public CardinalAnalogInputEditor() : this(null, Resolver.Resolve<IUndoService>()) {
    }

    [InjectionConstructor]
    public CardinalAnalogInputEditor(ValueControlDependencies dependencies, IUndoService undoService) : base(dependencies) {
        this._undoService = undoService;
        this.InitializeComponent();
    }

    public InputAction DownAction {
        get => this._downAction;
        set {
            if (value != this._downAction && this.Value is { } analogInput) {
                var originalValue = this._downAction;
                this.SetPropertyWithUndo(value, originalValue, x =>
                {
                    this.SetAndRaise(DownActionProperty, ref this._downAction, x);
                    analogInput.Down = x;
                });
            }
        }
    }

    public bool IsAnalogStickEnabled {
        get => this._isAnalogStickEnabled;
        set {
            if (value != this._isAnalogStickEnabled && this.Value is { } analogInput) {
                var originalValue = this._isAnalogStickEnabled;
                this.SetPropertyWithUndo(value, originalValue, x =>
                {
                    this.SetAndRaise(IsAnalogStickEnabledProperty, ref this._isAnalogStickEnabled, x);
                    analogInput.IsEnabled = x;
                });
            }
        }
    }

    public InputAction LeftAction {
        get => this._leftAction;
        set {
            if (value != this._leftAction && this.Value is { } analogInput) {
                var originalValue = this._leftAction;
                this.SetPropertyWithUndo(value, originalValue, x =>
                {
                    this.SetAndRaise(LeftActionProperty, ref this._leftAction, x);
                    analogInput.Left = x;
                });
            }
        }
    }

    public InputAction RightAction {
        get => this._rightAction;
        set {
            if (value != this._rightAction && this.Value is { } analogInput) {
                var originalValue = this._rightAction;
                this.SetPropertyWithUndo(value, originalValue, x =>
                {
                    this.SetAndRaise(RightActionProperty, ref this._rightAction, x);
                    analogInput.Right = x;
                });
            }
        }
    }

    public InputAction UpAction {
        get => this._upAction;
        set {
            if (value != this._upAction && this.Value is { } analogInput) {
                var originalValue = this._upAction;
                this.SetPropertyWithUndo(value, originalValue, x =>
                {
                    this.SetAndRaise(UpActionProperty, ref this._upAction, x);
                    analogInput.Up = x;
                });
            }
        }
    }

    protected override void OnValueChanged() {
        base.OnValueChanged();

        if (this.Value is { } analogInput) {
            this.SetAndRaise(IsAnalogStickEnabledProperty, ref this._isAnalogStickEnabled, analogInput.IsEnabled);
            this.SetAndRaise(UpActionProperty, ref this._upAction, analogInput.Up);
            this.SetAndRaise(DownActionProperty, ref this._downAction, analogInput.Down);
            this.SetAndRaise(LeftActionProperty, ref this._leftAction, analogInput.Left);
            this.SetAndRaise(RightActionProperty, ref this._rightAction, analogInput.Right);
        }
    }

    private void InitializeComponent() {
        AvaloniaXamlLoader.Load(this);
    }

    private void SetPropertyWithUndo<T>(T newValue, T originalValue, Action<T> setAction) {
        this._undoService.Do(() => { setAction(newValue); }, () => { setAction(originalValue); });
    }
}