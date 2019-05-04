namespace Macabre2D.UI.Services {

    using Macabre2D.Framework;
    using Macabre2D.Framework.Audio;
    using Macabre2D.Framework.Rendering;
    using Macabre2D.Framework.Serialization;
    using Macabre2D.UI.Models;
    using Macabre2D.UI.Models.FrameworkWrappers;
    using Macabre2D.UI.ServiceInterfaces;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;

    public sealed class SceneService : NotifyPropertyChanged, ISceneService {
        private readonly IDialogService _dialogService;
        private readonly Serializer _serializer;
        private SceneWrapper _currentScene;
        private bool _hasChanges;

        public SceneService(IDialogService dialogService, Serializer serializer) {
            this._serializer = serializer;
            this._dialogService = dialogService;
        }

        public SceneWrapper CurrentScene {
            get {
                return this._currentScene;
            }

            private set {
                var originalScene = this._currentScene;
                if (this.Set(ref this._currentScene, value)) {
                    this.RaisePropertyChanged(nameof(this.HasChanges));

                    if (originalScene != null) {
                        originalScene.PropertyChanged -= this.CurrentScene_PropertyChanged;
                    }

                    if (this._currentScene != null) {
                        this.CurrentScene.PropertyChanged += this.CurrentScene_PropertyChanged;

                        if (this._currentScene.SceneAsset != null) {
                            this._currentScene.SceneAsset.OnDeleted += this.SceneAsset_OnDeleted;
                        }
                    }
                }
            }
        }

        public bool HasChanges {
            get {
                return this._hasChanges || (this.CurrentScene != null && this.CurrentScene.SceneAsset == null);
            }

            set {
                this.Set(ref this._hasChanges, value);
            }
        }

        public async Task<SceneWrapper> CreateScene() {
            this.CurrentScene = await Task.Run(() => new SceneWrapper(new Scene()));
            this.HasChanges = true;
            return this.CurrentScene;
        }

        public async Task<SceneWrapper> LoadScene(Project project, SceneAsset asset) {
            if (asset != null) {
                if (this.CurrentScene != null && this.HasChanges) {
                    var message = this.CurrentScene.SceneAsset != null ? $"Would you like to save {this.CurrentScene.SceneAsset.Name}?" : "Would you like to save the current scene?";
                    var result = this._dialogService.ShowYesNoCancelMessageBox($"Save Scene", message);
                    if (result == MessageBoxResult.Cancel || (result == MessageBoxResult.Yes && !await this.SaveCurrentScene(project))) {
                        return null;
                    }
                }

                this.CurrentScene = await Task.Run(() => new SceneWrapper(asset));

                // TODO: Generesize this by looping through all assets, getting their asset type, and refreshing from there.
                // IIdentifiableAsset<T> where T must be IIdentifiable.
                this.RefreshSpritesFromAssets(project);
                this.RefreshAudioClipsFromAssets(project);
                this.RefreshFontsFromAssets(project);

                this.HasChanges = false;
                return this.CurrentScene;
            }

            return null;
        }

        public async Task<bool> SaveCurrentScene(Project project) {
            var result = false;

            if (project != null) {
                var sceneExists = await Task.Run(() => this.CurrentScene.SceneAsset != null && SceneService.SceneAssetExistsInProject(project, this.CurrentScene.SceneAsset));
                if (sceneExists) {
                    await Task.Run(() => {
                        var scene = this.CurrentScene.Scene;
                        var scenePath = this.CurrentScene.SceneAsset.GetPath();
                        scene.SaveToFile(scenePath, this._serializer);

                        // This may seem odd, but we want to force a save of the metadata asset when
                        // a scene gets saved. This stores camera position and view height between sessions.
                        this.CurrentScene.SceneAsset.HasChanges = true;
                        this.CurrentScene.SceneAsset.Save(this._serializer);
                    });

                    result = true;
                }
                else {
                    var sceneAsset = this._dialogService.ShowSaveSceneWindow(project, this.CurrentScene.Scene);
                    result = sceneAsset != null;
                    this.CurrentScene = null;
                    await this.LoadScene(project, sceneAsset);
                }
            }

            if (result) {
                this.HasChanges = false;
                project.LastSceneOpened = this.CurrentScene.SceneAsset;
            }

            return result;
        }

        private static bool SceneAssetExistsInProject(Project project, SceneAsset sceneAsset) {
            return project.SceneAssets.Any(x => x.Name == sceneAsset.Name && x.GetPath() == sceneAsset.GetPath());
        }

        private void CurrentScene_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            this.HasChanges = true;
        }

        private void RefreshAudioClipsFromAssets(Project project) {
            var audioClipComponents = this.CurrentScene.Scene.GetAllComponentsOfType<IAssetComponent<AudioClip>>();
            if (audioClipComponents.Any()) {
                var audioAssets = project.AssetFolder.GetAssetsOfType<AudioAsset>().Select(x => x.AudioClip).ToDictionary(x => x.Id);

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

        private void RefreshFontsFromAssets(Project project) {
            var fontComponents = this.CurrentScene.Scene.GetAllComponentsOfType<IAssetComponent<Font>>();
            if (fontComponents.Any()) {
                var fonts = project.AssetFolder.GetAssetsOfType<FontAsset>().Select(x => x.SavableValue).ToDictionary(x => x.Id);

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

        private void RefreshSpritesFromAssets(Project project) {
            // Note: this allows us to edit sprites freely and have all of the changes reflect up
            // into the objects. This is exactly what we should do with prefabs.
            var spriteComponents = this.CurrentScene.Scene.GetAllComponentsOfType<IAssetComponent<Sprite>>();
            if (spriteComponents.Any()) {
                var sprites = project.AssetFolder.GetAssetsOfType<SpriteWrapper>().Select(x => x.Sprite).ToDictionary(x => x.Id);

                foreach (var spriteComponent in spriteComponents) {
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
            }
        }

        private void SceneAsset_OnDeleted(object sender, System.EventArgs e) {
            this.CurrentScene = null;
        }
    }
}