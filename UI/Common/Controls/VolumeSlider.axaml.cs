namespace Macabre2D.UI.Common;

using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;

public partial class VolumeSlider : UserControl, IObserver<AvaloniaPropertyChangedEventArgs<float>> {
    private const float Tolerance = 0.001f;

    public static readonly DirectProperty<VolumeSlider, float> ValueDisplayProperty =
        AvaloniaProperty.RegisterDirect<VolumeSlider, float>(
            nameof(ValueDisplay),
            editor => editor.ValueDisplay,
            (editor, value) => editor.ValueDisplay = value);

    public static readonly StyledProperty<float> ValueProperty =
        AvaloniaProperty.Register<VolumeSlider, float>(nameof(Value), defaultBindingMode: BindingMode.TwoWay);

    private IDisposable _pointerReleaseDispose;

    private float _valueDisplay;

    public VolumeSlider() {
        ValueProperty.Changed.Subscribe(this);
        this.InitializeComponent();
    }

    public float Value {
        get => this.GetValue(ValueProperty);
        set => this.SetValue(ValueProperty, value);
    }

    public float ValueDisplay {
        get => this._valueDisplay;
        set => this.SetAndRaise(ValueDisplayProperty, ref this._valueDisplay, value);
    }

    public void OnCompleted() {
    }

    public void OnError(Exception error) {
    }

    public void OnNext(AvaloniaPropertyChangedEventArgs<float> value) {
        if (value.Sender == this && value.NewValue.HasValue && Math.Abs(value.NewValue.Value - this.ValueDisplay) > Tolerance) {
            this.ValueDisplay = value.NewValue.Value;
        }
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e) {
        base.OnApplyTemplate(e);

        this._pointerReleaseDispose?.Dispose();

        if (this.Content is Slider slider) {
            this._pointerReleaseDispose = slider.AddDisposableHandler(PointerReleasedEvent, this.OnPointerReleased, RoutingStrategies.Tunnel);
        }
    }

    private void OnPointerReleased(object sender, PointerReleasedEventArgs e) {
        if (Math.Abs(this.Value - this.ValueDisplay) > Tolerance && sender == this.Content) {
            this.Value = this.ValueDisplay;
        }
    }

    private void Slider_OnKeyUp(object sender, KeyEventArgs e) {
        if (Math.Abs(this.Value - this.ValueDisplay) > Tolerance && sender == this.Content) {
            this.Value = this.ValueDisplay;
        }
    }
}