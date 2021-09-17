namespace Macabresoft.Macabre2D.UI.ProjectEditor.Views.Dialogs {
    using Avalonia.Markup.Xaml;
    using Macabresoft.Macabre2D.Framework;
    using Macabresoft.Macabre2D.UI.Common.ViewModels.Dialogs;
    using Unity;

    public class AutoTileSetSelectionDialog : BaseDialog {
        public AutoTileSetSelectionDialog() {
        }

        [InjectionConstructor]
        public AutoTileSetSelectionDialog(SpriteSheetAssetSelectionViewModel<AutoTileSet> viewModel) {
            this.DataContext = viewModel;
            viewModel.CloseRequested += this.OnCloseRequested;
            this.InitializeComponent();
        }

        public SpriteSheetAssetSelectionViewModel<AutoTileSet> ViewModel => this.DataContext as SpriteSheetAssetSelectionViewModel<AutoTileSet>;

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }

        private void OnCloseRequested(object sender, bool e) {
            this.Close(e);
        }
    }
}