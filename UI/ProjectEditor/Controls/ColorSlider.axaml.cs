namespace Macabresoft.Macabre2D.UI.ProjectEditor.Controls {
    using System;
    using Avalonia;
    using Avalonia.Controls;
    using Avalonia.Controls.Primitives;
    using Avalonia.Data;
    using Avalonia.Input;
    using Avalonia.Interactivity;
    using Avalonia.Markup.Xaml;

    public class ColorSlider : UserControl {
        public static readonly DirectProperty<ColorSlider, byte> ValueDisplayProperty =
            AvaloniaProperty.RegisterDirect<ColorSlider, byte>(
                nameof(ValueDisplay),
                editor => editor.ValueDisplay,
                (editor, value) => editor.ValueDisplay = value);

        public static readonly StyledProperty<byte> ValueProperty =
            AvaloniaProperty.Register<ColorSlider, byte>(nameof(Value), notifying: OnValueChanging, defaultBindingMode: BindingMode.TwoWay);

        private IDisposable _pointerReleaseDispose;
        private byte _valueDisplay;

        public ColorSlider() {
            this.InitializeComponent();
        }

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e) {
            base.OnApplyTemplate(e);
            
            this._pointerReleaseDispose?.Dispose();

            if (this.Content is Slider slider) {
                this._pointerReleaseDispose = slider.AddDisposableHandler(PointerReleasedEvent, OnPointerReleased, RoutingStrategies.Tunnel);
            }
        }

        public byte Value {
            get => this.GetValue(ValueProperty);
            set => this.SetValue(ValueProperty, value);
        }

        public byte ValueDisplay {
            get => this._valueDisplay;
            set => this.SetAndRaise(ValueDisplayProperty, ref this._valueDisplay, value);
        }

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }
        
        private static void OnValueChanging(IAvaloniaObject control, bool isBeforeChange) {
            if (!isBeforeChange && control is ColorSlider slider && slider.Value != slider.ValueDisplay) {
                slider.ValueDisplay = slider.Value;
            }
        }

        private void OnPointerReleased(object sender, PointerReleasedEventArgs e) {
            if (this.Value != this.ValueDisplay) {
                this.Value = this.ValueDisplay;
            }
        }

    }
}