namespace Macabre2D.UI.Controls.AssetEditors {

    using GalaSoft.MvvmLight.CommandWpf;
    using Macabre2D.UI.Common;
    using Macabre2D.UI.Models;
    using Macabre2D.UI.Models.FrameworkWrappers;
    using Macabre2D.UI.ServiceInterfaces;
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

        private readonly IDialogService _dialogService = ViewContainer.Resolve<IDialogService>();
        private readonly IProjectService _projectService = ViewContainer.Resolve<IProjectService>();
        private readonly IUndoService _undoService = ViewContainer.Resolve<IUndoService>();

        public AutoTileSetAssetEditor() {
            this.InitializeComponent();
            this.ChooseSpriteCommand = new RelayCommand<IndexedWrapper<SpriteWrapper>>(x => this.ChooseSprite(x), x => x != null);
        }

        public AutoTileSetAsset Asset {
            get { return (AutoTileSetAsset)this.GetValue(AssetProperty); }
            set { this.SetValue(AssetProperty, value); }
        }

        public RelayCommand<IndexedWrapper<SpriteWrapper>> ChooseSpriteCommand { get; }

        private void ChooseSprite(IndexedWrapper<SpriteWrapper> indexedWrapper) {
            if (indexedWrapper != null) {
                if (this._dialogService.ShowSelectAssetDialog(this._projectService.CurrentProject, AssetType.Image | AssetType.Sprite, AssetType.Sprite, out var asset)) {
                    var originalValue = indexedWrapper.WrappedObject;
                    var hasChanges = this._projectService.HasChanges;
                    var undoCommand = new UndoCommand(() => {
                        indexedWrapper.WrappedObject = asset as SpriteWrapper;
                        this._projectService.HasChanges = true;
                    }, () => {
                        indexedWrapper.WrappedObject = originalValue;
                        this._projectService.HasChanges = hasChanges;
                    });

                    this._undoService.Do(undoCommand);
                }
            }
        }

        private void ListBoxItem_MouseDoubleClick(object sender, RoutedEventArgs e) {
            if (sender is ListBoxItem listBoxItem && listBoxItem.DataContext is IndexedWrapper<SpriteWrapper> indexedWrapper) {
                this.ChooseSprite(indexedWrapper);
            }
        }
    }
}