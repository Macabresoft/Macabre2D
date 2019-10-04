namespace Macabre2D.UI.Services {

    using Macabre2D.Framework;
    using Macabre2D.UI.Common;
    using Macabre2D.UI.Models;
    using Macabre2D.UI.ServiceInterfaces;
    using System;
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
                        this.RefreshAssets(assetFolder);
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
                RequiresCreation = true
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
                    if (result == MessageBoxResult.Cancel || (result == MessageBoxResult.Yes && !await this.SaveCurrentScene(project))) {
                        return null;
                    }
                }

                asset.Load();
                this.CurrentScene = asset;
                this.HasChanges = false;
                return this.CurrentScene;
            }

            return null;
        }

        public async Task<bool> SaveCurrentScene(Project project) {
            var result = false;

            if (project != null) {
                var sceneExists = await Task.Run(() => this.CurrentScene != null && SceneService.SceneAssetExistsInProject(project, this.CurrentScene));
                if (sceneExists) {
                    await Task.Run(() => {
                        var scene = this.CurrentScene.SavableValue;
                        this.CurrentScene.HasChanges = true;
                        this.CurrentScene.Save(project.AssetManager);
                    });

                    result = true;
                }
                else {
                    var sceneAsset = this._dialogService.ShowSaveSceneWindow(project, this.CurrentScene.SavableValue);
                    result = sceneAsset != null;
                    this.CurrentScene = null;
                    await this.LoadScene(project, sceneAsset);
                }
            }

            if (result) {
                this.HasChanges = false;
                project.LastSceneOpened = this.CurrentScene;
            }

            return result;
        }

        private static bool SceneAssetExistsInProject(Project project, SceneAsset sceneAsset) {
            return project.SceneAssets.Any(x => x.Id == sceneAsset.Id);
        }

        private void CurrentScene_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            this.HasChanges = true;
        }

        private void RefreshAssets(FolderAsset rootFolderAsset) {
            // TODO: Generesize this by looping through all assets, getting their asset type, and refreshing from there.
            // IIdentifiableAsset<T> where T must be IIdentifiable.
            this.RefreshSpritesFromAssets(rootFolderAsset);
            this.RefreshAudioClipsFromAssets(rootFolderAsset);
            this.RefreshFontsFromAssets(rootFolderAsset);
        }

        private void RefreshAudioClipsFromAssets(FolderAsset assetFolder) {
            var audioClipComponents = this.CurrentScene.SavableValue.GetAllComponentsOfType<IAssetComponent<AudioClip>>();
            if (audioClipComponents.Any()) {
                var audioAssets = assetFolder.GetAssetsOfType<AudioAsset>().Select(x => x.AudioClip).ToDictionary(x => x.Id);

                foreach (var audioClipComponent in audioClipComponents) {
                    var ids = audioClipComponent.GetOwnedAssetIds();

                    foreach (var id in ids) {
                        if (audioAssets.TryGetValue(id, out var sprite)) {
                            audioClipComponent.RefreshAsset(sprite);
                        }
                        else {
                            audioClipComponent.RemoveAsset(id);
                        }
                    }
                }
            }
        }

        private void RefreshFontsFromAssets(FolderAsset assetFolder) {
            var fontComponents = this.CurrentScene.SavableValue.GetAllComponentsOfType<IAssetComponent<Font>>();
            if (fontComponents.Any()) {
                var fonts = assetFolder.GetAssetsOfType<FontAsset>().Select(x => x.SavableValue).ToDictionary(x => x.Id);

                foreach (var fontComponent in fontComponents) {
                    var ids = fontComponent.GetOwnedAssetIds();

                    foreach (var id in ids) {
                        if (fonts.TryGetValue(id, out var sprite)) {
                            fontComponent.RefreshAsset(sprite);
                        }
                        else {
                            fontComponent.RemoveAsset(id);
                        }
                    }
                }
            }
        }

        private void RefreshSpritesFromAssets(FolderAsset assetFolder) {
            // Note: this allows us to edit sprites freely and have all of the changes reflect up
            // into the objects. This is exactly what we should do with prefabs.

            var spriteComponents = this.CurrentScene.SavableValue.GetAllComponentsOfType<IAssetComponent<Sprite>>();
            if (spriteComponents.Any()) {
                var sprites = assetFolder.GetAssetsOfType<ImageAsset>().SelectMany(x => x.Sprites).Select(x => x.Sprite).ToDictionary(x => x.Id);

                foreach (var spriteComponent in spriteComponents) {
                    try {
                        var ids = spriteComponent.GetOwnedAssetIds();

                        foreach (var id in ids) {
                            if (sprites.TryGetValue(id, out var sprite)) {
                                spriteComponent.RefreshAsset(sprite);
                            }
                            else {
                                spriteComponent.RemoveAsset(id);
                            }
                        }
                    }
                    catch (Exception e) {
                        Console.WriteLine(e.Message);
                    }
                }
            }
        }

        private void SceneAsset_OnDeleted(object sender, System.EventArgs e) {
            this.CurrentScene = null;
        }

        private void SceneAsset_OnRefreshed(object sender, System.EventArgs e) {
            if (sender is SceneAsset sceneAsset) {
                sceneAsset.Load();
            }
        }
    }
}