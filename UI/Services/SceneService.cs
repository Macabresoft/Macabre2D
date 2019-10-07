namespace Macabre2D.UI.Services {

    using Macabre2D.Framework;
    using Macabre2D.UI.Common;
    using Macabre2D.UI.Models;
    using Macabre2D.UI.ServiceInterfaces;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;

    public sealed class SceneService : NotifyPropertyChanged, ISceneService {
        private readonly IDialogService _dialogService;
        private SceneAsset _currentScene;
        private bool _hasChanges;

        public SceneService(IDialogService dialogService) {
            this._dialogService = dialogService;
        }

        public SceneAsset CurrentScene {
            get {
                return this._currentScene;
            }

            private set {
                var originalScene = this._currentScene;
                if (this.Set(ref this._currentScene, value)) {
                    this.RaisePropertyChanged(nameof(this.HasChanges));

                    if (originalScene != null) {
                        originalScene.PropertyChanged -= this.CurrentScene_PropertyChanged;
                        originalScene.OnDeleted -= this.SceneAsset_OnDeleted;
                        originalScene.OnRefreshed -= this.SceneAsset_OnRefreshed;
                        originalScene.Unload();
                    }

                    if (this._currentScene != null) {
                        this.CurrentScene.PropertyChanged += this.CurrentScene_PropertyChanged;

                        var assetFolder = this._currentScene.GetRootFolder();
                        this.SyncAssets(assetFolder);
                        this._currentScene.OnDeleted += this.SceneAsset_OnDeleted;
                        this._currentScene.OnRefreshed += this.SceneAsset_OnRefreshed;
                    }
                }
            }
        }

        public bool HasChanges {
            get {
                return this._hasChanges;
            }

            set {
                this.Set(ref this._hasChanges, value);
            }
        }

        public async Task<SceneAsset> CreateScene(FolderAsset parentAsset, string name) {
            this.CurrentScene = await Task.Run(() =>
            new SceneAsset($"{name}{FileHelper.SceneExtension}") {
                Parent = parentAsset,
            });

            this.CurrentScene.Load();
            this.HasChanges = true;
            return this.CurrentScene;
        }

        public async Task<SceneAsset> LoadScene(Project project, SceneAsset asset) {
            if (asset != null) {
                if (this.CurrentScene != null && this.HasChanges) {
                    var message = this.CurrentScene != null ? $"Would you like to save {this.CurrentScene.Name}?" : "Would you like to save the current scene?";
                    var result = this._dialogService.ShowYesNoCancelMessageBox($"Save Scene", message);
                    if (result == MessageBoxResult.Cancel) {
                        return null;
                    }
                    else if (result == MessageBoxResult.Yes) {
                        await Task.Run(() => this.CurrentScene.ForceSave());
                    }
                }

                asset.Load();
                this.CurrentScene = asset;
                this.HasChanges = false;
                return this.CurrentScene;
            }

            return null;
        }

        private void CurrentScene_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            this.HasChanges = true;
        }

        private void SceneAsset_OnDeleted(object sender, System.EventArgs e) {
            this.CurrentScene = null;
        }

        private void SceneAsset_OnRefreshed(object sender, System.EventArgs e) {
            if (sender is SceneAsset sceneAsset) {
                sceneAsset.Load();
            }
        }

        private void SyncAssets(FolderAsset rootFolderAsset) {
            // TODO: it'd be cool if this could be generic so we don't have to remember to update with each new asset type.
            this.SyncAssetsInScene<Prefab>(rootFolderAsset);
            this.SyncAssetsInScene<AutoTileSet>(rootFolderAsset);
            this.SyncAssetsInScene<SpriteAnimation>(rootFolderAsset);
            this.SyncAssetsInScene<AudioClip>(rootFolderAsset);
            this.SyncAssetsInScene<Font>(rootFolderAsset);
            this.SyncAssetsInScene<Shader>(rootFolderAsset);
            this.SyncAssetsInScene<Sprite>(rootFolderAsset);
        }

        private void SyncAssetsInScene<TAsset>(FolderAsset assetFolder) where TAsset : IIdentifiable {
            var components = this.CurrentScene.SavableValue.GetAllComponentsOfType<IAssetComponent<TAsset>>();
            if (components.Any()) {
                var idToAssets = assetFolder.GetAssetsOfType<ISyncAsset<TAsset>>().SelectMany(x => x.GetAssetsToSync()).ToDictionary(x => x.Id);

                foreach (var component in components) {
                    var ids = component.GetOwnedAssetIds();
                    foreach (var id in ids) {
                        if (idToAssets.TryGetValue(id, out var asset)) {
                            component.RefreshAsset(asset);
                        }
                        else {
                            component.RemoveAsset(id);
                        }
                    }
                }
            }
        }
    }
}