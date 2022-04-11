namespace Macabresoft.Macabre2D.UI.Common;

using Avalonia;
using Avalonia.Data;
using Avalonia.LogicalTree;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using Macabresoft.Macabre2D.Framework;
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
    
    public static readonly DirectProperty<ColorEditor, string> HexValueProperty =
        AvaloniaProperty.RegisterDirect<ColorEditor, string>(
            nameof(HexValue),
            editor => editor.HexValue,
            (editor, value) => editor.HexValue = value);

    private byte _alphaValue;
    private byte _blueValue;
    private byte _greenValue;
    private byte _redValue;
    private string _hexValue;
    private bool _isUpdating;

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
    
    public string HexValue {
        get => this._hexValue;
        set => this.SetAndRaise(HexValueProperty, ref this._hexValue, value);
    }

    protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e) {
        base.OnAttachedToLogicalTree(e);
        this.UpdateDisplayValues();
    }

    protected override void OnPropertyChanged<T>(AvaloniaPropertyChangedEventArgs<T> change) {
        base.OnPropertyChanged(change);

        if (this._isUpdating) {
            return;
        }

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
            case nameof(this.HexValue):
                Dispatcher.UIThread.Post(this.UpdateValueFromHex);
                break;
        }
    }

    private void InitializeComponent() {
        AvaloniaXamlLoader.Load(this);
    }

    private void UpdateDisplayValues() {
        Dispatcher.UIThread.Post(() => {
            try {
                this._isUpdating = true;
                this.RedValue = this.Value.R;
                this.GreenValue = this.Value.G;
                this.BlueValue = this.Value.B;
                this.AlphaValue = this.Value.A;
                this.HexValue = this.Value.ToHex();
            }
            finally {
                this._isUpdating = false;
            }
        });
    }

    private void UpdateValueFromHex() {
        // TODO: update it
    }
}