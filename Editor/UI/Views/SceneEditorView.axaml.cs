namespace Macabresoft.Macabre2D.Editor.UI.Views {
    using System.Linq;
    using Avalonia;
    using Avalonia.Controls;
    using Avalonia.Controls.Primitives;
    using Avalonia.LogicalTree;
    using Avalonia.Markup.Xaml;
    using Avalonia.VisualTree;
    using Macabresoft.Macabre2D.Editor.Library.MonoGame;
    using Macabresoft.Macabre2D.Editor.Library.ViewModels;

    public class SceneEditorView : UserControl {

        public SceneEditorView() {
            this.DataContext = Resolver.Resolve<SceneEditorViewModel>();
            this.InitializeComponent();
        }

        private SceneEditorViewModel ViewModel {
            get {
                return this.DataContext as SceneEditorViewModel;
            }
        }

        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e) {
            base.OnAttachedToVisualTree(e);
            
            // HACK: for some reason the toggle button IsChecked binding doesn't get set properly initially, so we go and set them manually here.
            var grid = this.FindControl<Grid>("_gizmoGrid");
            if (grid != null) {
                var toggleButtons = grid.Children.OfType<ToggleButton>();

                foreach (var toggleButton in toggleButtons) {
                    if (toggleButton.Tag is GizmoKind kind && kind == this.ViewModel?.EditorService?.SelectedGizmo) {
                        toggleButton.IsChecked = true;
                    }
                }
            }
        }

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }
    }
}