namespace Macabresoft.Macabre2D.UI.ProjectEditor.Controls.Popups {
    using System.Windows.Input;
    using Avalonia;
    using Avalonia.Controls.Primitives;
    using Avalonia.Input;
    using Avalonia.Markup.Xaml;
    using Macabresoft.Macabre2D.UI.Common.Models;

    public class PopupContainer : HeaderedContentControl {
        public static readonly StyledProperty<ICommand> CloseCommandProperty =
            AvaloniaProperty.Register<PopupContainer, ICommand>(nameof(CloseCommand));

        public PopupContainer() {
            this.InitializeComponent();
        }

        public IPopup Popup => this.Content as IPopup;

        public ICommand CloseCommand {
            get => this.GetValue(CloseCommandProperty);
            set => this.SetValue(CloseCommandProperty, value);
        }

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }
    }
}