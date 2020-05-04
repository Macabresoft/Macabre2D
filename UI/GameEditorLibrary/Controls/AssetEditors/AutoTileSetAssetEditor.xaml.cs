namespace Macabre2D.UI.GameEditorLibrary.Controls.AssetEditors {

    using GalaSoft.MvvmLight.CommandWpf;
    using Macabre2D.UI.CommonLibrary.Common;
    using Macabre2D.UI.CommonLibrary.Models;
    using Macabre2D.UI.CommonLibrary.Services;
    using Macabre2D.UI.GameEditorLibrary.Models;
    using Macabre2D.UI.GameEditorLibrary.Models.FrameworkWrappers;
    using Macabre2D.UI.GameEditorLibrary.Services;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for AutoTileSetEditor.xaml
    /// </summary>
    public partial class AutoTileSetAssetEditor : UserControl {

        public static readonly DependencyProperty AssetProperty = DependencyProperty.Register(
            nameof(Asset),
            typeof(AutoTileSetAsset),
            typeof(AutoTileSetAssetEditor),
            new PropertyMetadata());

        private readonly IGameDialogService _dialogService = ViewContainer.Resolve<IGameDialogService>();
        private readonly IProjectService _projectService = ViewContainer.Resolve<IProjectService>();
        private readonly IUndoService _undoService = ViewContainer.Resolve<IUndoService>();

        public AutoTileSetAssetEditor() {
            this.InitializeComponent();
            this.SelectSpriteCommand = new RelayCommand<IndexedWrapper<SpriteWrapper>>(x => this.SelectSprite(x), x => x != null);
            this.ClearSpriteCommand = new RelayCommand<IndexedWrapper<SpriteWrapper>>(x => this.SetSprite(x, null), x => x != null);
        }

        public AutoTileSetAsset Asset {
            get { return (AutoTileSetAsset)this.GetValue(AssetProperty); }
            set { this.SetValue(AssetProperty, value); }
        }

        public RelayCommand<IndexedWrapper<SpriteWrapper>> ClearSpriteCommand { get; }

        public RelayCommand<IndexedWrapper<SpriteWrapper>> SelectSpriteCommand { get; }

        private void ListBoxItem_MouseDoubleClick(object sender, RoutedEventArgs e) {
            if (sender is ListBoxItem listBoxItem && listBoxItem.DataContext is IndexedWrapper<SpriteWrapper> indexedWrapper) {
                this.SelectSprite(indexedWrapper);
            }
        }

        private void SelectSprite(IndexedWrapper<SpriteWrapper> indexedWrapper) {
            if (indexedWrapper != null) {
                if (this._dialogService.ShowSelectSpriteDialog(indexedWrapper.WrappedObject, out var spriteWrapper)) {
                    this.SetSprite(indexedWrapper, spriteWrapper);
                }
            }
        }

        private void SetSprite(IndexedWrapper<SpriteWrapper> indexedWrapper, SpriteWrapper newSprite) {
            var originalValue = indexedWrapper.WrappedObject;
            var hasChanges = this._projectService.HasChanges;
            var undoCommand = new UndoCommand(() => {
                indexedWrapper.WrappedObject = newSprite;
                this._projectService.HasChanges = true;
            }, () => {
                indexedWrapper.WrappedObject = originalValue;
                this._projectService.HasChanges = hasChanges;
            });

            this._undoService.Do(undoCommand);
        }
    }
}