namespace Macabre2D.Engine.Windows.Common.Behaviors {

    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    public static class MouseDoubleClickBehavior {

        public static DependencyProperty CommandParameterProperty = DependencyProperty.RegisterAttached(
            "CommandParameter",
            typeof(object),
            typeof(MouseDoubleClickBehavior),
            new UIPropertyMetadata(null));

        public static DependencyProperty CommandProperty = DependencyProperty.RegisterAttached(
            "Command",
            typeof(ICommand),
            typeof(MouseDoubleClickBehavior),
            new UIPropertyMetadata(CommandChanged));

        public static object GetCommandParameter(DependencyObject target) {
            return target.GetValue(CommandParameterProperty);
        }

        public static void SetCommand(DependencyObject target, ICommand value) {
            target.SetValue(CommandProperty, value);
        }

        public static void SetCommandParameter(DependencyObject target, object value) {
            target.SetValue(CommandParameterProperty, value);
        }

        private static void CommandChanged(DependencyObject target, DependencyPropertyChangedEventArgs e) {
            if (target is Control control) {
                if ((e.NewValue != null) && (e.OldValue == null)) {
                    control.MouseDoubleClick += OnMouseDoubleClick;
                }
                else if ((e.NewValue == null) && (e.OldValue != null)) {
                    control.MouseDoubleClick -= OnMouseDoubleClick;
                }
            }
        }

        private static void OnMouseDoubleClick(object sender, RoutedEventArgs e) {
            var control = sender as Control;

            if (!(sender is TreeViewItem treeViewItem) || treeViewItem.IsSelected) {
                var command = (ICommand)control.GetValue(CommandProperty);
                var commandParameter = control.GetValue(CommandParameterProperty);
                if (command.CanExecute(commandParameter)) {
                    command.Execute(commandParameter);
                    e.Handled = true;
                }
            }
        }
    }
}