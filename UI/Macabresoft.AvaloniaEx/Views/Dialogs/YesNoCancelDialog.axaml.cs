namespace Macabresoft.AvaloniaEx;

using System.Windows.Input;
using Avalonia;
using Avalonia.Markup.Xaml;
using ReactiveUI;
using Unity;

public class YesNoCancelDialog : BaseDialog {
    public static readonly StyledProperty<bool> AllowCancelProperty =
        AvaloniaProperty.Register<YesNoCancelDialog, bool>(nameof(AllowCancel), true);

    public static readonly StyledProperty<string> QuestionProperty =
        AvaloniaProperty.Register<YesNoCancelDialog, string>(nameof(Question));

    [InjectionConstructor]
    public YesNoCancelDialog() {
        this.CancelCommand = ReactiveCommand.Create<IWindow>(x => this.Close(YesNoCancelResult.Cancel));
        this.NoCommand = ReactiveCommand.Create<IWindow>(x => this.Close(YesNoCancelResult.No));
        this.YesCommand = ReactiveCommand.Create<IWindow>(x => this.Close(YesNoCancelResult.Yes));

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

    private void InitializeComponent() {
        AvaloniaXamlLoader.Load(this);
    }
}