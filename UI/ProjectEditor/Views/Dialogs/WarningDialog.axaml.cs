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

        public static readonly StyledProperty<string> WarningTitleProperty =
            AvaloniaProperty.Register<WarningDialog, string>(nameof(WarningTitle));

        public WarningDialog() {
            this.OkCommand = ReactiveCommand.Create<Unit, Unit>(this.Close);
            this.InitializeComponent();
        }

        public ICommand OkCommand { get; }

        public string WarningMessage {
            get => this.GetValue(WarningMessageProperty);
            set => this.SetValue(WarningMessageProperty, value);
        }

        public string WarningTitle {
            get => this.GetValue(WarningTitleProperty);
            set => this.SetValue(WarningTitleProperty, value);
        }

        private Unit Close(Unit unit) {
            this.Close(true);
            return Unit.Default;
        }

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }
    }
}