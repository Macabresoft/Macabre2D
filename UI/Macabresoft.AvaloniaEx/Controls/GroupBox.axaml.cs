namespace Macabresoft.AvaloniaEx;

using System.Windows.Input;
using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Markup.Xaml;
using ReactiveUI;

public class GroupBox : HeaderedContentControl {
    public static readonly DirectProperty<GroupBox, bool> HideContentProperty =
        AvaloniaProperty.RegisterDirect<GroupBox, bool>(nameof(HideContent), x => !x.ShowContent);

    public static readonly StyledProperty<bool> ShowContentProperty = AvaloniaProperty.Register<GroupBox, bool>(
        nameof(ShowContent),
        true,
        notifying: OnValueChanging,
        defaultBindingMode: BindingMode.TwoWay);

    public static readonly DirectProperty<GroupBox, ICommand> ToggleContentCommandProperty =
        AvaloniaProperty.RegisterDirect<GroupBox, ICommand>(nameof(ToggleContentCommand), x => x.ToggleContentCommand);

    public GroupBox() {
        this.ToggleContentCommand = ReactiveCommand.Create(this.CollapseContent);
        this.InitializeComponent();
    }

    public bool HideContent => !this.ShowContent;

    public ICommand ToggleContentCommand { get; }

    public bool ShowContent {
        get => this.GetValue(ShowContentProperty);
        set => this.SetValue(ShowContentProperty, value);
    }

    private void CollapseContent() {
        this.ShowContent = !this.ShowContent;
    }

    private void InitializeComponent() {
        AvaloniaXamlLoader.Load(this);
    }

    private static void OnValueChanging(IAvaloniaObject control, bool isBeforeChange) {
        if (!isBeforeChange && control is GroupBox groupBox) {
            groupBox.RaisePropertyChanged(HideContentProperty, Optional<bool>.Empty, !groupBox.ShowContent);
        }
    }
}