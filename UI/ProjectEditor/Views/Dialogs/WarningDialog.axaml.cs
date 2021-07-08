namespace Macabresoft.Macabre2D.UI.ProjectEditor.Views.Dialogs {
    using System.Reactive;
    using System.Windows.Input;
    using Avalonia;
    using Avalonia.Controls;
    using Avalonia.Markup.Xaml;
    using ReactiveUI;

    public class WarningDialog : Window {
        public static readonly StyledProperty<string> WarningMessageProperty =
            AvaloniaProperty.Register<WarningDialog, string>(nameof(WarningMessage));

        public WarningDialog() {
            this.OkCommand = ReactiveCommand.Create<Unit, Unit>(this.Close);
            this.InitializeComponent();
        }

        public ICommand OkCommand { get; }

        public string WarningMessage {
            get => this.GetValue(WarningMessageProperty);
            set => this.SetValue(WarningMessageProperty, value);
        }

        private Unit Close(Unit unit) {
            this.Close(true);
            return unit;
        }

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }
    }
}