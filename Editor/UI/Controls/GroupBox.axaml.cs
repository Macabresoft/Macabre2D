namespace Macabresoft.Macabre2D.Editor.UI.Controls {
    using System.Windows.Input;
    using Avalonia;
    using Avalonia.Controls.Primitives;
    using Avalonia.Data;
    using Avalonia.Markup.Xaml;
    using ReactiveUI;

    public class GroupBox : HeaderedContentControl {
        private static readonly DirectProperty<GroupBox, bool> HideContentProperty =
            AvaloniaProperty.RegisterDirect<GroupBox, bool>(nameof(HideContent), x => !x.ShowContent);

        private static readonly DirectProperty<GroupBox, bool> ShowContentProperty =
            AvaloniaProperty.RegisterDirect<GroupBox, bool>(nameof(ShowContent), x => x.ShowContent);

        // ReSharper disable once UnusedMember.Local
        private static readonly DirectProperty<GroupBox, ICommand> ToggleContentCommandProperty =
            AvaloniaProperty.RegisterDirect<GroupBox, ICommand>(nameof(ToggleContentCommand), x => x.ToggleContentCommand);

        private bool _showContent = true;

        public GroupBox() {
            this.ToggleContentCommand = ReactiveCommand.Create(this.CollapseContent);
            this.InitializeComponent();
        }

        public bool HideContent => !this.ShowContent;

        public ICommand ToggleContentCommand { get; }

        public bool ShowContent {
            get => this._showContent;
            private set {
                this.SetAndRaise(ShowContentProperty, ref this._showContent, value);
                this.RaisePropertyChanged(HideContentProperty, Optional<bool>.Empty, !this.ShowContent);
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