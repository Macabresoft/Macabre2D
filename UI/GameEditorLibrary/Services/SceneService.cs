namespace Macabre2D.UI.GameEditorLibrary.Services {

    using Macabre2D.Framework;
    using Macabre2D.UI.CommonLibrary.Common;
    using Macabre2D.UI.CommonLibrary.Services;
    using Macabre2D.UI.GameEditorLibrary.Models;
    using System.ComponentModel;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;

    public interface ISceneService : INotifyPropertyChanged, IChangeDetectionService {
        SceneAsset CurrentScene { get; }

        Task<SceneAsset> CreateScene(FolderAsset parentAsset, string name);

        Task<SceneAsset> LoadScene(Project project, SceneAsset asset);
    }

    public sealed class SceneService : NotifyPropertyChanged, ISceneService {
        private readonly IGameDialogService _dialogService;
        private SceneAsset _currentScene;

        public SceneService(IGameDialogService dialogService) {
            this._dialogService = dialogService;
        }

        public SceneAsset CurrentScene {
            get {
                return this._currentScene;
            }

            private set {
                var originalScene = this._currentScene;
                if (this.Set(ref this._currentScene, value)) {
                    if (originalScene != null) {
                        originalScene.OnDeleted -= this.SceneAsset_OnDeleted;
                    }

                    if (this._currentScene != null) {
                        var assetFolder = this._currentScene.GetRootFolder();
                        this.SyncAssets(assetFolder);
                        this._currentScene.OnDeleted += this.SceneAsset_OnDeleted;
                    }
                }
            }
        }

        public bool HasChanges {
            get {
                return this.CurrentScene?.HasChanges == true;
            }

            set {
                if (this.CurrentScene != null) {
                    this.CurrentScene.HasChanges = value;
                }
            }
        }

        public async Task<SceneAsset> CreateScene(FolderAsset parentAsset, string name) {
            this.CurrentScene = await Task.Run(() =>
            new SceneAsset($"{name}{FileHelper.SceneExtension}") {
                Parent = parentAsset,
            });

            this.CurrentScene.HasChanges = true;
            return this.CurrentScene;
        }

        public async Task<SceneAsset> LoadScene(Project project, SceneAsset asset) {
            if (asset != null) {
                if (this.CurrentScene != null && this.CurrentScene.HasChanges) {
                    var message = this.CurrentScene != null ? $"Would you like to save {this.CurrentScene.Name}?" : "Would you like to save the current scene?";
                    var result = this._dialogService.ShowYesNoCancelMessageBox($"Save Scene", message);
                    if (result == MessageBoxResult.Cancel) {
                        return null;
                    }
                    else if (result == MessageBoxResult.Yes) {
                        await Task.Run(() => this.CurrentScene.ForceSave());
                    }
                }

                this.CurrentScene = asset;
                this.CurrentScene.HasChanges = false;
                return this.CurrentScene;
            }

            return null;
        }

        private void SceneAsset_OnDeleted(object sender, System.EventArgs e) {
            this.CurrentScene = null;
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