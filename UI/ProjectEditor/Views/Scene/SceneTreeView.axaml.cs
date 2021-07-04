namespace Macabresoft.Macabre2D.UI.ProjectEditor.Views.Scene {
    using Avalonia;
    using Avalonia.Controls;
    using Avalonia.Markup.Xaml;
    using Macabresoft.Macabre2D.UI.Common.ViewModels.Scene;

    public class SceneTreeView : UserControl {
        public static readonly DirectProperty<SceneTreeView, SceneTreeViewModel> ViewModelProperty =
            AvaloniaProperty.RegisterDirect<SceneTreeView, SceneTreeViewModel>(
                nameof(ViewModel),
                editor => editor.ViewModel);

        public SceneTreeView() {
            this.DataContext = Resolver.Resolve<SceneTreeViewModel>();
            this.InitializeComponent();
        }

        public SceneTreeViewModel ViewModel => this.DataContext as SceneTreeViewModel;

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }
    }
}