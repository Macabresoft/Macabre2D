namespace Macabresoft.Macabre2D.UI.Common;

using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;

public partial class AssetReferenceControl : UserControl {
    public static readonly StyledProperty<ICommand> ClearCommandProperty =
        AvaloniaProperty.Register<AssetReferenceControl, ICommand>(nameof(ClearCommand));

    public static readonly StyledProperty<string> PathTextProperty =
        AvaloniaProperty.Register<AssetReferenceControl, string>(nameof(PathText));

    public static readonly StyledProperty<StreamGeometry> SearchIconProperty =
        AvaloniaProperty.Register<AssetReferenceControl, StreamGeometry>(nameof(SearchIcon));

    public static readonly StyledProperty<ICommand> SelectCommandProperty =
        AvaloniaProperty.Register<AssetReferenceControl, ICommand>(nameof(SelectCommand));

    public AssetReferenceControl() : base() {
        this.InitializeComponent();
    }

    public ICommand ClearCommand {
        get => this.GetValue(ClearCommandProperty);
        set => this.SetValue(ClearCommandProperty, value);
    }

    public string PathText {
        get => this.GetValue(PathTextProperty);
        set => this.SetValue(PathTextProperty, value);
    }

    public StreamGeometry SearchIcon {
        get => this.GetValue(SearchIconProperty);
        set => this.SetValue(SearchIconProperty, value);
    }

    public ICommand SelectCommand {
        get => this.GetValue(SelectCommandProperty);
        set => this.SetValue(SelectCommandProperty, value);
    }
}