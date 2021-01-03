namespace Macabresoft.Macabre2D.Editor.UI.Controls {
    using Avalonia.Controls.Primitives;
    using Avalonia.Markup.Xaml;

    public class VerticalGrouping : HeaderedContentControl {
        public VerticalGrouping() {
            this.InitializeComponent();
        }

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }
    }
}