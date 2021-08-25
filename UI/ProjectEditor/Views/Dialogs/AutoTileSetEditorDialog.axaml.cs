using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Macabresoft.Macabre2D.UI.ProjectEditor.Views.Dialogs {
    using Macabresoft.Macabre2D.UI.Common.ViewModels.Dialogs;
    using Unity;

    public class AutoTileSetEditorDialog : Window {

        public AutoTileSetEditorDialog() {
        }

        [InjectionConstructor]
        public AutoTileSetEditorDialog(AutoTileSetEditorViewModel viewModel) {
            this.DataContext = viewModel;
            InitializeComponent();
        }

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }
    }
}