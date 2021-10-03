namespace Macabresoft.Macabre2D.UI.Editor {
    using System.Reactive;
    using System.Windows.Input;
    using Avalonia;
    using Avalonia.Markup.Xaml;
    using Macabresoft.Macabre2D.UI.Common;
    using ReactiveUI;

    public class YesNoCancelDialog : BaseDialog {
        public static readonly StyledProperty<bool> AllowCancelProperty =
            AvaloniaProperty.Register<YesNoCancelDialog, bool>(nameof(AllowCancel), true);

        public static readonly StyledProperty<string> QuestionProperty =
            AvaloniaProperty.Register<YesNoCancelDialog, string>(nameof(Question));

        public YesNoCancelDialog() {
            this.CancelCommand = ReactiveCommand.Create<Unit, Unit>(x => this.Close(x, YesNoCancelResult.Cancel));
            this.NoCommand = ReactiveCommand.Create<Unit, Unit>(x => this.Close(x, YesNoCancelResult.No));
            this.YesCommand = ReactiveCommand.Create<Unit, Unit>(x => this.Close(x, YesNoCancelResult.Yes));

            this.InitializeComponent();
        }


        public ICommand CancelCommand { get; }

        public ICommand NoCommand { get; }

        public ICommand YesCommand { get; }

        public bool AllowCancel {
            get => this.GetValue(AllowCancelProperty);
            set => this.SetValue(AllowCancelProperty, value);
        }

        public string Question {
            get => this.GetValue(QuestionProperty);
            set => this.SetValue(QuestionProperty, value);
        }

        private Unit Close(Unit unit, YesNoCancelResult dialogResult) {
            this.Close(dialogResult);
            return unit;
        }

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }
    }
}