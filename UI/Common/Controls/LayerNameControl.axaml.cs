namespace Macabresoft.Macabre2D.UI.Common;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Markup.Xaml;
using Macabresoft.AvaloniaEx;
using Macabresoft.Macabre2D.Framework;

public class LayerNameControl : UserControl {
    public static readonly DirectProperty<LayerNameControl, string> LayerNameProperty =
        AvaloniaProperty.RegisterDirect<LayerNameControl, string>(
            nameof(LayerName),
            editor => editor.LayerName,
            (editor, value) => editor.LayerName = value,
            defaultBindingMode: BindingMode.TwoWay);

    public static readonly StyledProperty<Layers> LayerProperty =
        AvaloniaProperty.Register<LayerNameControl, Layers>(
            nameof(Layer),
            defaultBindingMode: BindingMode.OneTime,
            notifying: OnSettingsOrLayerChanging);

    public static readonly StyledProperty<LayerSettings> LayerSettingsProperty =
        AvaloniaProperty.Register<LayerNameControl, LayerSettings>(
            nameof(LayerSettings),
            defaultBindingMode: BindingMode.OneTime,
            notifying: OnSettingsOrLayerChanging);

    public static readonly DirectProperty<LayerNameControl, bool> IsLayerEnabledProperty =
        AvaloniaProperty.RegisterDirect<LayerNameControl, bool>(
            nameof(IsLayerEnabled),
            editor => editor.IsLayerEnabled,
            (editor, value) => editor.IsLayerEnabled = value,
            defaultBindingMode: BindingMode.TwoWay);


    private readonly IUndoService _undoService;

    private bool _isLayerEnabled;
    private string _layerName;

    public LayerNameControl() : this(Resolver.Resolve<IUndoService>()) {
    }

    public LayerNameControl(IUndoService undoService) {
        this._undoService = undoService;
        this.InitializeComponent();
    }

    public bool IsLayerEnabled {
        get => this._isLayerEnabled;
        set {
            if (value != this._isLayerEnabled && this.LayerSettings is { } layerSettings) {
                var originalValue = this.LayerSettings.IsLayerEnabled(this.Layer);
                this._undoService.Do(() => {
                        this.SetAndRaise(IsLayerEnabledProperty, ref this._isLayerEnabled, value);
                        if (value) {
                            layerSettings.EnableLayers(this.Layer);
                        }
                        else {
                            layerSettings.DisableLayers(this.Layer);
                        }
                    },
                    () => {
                        this.SetAndRaise(IsLayerEnabledProperty, ref this._isLayerEnabled, originalValue);
                        if (originalValue) {
                            layerSettings.EnableLayers(this.Layer);
                        }
                        else {
                            layerSettings.DisableLayers(this.Layer);
                        }
                    });
            }
        }
    }

    public Layers Layer {
        get => this.GetValue(LayerProperty);
        set => this.SetValue(LayerProperty, value);
    }

    public string LayerName {
        get => this._layerName;
        set {
            if (value != this._layerName && this.LayerSettings is { } layerSettings) {
                var originalValue = layerSettings.GetName(this.Layer);
                this._undoService.Do(() => {
                        this.SetAndRaise(LayerNameProperty, ref this._layerName, value);
                        layerSettings.SetName(this.Layer, value);
                    },
                    () => {
                        this.SetAndRaise(LayerNameProperty, ref this._layerName, originalValue);
                        layerSettings.SetName(this.Layer, originalValue);
                    });
            }
        }
    }

    public LayerSettings LayerSettings {
        get => this.GetValue(LayerSettingsProperty);
        set => this.SetValue(LayerSettingsProperty, value);
    }

    private void InitializeComponent() {
        AvaloniaXamlLoader.Load(this);
    }

    private static void OnSettingsOrLayerChanging(IAvaloniaObject control, bool isBeforeChange) {
        if (control is LayerNameControl layerNameControl) {
            if (!isBeforeChange) {
                layerNameControl.Reset();
            }
        }
    }

    private void RaisePropertyChanged<T>(AvaloniaProperty<T> property, T value) {
        this.RaisePropertyChanged(property, Optional<T>.Empty, new BindingValue<T>(value));
    }

    private void Reset() {
        if (this.LayerSettings is { } layerSettings && this.Layer != Layers.None) {
            this._isLayerEnabled = layerSettings.IsLayerEnabled(this.Layer);
            this._layerName = layerSettings.GetName(this.Layer);
            this.RaisePropertyChanged(IsLayerEnabledProperty, this._isLayerEnabled);
            this.RaisePropertyChanged(LayerNameProperty, this._layerName);
            this.IsEnabled = this.Layer != Layers.Default;
        }
    }
}