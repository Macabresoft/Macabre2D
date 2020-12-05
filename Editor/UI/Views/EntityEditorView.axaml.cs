namespace Macabresoft.Macabre2D.Editor.UI.Views {
    using System.Linq;
    using Avalonia;
    using Avalonia.Controls;
    using Avalonia.Markup.Xaml;
    using Avalonia.VisualTree;
    using Macabresoft.Macabre2D.Editor.Library.ViewModels;

    public class EntityEditorView : UserControl {
        public EntityEditorView() {
            this.DataContext = Resolver.Resolve<EntityEditorViewModel>();
            this.InitializeComponent();
        }

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }
    }
}