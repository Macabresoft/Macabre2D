namespace Macabresoft.Macabre2D.UI.ProjectEditor.Controls {
    using System;
    using Avalonia;
    using Avalonia.Controls;
    using Avalonia.Data;
    using Avalonia.Markup.Xaml;
    using Macabresoft.Macabre2D.UI.Common.Models.Content;

    public class FilteredContentTree : UserControl {
        public static readonly StyledProperty<FilteredContentWrapper> RootProperty =
            AvaloniaProperty.Register<FilteredContentTree, FilteredContentWrapper>(nameof(Root),
                notifying: Notifying);

        private static void Notifying(IAvaloniaObject arg1, bool arg2) {
            if (arg1 is FilteredContentTree tree) {
                Console.WriteLine();
            }
        }

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