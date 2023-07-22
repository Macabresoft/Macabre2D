namespace Macabresoft.Macabre2D.UI.Common;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Markup.Xaml;

public partial class FilteredSceneTree : UserControl {
    public static readonly StyledProperty<FilteredEntityWrapper> RootProperty =
        AvaloniaProperty.Register<FilteredSceneTree, FilteredEntityWrapper>(nameof(Root));

    public static readonly StyledProperty<FilteredEntityWrapper> SelectedItemProperty =
        AvaloniaProperty.Register<FilteredSceneTree, FilteredEntityWrapper>(
            nameof(SelectedItem),
            defaultBindingMode: BindingMode.TwoWay);

    public FilteredSceneTree() {
        this.InitializeComponent();
    }

    public FilteredEntityWrapper Root {
        get => this.GetValue(RootProperty);
        set => this.SetValue(RootProperty, value);
    }

    public FilteredEntityWrapper SelectedItem {
        get => this.GetValue(SelectedItemProperty);
        set => this.SetValue(SelectedItemProperty, value);
    }
}