namespace Macabresoft.Macabre2D.UI.ProjectEditor.Views.Content {
    using Avalonia.Controls;
    using Avalonia.Markup.Xaml;
    using Macabresoft.Macabre2D.UI.Common.ViewModels.Content;
    using Macabresoft.Macabre2D.UI.Common.ViewModels.Scene;

    public class ContentTreeView : UserControl {
        public ContentTreeView() {
            this.DataContext = Resolver.Resolve<ContentTreeViewModel>();
            this.InitializeComponent();
        }

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }
    }
}