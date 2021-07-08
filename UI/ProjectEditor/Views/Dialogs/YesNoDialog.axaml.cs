namespace Macabresoft.Macabre2D.UI.ProjectEditor.Views.Dialogs {
    using System.Reactive;
    using System.Windows.Input;
    using Avalonia;
    using Avalonia.Controls;
    using Avalonia.Markup.Xaml;
    using ReactiveUI;

    public class YesNoDialog : Window {
        public static readonly StyledProperty<string> QuestionProperty =
            AvaloniaProperty.Register<YesNoDialog, string>(nameof(Question));

        public YesNoDialog() {
            this.DataContext = this;
            this.YesCommand = ReactiveCommand.Create<Unit, Unit>(x => this.Close(x, true));
            this.NoCommand = ReactiveCommand.Create<Unit, Unit>(x => this.Close(x, false));

            this.InitializeComponent();
        }

        public ICommand NoCommand { get; }

        public ICommand YesCommand { get; }

        public string Question {
            get => this.GetValue(QuestionProperty);
            set => this.SetValue(QuestionProperty, value);
        }

        private Unit Close(Unit unit, bool dialogResult) {
            this.Close(dialogResult);
            return unit;
        }

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }
    }
}