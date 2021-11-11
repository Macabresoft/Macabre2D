namespace Macabresoft.Macabre2D.UI.Common {
    using Avalonia;
    using Avalonia.Data;
    using Avalonia.LogicalTree;
    using Avalonia.Markup.Xaml;
    using Avalonia.Threading;
    using Microsoft.Xna.Framework;
    using Unity;

    public class ColorEditor : ValueEditorControl<Color> {
        public static readonly DirectProperty<ColorEditor, byte> AlphaValueProperty =
            AvaloniaProperty.RegisterDirect<ColorEditor, byte>(
                nameof(AlphaValue),
                editor => editor.AlphaValue,
                (editor, value) => editor.AlphaValue = value);

        public static readonly DirectProperty<ColorEditor, byte> BlueValueProperty =
            AvaloniaProperty.RegisterDirect<ColorEditor, byte>(
                nameof(BlueValue),
                editor => editor.BlueValue,
                (editor, value) => editor.BlueValue = value);

        public static readonly DirectProperty<ColorEditor, byte> GreenValueProperty =
            AvaloniaProperty.RegisterDirect<ColorEditor, byte>(
                nameof(GreenValue),
                editor => editor.GreenValue,
                (editor, value) => editor.GreenValue = value);

        public static readonly DirectProperty<ColorEditor, byte> RedValueProperty =
            AvaloniaProperty.RegisterDirect<ColorEditor, byte>(
                nameof(RedValue),
                editor => editor.RedValue,
                (editor, value) => editor.RedValue = value);

        private byte _alphaValue;
        private byte _blueValue;
        private byte _greenValue;

        private byte _redValue;

        public ColorEditor() : this(null) {
        }

        [InjectionConstructor]
        public ColorEditor(ValueControlDependencies dependencies) : base(dependencies) {
            this.InitializeComponent();
        }

        public byte AlphaValue {
            get => this._alphaValue;
            set => this.SetAndRaise(AlphaValueProperty, ref this._alphaValue, value);
        }

        public byte BlueValue {
            get => this._blueValue;
            set => this.SetAndRaise(BlueValueProperty, ref this._blueValue, value);
        }

        public byte GreenValue {
            get => this._greenValue;
            set => this.SetAndRaise(GreenValueProperty, ref this._greenValue, value);
        }

        public byte RedValue {
            get => this._redValue;
            set => this.SetAndRaise(RedValueProperty, ref this._redValue, value);
        }

        protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e) {
            base.OnAttachedToLogicalTree(e);
            this.UpdateDisplayValues();
        }

        protected override void OnPropertyChanged<T>(AvaloniaPropertyChangedEventArgs<T> change) {
            base.OnPropertyChanged(change);

            switch (change.Property.Name) {
                case nameof(this.Value):
                    this.UpdateDisplayValues();
                    break;
                case nameof(this.RedValue):
                    Dispatcher.UIThread.Post(() => this.SetValue(ValueProperty, new Color(this.RedValue, this.Value.G, this.Value.B, this.Value.A)));
                    break;
                case nameof(this.GreenValue):
                    Dispatcher.UIThread.Post(() => this.SetValue(ValueProperty, new Color(this.Value.R, this.GreenValue, this.Value.B, this.Value.A)));
                    break;
                case nameof(this.BlueValue):
                    Dispatcher.UIThread.Post(() => this.SetValue(ValueProperty, new Color(this.Value.R, this.Value.G, this.BlueValue, this.Value.A)));
                    break;
                case nameof(this.AlphaValue):
                    Dispatcher.UIThread.Post(() => this.SetValue(ValueProperty, new Color(this.Value.R, this.Value.G, this.Value.B, this.AlphaValue)));
                    break;
            }
        }

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }

        private void UpdateDisplayValues() {
            this._redValue = this.Value.R;
            this._greenValue = this.Value.G;
            this._blueValue = this.Value.B;
            this._alphaValue = this.Value.A;

            Dispatcher.UIThread.Post(() => {
                this.RaisePropertyChanged(RedValueProperty, Optional<byte>.Empty, this.RedValue);
                this.RaisePropertyChanged(GreenValueProperty, Optional<byte>.Empty, this.GreenValue);
                this.RaisePropertyChanged(BlueValueProperty, Optional<byte>.Empty, this.BlueValue);
                this.RaisePropertyChanged(AlphaValueProperty, Optional<byte>.Empty, this.AlphaValue);
            });
        }
    }
}