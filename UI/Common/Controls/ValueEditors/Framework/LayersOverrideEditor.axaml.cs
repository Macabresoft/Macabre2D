namespace Macabresoft.Macabre2D.UI.Common;

using System.ComponentModel;
using Avalonia;
using Avalonia.Markup.Xaml;
using Macabresoft.AvaloniaEx;
using Macabresoft.Macabre2D.Framework;
using Unity;

public partial class LayersOverrideEditor : ValueEditorControl<LayersOverride> {
    public static readonly DirectProperty<LayersOverrideEditor, bool> IsOverrideEnabledProperty =
        AvaloniaProperty.RegisterDirect<LayersOverrideEditor, bool>(
            nameof(IsOverrideEnabled),
            editor => editor.IsOverrideEnabled,
            (editor, value) => editor.IsOverrideEnabled = value);

    public static readonly DirectProperty<LayersOverrideEditor, Layers> LayersValueProperty =
        AvaloniaProperty.RegisterDirect<LayersOverrideEditor, Layers>(
            nameof(LayersValue),
            editor => editor.LayersValue,
            (editor, value) => editor.LayersValue = value);

    private readonly IUndoService _undoService;
    private bool _isOverrideEnabled;
    private Layers _layersValue;

    public LayersOverrideEditor() : this(null, Resolver.Resolve<IUndoService>()) {
    }

    [InjectionConstructor]
    public LayersOverrideEditor(ValueControlDependencies dependencies, IUndoService undoService) : base(dependencies) {
        this._undoService = undoService;
        this.InitializeComponent();
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

    public Layers LayersValue {
        get => this._layersValue;
        set {
            if (this.SetAndRaise(LayersValueProperty, ref this._layersValue, value) && this.Value.Value != value) {
                var previousLayers = this.Value.Value;
                var newLayers = value;
                this._undoService.Do(() => { this.Value.Value = newLayers; }, () => { this.Value.Value = previousLayers; });
            }
        }
    }

    protected override void OnValueChanged(AvaloniaPropertyChangedEventArgs<LayersOverride> args) {
        base.OnValueChanged(args);

        if (args.OldValue is { HasValue: true, Value: { } layersOverride }) {
            layersOverride.PropertyChanged -= this.Value_PropertyChanged;
        }

        if (this.Value != null) {
            this.LayersValue = this.Value.Value;
            this.IsOverrideEnabled = this.Value.IsEnabled;
            this.Value.PropertyChanged += this.Value_PropertyChanged;
        }
    }

    private void Value_PropertyChanged(object sender, PropertyChangedEventArgs e) {
        if (this.Value != null) {
            switch (e.PropertyName) {
                case nameof(this.Value.Value):
                    this.LayersValue = this.Value.Value;
                    break;
                case nameof(this.Value.IsEnabled):
                    this.IsOverrideEnabled = this.Value.IsEnabled;
                    break;
            }
        }
    }
}