namespace Macabresoft.Macabre2D.UI.ProjectEditor.Views.Content {
    using Avalonia;
    using Avalonia.Controls;
    using Avalonia.Markup.Xaml;
    using Macabresoft.Macabre2D.UI.Common.ViewModels.Content;

    public class ContentTreeView : UserControl {
        public static readonly DirectProperty<ContentTreeView, ContentTreeViewModel> ViewModelProperty =
            AvaloniaProperty.RegisterDirect<ContentTreeView, ContentTreeViewModel>(
                nameof(ViewModel),
                editor => editor.ViewModel);

        public ContentTreeView() {
            this.DataContext = Resolver.Resolve<ContentTreeViewModel>();
            this.InitializeComponent();
        }

        public ContentTreeViewModel ViewModel => this.DataContext as ContentTreeViewModel;

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }
    }
}