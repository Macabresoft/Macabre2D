namespace Macabresoft.Macabre2D.UI.Editor.Views {
    using Avalonia.Controls;
    using Avalonia.Markup.Xaml;
    using Macabresoft.Macabre2D.UI.Common;

    public class GizmoSelectionView : UserControl {
        public GizmoSelectionView() {
            this.DataContext = Resolver.Resolve<GizmoSelectionViewModel>();
            this.InitializeComponent();
        }

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }
    }
}