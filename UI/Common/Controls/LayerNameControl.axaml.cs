namespace Macabresoft.Macabre2D.UI.Common;

using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Macabresoft.AvaloniaEx;
using Macabresoft.Macabre2D.Framework;

public partial class LayerNameControl : UserControl, IObserver<AvaloniaPropertyChangedEventArgs<Layers>>, IObserver<AvaloniaPropertyChangedEventArgs<LayerSettings>> {
    public static readonly DirectProperty<LayerNameControl, bool> IsLayerEnabledProperty =
        AvaloniaProperty.RegisterDirect<LayerNameControl, bool>(
            nameof(IsLayerEnabled),
            editor => editor.IsLayerEnabled,
            (editor, value) => editor.IsLayerEnabled = value,
            defaultBindingMode: BindingMode.TwoWay);

    public static readonly DirectProperty<LayerNameControl, string> LayerNameProperty =
        AvaloniaProperty.RegisterDirect<LayerNameControl, string>(
            nameof(LayerName),
            editor => editor.LayerName,
            (editor, value) => editor.LayerName = value,
            defaultBindingMode: BindingMode.TwoWay);

    public static readonly StyledProperty<Layers> LayerProperty =
        AvaloniaProperty.Register<LayerNameControl, Layers>(
            nameof(Layer),
            defaultBindingMode: BindingMode.OneTime);

    public static readonly StyledProperty<LayerSettings> LayerSettingsProperty =
        AvaloniaProperty.Register<LayerNameControl, LayerSettings>(
            nameof(LayerSettings),
            defaultBindingMode: BindingMode.OneTime);


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
                this._undoService.Do(() =>
                    {
                        this.SetAndRaise(IsLayerEnabledProperty, ref this._isLayerEnabled, value);
                        if (value) {
                            layerSettings.EnableLayers(this.Layer);
                        }
                        else {
                            layerSettings.DisableLayers(this.Layer);
                        }
                    },
                    () =>
                    {
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
                this._undoService.Do(() =>
                    {
                        this.SetAndRaise(LayerNameProperty, ref this._layerName, value);
                        layerSettings.SetName(this.Layer, value);
                    },
                    () =>
                    {
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

    public void OnCompleted() {
    }

    public void OnError(Exception error) {
    }

    public void OnNext(AvaloniaPropertyChangedEventArgs<LayerSettings> value) {
        this.Reset();
    }

    public void OnNext(AvaloniaPropertyChangedEventArgs<Layers> value) {
        this.Reset();
    }

    private void RaisePropertyChanged<T>(DirectPropertyBase<T> property, T value) {
        this.RaisePropertyChanged(property, default, value);
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