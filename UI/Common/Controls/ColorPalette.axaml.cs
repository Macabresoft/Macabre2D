namespace Macabresoft.Macabre2D.UI.Common;

using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Markup.Xaml;
using Macabresoft.Macabre2D.Framework;
using Microsoft.Xna.Framework;

public class ColorPalette : UserControl {
    public static readonly StyledProperty<IEnumerable<Color>> AvailableColorsProperty = AvaloniaProperty.Register<ColorPalette, IEnumerable<Color>>(
        nameof(AvailableColors),
        DefinedColors.AllColors,
        defaultBindingMode: BindingMode.TwoWay);

    public static readonly StyledProperty<Color> SelectedColorProperty = AvaloniaProperty.Register<ColorPalette, Color>(
        nameof(SelectedColor),
        Color.Transparent,
        defaultBindingMode: BindingMode.TwoWay);


    public ColorPalette() {
        this.InitializeComponent();
    }

    public IEnumerable<Color> AvailableColors {
        get => this.GetValue(AvailableColorsProperty);
        set => this.SetValue(AvailableColorsProperty, value);
    }

    public Color SelectedColor {
        get => this.GetValue(SelectedColorProperty);
        set => this.SetValue(SelectedColorProperty, value);
    }

    private void InitializeComponent() {
        AvaloniaXamlLoader.Load(this);
    }
}