namespace Macabresoft.Macabre2D.UI.Common;

using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Macabresoft.Macabre2D.Project.Common;
using Microsoft.Xna.Framework;

public partial class ColorPalette : UserControl {
    public static readonly StyledProperty<IEnumerable<Color>> AvailableColorsProperty = AvaloniaProperty.Register<ColorPalette, IEnumerable<Color>>(
        nameof(AvailableColors),
        PredefinedColors.Colors,
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
}