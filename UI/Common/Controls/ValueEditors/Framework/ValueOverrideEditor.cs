namespace Macabresoft.Macabre2D.UI.Common;

using System.ComponentModel;
using Avalonia;
using Macabresoft.AvaloniaEx;
using Macabresoft.Macabre2D.Framework;

public abstract class ValueOverrideEditor<TValue, TOverride> : ValueEditorControl<TValue> where TValue : ValueOverride<TOverride> where TOverride : notnull {
    public static readonly DirectProperty<ValueOverrideEditor<TValue, TOverride>, bool> IsOverrideEnabledProperty =
        AvaloniaProperty.RegisterDirect<ValueOverrideEditor<TValue, TOverride>, bool>(
            nameof(IsOverrideEnabled),
            editor => editor.IsOverrideEnabled,
            (editor, value) => editor.IsOverrideEnabled = value);

    public static readonly DirectProperty<ValueOverrideEditor<TValue, TOverride>, TOverride> ValueOverrideProperty =
        AvaloniaProperty.RegisterDirect<ValueOverrideEditor<TValue, TOverride>, TOverride>(
            nameof(ValueOverride),
            editor => editor.ValueOverride,
            (editor, value) => editor.ValueOverride = value);

    private readonly IUndoService _undoService;
    private bool _isOverrideEnabled;
    private TOverride _overrideValue;

    protected ValueOverrideEditor() : this(null, Resolver.Resolve<IUndoService>()) {
    }

    protected ValueOverrideEditor(ValueControlDependencies dependencies, IUndoService undoService) : base(dependencies) {
        this._undoService = undoService;
    }

    public bool IsOverrideEnabled {
        get => this._isOverrideEnabled;
        set {
            if (this.SetAndRaise(IsOverrideEnabledProperty, ref this._isOverrideEnabled, value) && this.Value.IsEnabled != value) {
                var previousIsEnabled = this.Value.IsEnabled;
                var newIsEnabled = value;
                this._undoService.Do(() => { this.Value.IsEnabled = newIsEnabled; }, () => { this.Value.IsEnabled = previousIsEnabled; });
            }
        }
    }

    public TOverride ValueOverride {
        get => this._overrideValue;
        set {
            if (this.SetAndRaise(ValueOverrideProperty, ref this._overrideValue, value) && !this.Value.Value.Equals(value)) {
                var previousLayers = this.Value.Value;
                var newLayers = value;
                this._undoService.Do(() => { this.Value.Value = newLayers; }, () => { this.Value.Value = previousLayers; });
            }
        }
    }

    protected override void OnValueChanged(AvaloniaPropertyChangedEventArgs<TValue> args) {
        base.OnValueChanged(args);

        if (args.OldValue is { HasValue: true, Value: { } layersOverride }) {
            layersOverride.PropertyChanged -= this.Value_PropertyChanged;
        }

        if (this.Value != null) {
            this.ValueOverride = this.Value.Value;
            this.IsOverrideEnabled = this.Value.IsEnabled;
            this.Value.PropertyChanged += this.Value_PropertyChanged;
        }
    }

    private void Value_PropertyChanged(object sender, PropertyChangedEventArgs e) {
        if (this.Value != null) {
            switch (e.PropertyName) {
                case nameof(this.Value.Value):
                    this.ValueOverride = this.Value.Value;
                    break;
                case nameof(this.Value.IsEnabled):
                    this.IsOverrideEnabled = this.Value.IsEnabled;
                    break;
            }
        }
    }
}