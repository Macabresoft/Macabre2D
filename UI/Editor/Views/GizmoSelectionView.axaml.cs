namespace Macabresoft.Macabre2D.UI.Editor {
    using Avalonia.Controls;
    using Avalonia.Markup.Xaml;
    using Macabresoft.Macabre2D.UI.Common;

    public class GizmoSelectionView : UserControl {
        public GizmoSelectionView() {
            this.ViewModel = Resolver.Resolve<GizmoSelectionViewModel>();
            this.InitializeComponent();
        }
        
        public GizmoSelectionViewModel ViewModel { get; }

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }
    }
}