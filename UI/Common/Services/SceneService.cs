namespace Macabresoft.Macabre2D.UI.Common;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Macabresoft.Macabre2D.Framework;
using ReactiveUI;

/// <summary>
/// Interface for a service which handles the <see cref="IScene" /> open in the editor.
/// </summary>
public interface ISceneService : ISelectionService<object> {
    /// <summary>
    /// Gets the current scene.
    /// </summary>
    /// <value>The current scene.</value>
    IScene CurrentScene { get; }

    /// <summary>
    /// Gets the current scene metadata.
    /// </summary>
    ContentMetadata CurrentSceneMetadata { get; }

    /// <summary>
    /// Gets the implied selected object.
    /// </summary>
    object ImpliedSelected { get; }

    /// <summary>
    /// Gets a value indicating whether or not the state of the program is in an entity context.
    /// </summary>
    bool IsEntityContext { get; }

    /// <summary>
    /// Saves the current scene.
    /// </summary>
    void SaveScene();

    /// <summary>
    /// Saves the provided scene and metadata.
    /// </summary>
    void SaveScene(ContentMetadata metadata, IScene scene);

    /// <summary>
    /// Tries to load a scene.
    /// </summary>
    /// <param name="contentId">The content identifier of the scene.</param>
    /// <param name="sceneAsset">The scene asset.</param>
    /// <returns>A value indicating whether or not the scene was loaded.</returns>
    bool TryLoadScene(Guid contentId, out SceneAsset sceneAsset);
}

/// <summary>
/// A service which handles the <see cref="IScene" /> open in the editor.
/// </summary>
public sealed class SceneService : ReactiveObject, ISceneService {
    private readonly IEntityService _entityService;
    private readonly IFileSystemService _fileSystem;
    private readonly IPathService _pathService;
    private readonly ISerializer _serializer;
    private readonly IEditorSettingsService _settingsService;
    private readonly ISystemService _systemService;
    private ContentMetadata _currentSceneMetadata;
    private object _impliedSelected;
    private bool _isEntityContext;
    private object _selected;

    /// <summary>
    /// Initializes a new instance of the <see cref="SceneService" /> class.
    /// </summary>
    /// <param name="entityService">The entity service.</param>
    /// <param name="fileSystem">The file system service.</param>
    /// <param name="pathService">The path service.</param>
    /// <param name="serializer">The serializer.</param>
    /// <param name="settingsService">The settings service.</param>
    /// <param name="systemService">The system service.</param>
    public SceneService(
        IEntityService entityService,
        IFileSystemService fileSystem,
        IPathService pathService,
        ISerializer serializer,
        IEditorSettingsService settingsService,
        ISystemService systemService) {
        this._entityService = entityService;
        this._fileSystem = fileSystem;
        this._pathService = pathService;
        this._serializer = serializer;
        this._settingsService = settingsService;
        this._systemService = systemService;
    }

    /// <inheritdoc />
    public IScene CurrentScene => (this._currentSceneMetadata?.Asset as SceneAsset)?.Content;


    /// <inheritdoc />
    public IReadOnlyCollection<ValueControlCollection> Editors {
        get {
            return this._selected switch {
                IEntity => this._entityService.Editors,
                IUpdateableSystem => this._systemService.Editors,
                _ => null
            };
        }
    }

    /// <inheritdoc />
    public ContentMetadata CurrentSceneMetadata {
        get => this._currentSceneMetadata;
        private set {
            this.RaiseAndSetIfChanged(ref this._currentSceneMetadata, value);
            this.RaisePropertyChanged(nameof(this.CurrentScene));
        }
    }

    /// <inheritdoc />
    public object ImpliedSelected {
        get => this._impliedSelected;
        private set => this.RaiseAndSetIfChanged(ref this._impliedSelected, value);
    }

    /// <inheritdoc />
    public bool IsEntityContext {
        get => this._isEntityContext;
        private set => this.RaiseAndSetIfChanged(ref this._isEntityContext, value);
    }

    /// <inheritdoc />
    public object Selected {
        get => this._selected;
        set {
            this.RaiseAndSetIfChanged(ref this._selected, value);
            this._entityService.Selected = null;
            this._systemService.Selected = null;
            this.IsEntityContext = false;

            switch (this._selected) {
                case IScene scene:
                    this._entityService.Selected = scene;
                    this.ImpliedSelected = this._selected;
                    break;
                case IUpdateableSystem system:
                    this._systemService.Selected = system;
                    this.ImpliedSelected = this._selected;
                    break;
                case IEntity entity:
                    this._entityService.Selected = entity;
                    this.ImpliedSelected = this._selected;
                    this.IsEntityContext = true;
                    break;
                case SystemCollection:
                    this._entityService.Selected = this.CurrentScene;
                    this.ImpliedSelected = this.CurrentScene;
                    break;
                case EntityCollection:
                    this.IsEntityContext = true;
                    this._entityService.Selected = this.CurrentScene;
                    this.ImpliedSelected = this.CurrentScene;
                    break;
            }

            this.RaisePropertyChanged(nameof(this.Editors));
        }
    }

    /// <inheritdoc />
    public void SaveScene() {
        this.SaveScene(this.CurrentSceneMetadata, this.CurrentScene);
    }

    /// <inheritdoc />
    public void SaveScene(ContentMetadata metadata, IScene scene) {
        if (metadata != null && scene != null && metadata.Asset is IAsset<Scene>) {
            var metadataPath = this._pathService.GetMetadataFilePath(metadata.ContentId);
            this._serializer.Serialize(metadata, metadataPath);

            var filePath = Path.Combine(this._pathService.ContentDirectoryPath, metadata.GetContentPath());
            this._serializer.Serialize(scene, filePath);
        }
    }

    /// <inheritdoc />
    public bool TryLoadScene(Guid contentId, out SceneAsset sceneAsset) {
        if (contentId == Guid.Empty) {
            sceneAsset = null;
        }
        else {
            var metadataFilePath = this._pathService.GetMetadataFilePath(contentId);
            if (this._fileSystem.DoesFileExist(metadataFilePath)) {
                var metadata = this._serializer.Deserialize<ContentMetadata>(metadataFilePath);
                sceneAsset = metadata?.Asset as SceneAsset;

                if (metadata != null && sceneAsset != null) {
                    var contentPath = Path.Combine(this._pathService.ContentDirectoryPath, metadata.GetContentPath());

                    if (this._fileSystem.DoesFileExist(contentPath)) {
                        var scene = this._serializer.Deserialize<Scene>(contentPath);
                        if (scene != null) {
                            sceneAsset.LoadContent(scene);
                            this.CurrentSceneMetadata = metadata;
                            this._settingsService.Settings.LastSceneOpened = metadata.ContentId;
                            this._entityService.Selected = scene;
                            this._systemService.Selected = scene.Systems.FirstOrDefault();
                        }
                    }
                }
            }
            else {
                sceneAsset = null;
            }
        }

        return sceneAsset != null;
    }
}