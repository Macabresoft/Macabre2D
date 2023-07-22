namespace Macabresoft.Macabre2D.UI.Common;

using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;

public partial class VolumeSlider : UserControl, IObserver<AvaloniaPropertyChangedEventArgs<float>> {
    private const float Tolerance = 0.001f;

    public static readonly StyledProperty<float> ValueProperty =
        AvaloniaProperty.Register<VolumeSlider, float>(nameof(Value), defaultBindingMode: BindingMode.TwoWay);

    private IDisposable _pointerReleaseDispose;

    public VolumeSlider() {
        this.InitializeComponent();
    }

    public float Value {
        get => this.GetValue(ValueProperty);
        set => this.SetValue(ValueProperty, value);
    }

    public void OnCompleted() {
    }

    public void OnError(Exception error) {
    }

    public void OnNext(AvaloniaPropertyChangedEventArgs<float> value) {
        if (this.Content is Slider slider) {
            slider.Value = this.Value;
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
        if (this.Content is Slider slider && Math.Abs(this.Value - slider.Value) > Tolerance) {
            this.Value = (float)slider.Value;
        }
    }
}