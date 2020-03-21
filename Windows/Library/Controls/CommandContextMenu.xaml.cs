namespace Macabre2D.Engine.Windows.Controls {

    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;

    public partial class CommandContextMenu : UserControl {

        public static readonly DependencyProperty MenuItemsProperty = DependencyProperty.Register(
            nameof(MenuItems),
            typeof(IEnumerable<MenuItem>),
            typeof(CommandContextMenu),
            new PropertyMetadata(Enumerable.Empty<MenuItem>(), new PropertyChangedCallback(OnMenuItemsChanged)));

        public CommandContextMenu() {
            this.Loaded += this.CommandContextMenu_Loaded;
            this.InitializeComponent();
        }

        public IEnumerable<MenuItem> MenuItems {
            get { return (IEnumerable<MenuItem>)this.GetValue(MenuItemsProperty); }
            set { this.SetValue(MenuItemsProperty, value); }
        }

        private static void OnMenuItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if (d is CommandContextMenu control) {
                control.Reload();
            }
        }

        private void CommandContextMenu_Loaded(object sender, RoutedEventArgs e) {
            this.Reload();
            this.Loaded -= this.CommandContextMenu_Loaded;
        }

        private void OnMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            e.Handled = true;
            this._contextMenu.IsOpen = true;
        }

        private void Reload() {
            this.Visibility = this.MenuItems?.Any() == true ? Visibility.Visible : Visibility.Collapsed;
            this._contextMenu.Items.Clear();

            foreach (var menuItem in this.MenuItems) {
                this._contextMenu.Items.Add(menuItem);
            }
        }
    }
}