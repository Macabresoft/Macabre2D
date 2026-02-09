namespace Macabre2D.UI.Common;

using System;
using System.Globalization;
using Avalonia;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.Threading;
using Macabre2D.Framework;
using Microsoft.Xna.Framework;
using Unity;

public partial class ColorEditor : ValueEditorControl<Color> {
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

    public static readonly DirectProperty<ColorEditor, string> HexValueProperty =
        AvaloniaProperty.RegisterDirect<ColorEditor, string>(
            nameof(HexValue),
            editor => editor.HexValue,
            (editor, value) => editor.HexValue = value);

    public static readonly DirectProperty<ColorEditor, byte> RedValueProperty =
        AvaloniaProperty.RegisterDirect<ColorEditor, byte>(
            nameof(RedValue),
            editor => editor.RedValue,
            (editor, value) => editor.RedValue = value);

    private byte _alphaValue;
    private byte _blueValue;
    private byte _greenValue;
    private string _hexValue;
    private bool _isUpdating;
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

    public string HexValue {
        get => this._hexValue;
        set => this.SetAndRaise(HexValueProperty, ref this._hexValue, value);
    }

    public byte RedValue {
        get => this._redValue;
        set => this.SetAndRaise(RedValueProperty, ref this._redValue, value);
    }

    protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e) {
        base.OnAttachedToLogicalTree(e);
        this.UpdateDisplayValues();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change) {
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
        }
    }

    private void HexValueTextBox_OnLostFocus(object sender, RoutedEventArgs e) {
        if (!string.Equals(this.Value.ToHex(), this.HexValue, StringComparison.OrdinalIgnoreCase)) {
            Dispatcher.UIThread.Post(this.UpdateValueFromHex);
        }
    }

    private void UpdateDisplayValues() {
        Dispatcher.UIThread.Post(() =>
        {
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
        var hexValue = this.HexValue;
        if (hexValue.StartsWith("#")) {
            hexValue = hexValue.Replace("#", string.Empty);
        }

        if (hexValue.Length == 3) {
            hexValue = $"{hexValue[0]}{hexValue[0]}{hexValue[1]}{hexValue[1]}{hexValue[2]}{hexValue[2]}";
        }

        if (hexValue.Length == 6) {
            try {
                var r = byte.Parse($"{hexValue[0]}{hexValue[1]}", NumberStyles.HexNumber);
                var g = byte.Parse($"{hexValue[2]}{hexValue[3]}", NumberStyles.HexNumber);
                var b = byte.Parse($"{hexValue[4]}{hexValue[5]}", NumberStyles.HexNumber);
                var color = new Color(r, g, b, this.Value.A);
                Dispatcher.UIThread.Post(() => this.Value = color);
            }
            catch {
                Dispatcher.UIThread.Post(() => this.HexValue = this.Value.ToHex());
            }
        }
        else {
            Dispatcher.UIThread.Post(() => this.HexValue = this.Value.ToHex());
        }
    }
}