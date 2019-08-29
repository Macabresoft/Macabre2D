namespace Macabre2D.UI.Controls {

    using Macabre2D.UI.Models;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;

    public partial class CommandContextMenu : UserControl {

        public static readonly DependencyProperty CommandsProperty = DependencyProperty.Register(nameof(Commands),
            typeof(IReadOnlyCollection<ComponentCommand>),
            typeof(CommandContextMenu),
            new PropertyMetadata(new List<ComponentCommand>(), new PropertyChangedCallback(OnCommandsChanged)));

        public CommandContextMenu() {
            this.Loaded += this.CommandContextMenu_Loaded;
            this.InitializeComponent();
        }

        public IReadOnlyCollection<ComponentCommand> Commands {
            get { return (IReadOnlyCollection<ComponentCommand>)this.GetValue(CommandsProperty); }
            set { this.SetValue(CommandsProperty, value); }
        }

        private static void OnCommandsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
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
            this.Visibility = this.Commands.Any() ? Visibility.Visible : Visibility.Collapsed;
            this._contextMenu.Items.Clear();

            foreach (var command in this.Commands) {
                this._contextMenu.Items.Add(new MenuItem() {
                    Header = command.Name,
                    Command = command.Command
                });
            }
        }
    }
}