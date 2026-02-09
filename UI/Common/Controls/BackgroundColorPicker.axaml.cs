namespace Macabre2D.UI.Common;

using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Macabre2D.Project.Common;
using Microsoft.Xna.Framework;

public partial class BackgroundColorPicker : UserControl {

    public static readonly StyledProperty<Color> BackgroundColorProperty = AvaloniaProperty.Register<BackgroundColorPicker, Color>(
        nameof(BackgroundColor),
        Color.Transparent,
        defaultBindingMode: BindingMode.TwoWay);


    public BackgroundColorPicker() {
        this.InitializeComponent();
    }

    public Color BackgroundColor {
        get => this.GetValue(BackgroundColorProperty);
        set => this.SetValue(BackgroundColorProperty, value);
    }
}