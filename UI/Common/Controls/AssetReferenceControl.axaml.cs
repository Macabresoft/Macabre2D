namespace Macabresoft.Macabre2D.UI.Common;

using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

public class AssetReferenceControl : UserControl {
    public static readonly StyledProperty<ICommand> ClearCommandProperty =
        AvaloniaProperty.Register<AssetReferenceControl, ICommand>(nameof(ClearCommand));

    public static readonly StyledProperty<string> PathTextProperty =
        AvaloniaProperty.Register<AssetReferenceControl, string>(nameof(PathText));

    public static readonly StyledProperty<ICommand> SelectCommandProperty =
        AvaloniaProperty.Register<AssetReferenceControl, ICommand>(nameof(SelectCommand));

    public static readonly StyledProperty<string> TitleProperty =
        AvaloniaProperty.Register<AssetReferenceControl, string>(nameof(Title));

    public AssetReferenceControl() {
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

    public ICommand SelectCommand {
        get => this.GetValue(SelectCommandProperty);
        set => this.SetValue(SelectCommandProperty, value);
    }

    public string Title {
        get => this.GetValue(TitleProperty);
        set => this.SetValue(TitleProperty, value);
    }

    private void InitializeComponent() {
        AvaloniaXamlLoader.Load(this);
    }
}