namespace Macabresoft.Macabre2D.UI.Common;

using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public class VolumeSlider : UserControl {
    private const float Tolerance = 0.001f;

    public static readonly StyledProperty<float> ValueProperty =
        AvaloniaProperty.Register<VolumeSlider, float>(nameof(Value), notifying: OnValueChanging, defaultBindingMode: BindingMode.TwoWay);

    private IDisposable _pointerReleaseDispose;

    public VolumeSlider() {
        this.InitializeComponent();
    }

    public float Value {
        get => this.GetValue(ValueProperty);
        set => this.SetValue(ValueProperty, value);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e) {
        base.OnApplyTemplate(e);

        this._pointerReleaseDispose?.Dispose();

        if (this.Content is Slider slider) {
            this._pointerReleaseDispose = slider.AddDisposableHandler(PointerReleasedEvent, this.OnPointerReleased, RoutingStrategies.Tunnel);
        }
    }

    private void InitializeComponent() {
        AvaloniaXamlLoader.Load(this);
    }

    private void OnPointerReleased(object sender, PointerReleasedEventArgs e) {
        if (this.Content is Slider slider && Math.Abs(this.Value - slider.Value) > Tolerance) {
            this.Value = (float)slider.Value;
        }
    }

    private static void OnValueChanging(IAvaloniaObject control, bool isBeforeChange) {
        if (!isBeforeChange && control is VolumeSlider { Content: Slider slider } volumeSlider && Math.Abs(volumeSlider.Value - slider.Value) > Tolerance) {
            slider.Value = volumeSlider.Value;
        }
    }
}