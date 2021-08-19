namespace Macabresoft.Macabre2D.UI.ProjectEditor.Controls {
    using Avalonia;
    using Avalonia.Controls;
    using Avalonia.Data;
    using Avalonia.Markup.Xaml;
    using Macabresoft.Macabre2D.UI.Common.Models.Content;

    public class FilteredContentTree : UserControl {
        public static readonly StyledProperty<FilteredContentWrapper> RootProperty =
            AvaloniaProperty.Register<FilteredContentTree, FilteredContentWrapper>(nameof(Root));

        public static readonly StyledProperty<FilteredContentWrapper> SelectedItemProperty =
            AvaloniaProperty.Register<FilteredContentTree, FilteredContentWrapper>(
                nameof(SelectedItem),
                defaultBindingMode: BindingMode.TwoWay);

        public FilteredContentTree() {
            this.InitializeComponent();
        }

        public FilteredContentWrapper Root {
            get => this.GetValue(RootProperty);
            set => this.SetValue(RootProperty, value);
        }

        public FilteredContentWrapper SelectedItem {
            get => this.GetValue(SelectedItemProperty);
            set => this.SetValue(SelectedItemProperty, value);
        }

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }
    }
}