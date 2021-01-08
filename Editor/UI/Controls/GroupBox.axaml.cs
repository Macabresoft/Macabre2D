namespace Macabresoft.Macabre2D.Editor.UI.Controls {
    using System.Windows.Input;
    using Avalonia;
    using Avalonia.Controls.Primitives;
    using Avalonia.Data;
    using Avalonia.Markup.Xaml;
    using ReactiveUI;

    public class GroupBox : HeaderedContentControl {
        public static readonly StyledProperty<object> CloseCommandParameterProperty =
            AvaloniaProperty.Register<GroupBox, object>(nameof(CloseCommand));

        public static readonly StyledProperty<ICommand> CloseCommandProperty =
            AvaloniaProperty.Register<GroupBox, ICommand>(nameof(CloseCommand));

        private static readonly DirectProperty<GroupBox, bool> ShowContentProperty =
            AvaloniaProperty.RegisterDirect<GroupBox, bool>(nameof(ShowContent), x => x.ShowContent);

        private static readonly DirectProperty<GroupBox, bool> HideContentProperty =
            AvaloniaProperty.RegisterDirect<GroupBox, bool>(nameof(HideContent), x => !x.ShowContent);
        
        // ReSharper disable once UnusedMember.Local
        private static readonly DirectProperty<GroupBox, ICommand> ToggleContentCommandProperty =
            AvaloniaProperty.RegisterDirect<GroupBox, ICommand>(nameof(ToggleContentCommand), x => x.ToggleContentCommand);

        private bool _showContent = true;

        public GroupBox() {
            this.ToggleContentCommand = ReactiveCommand.Create(this.CollapseContent);
            this.InitializeComponent();
        }

        public ICommand ToggleContentCommand { get; }

        public ICommand CloseCommand {
            get => this.GetValue(CloseCommandProperty);
            set => this.SetValue(CloseCommandProperty, value);
        }

        public object CloseCommandParameter {
            get => this.GetValue(CloseCommandParameterProperty);
            set => this.SetValue(CloseCommandParameterProperty, value);
        }

        public bool HideContent => !this.ShowContent;

        public bool ShowContent {
            get => this._showContent;
            private set {
                this.SetAndRaise(ShowContentProperty, ref this._showContent, value);
                RaisePropertyChanged(HideContentProperty, Optional<bool>.Empty, !this.ShowContent);
            } 
        }

        private void CollapseContent() {
            this.ShowContent = !this.ShowContent;
        }

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }
    }
}