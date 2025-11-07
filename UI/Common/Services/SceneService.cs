namespace Macabresoft.Macabre2D.UI.Common;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using Macabresoft.AvaloniaEx;
using Macabresoft.Macabre2D.Common;
using Macabresoft.Macabre2D.Framework;
using ReactiveUI;

/// <summary>
/// Interface for a service which handles the <see cref="IScene" /> open in the editor.
/// </summary>
public interface ISceneService : ISelectionService<object> {

    /// <summary>
    /// Gets the current scene metadata.
    /// </summary>
    ContentMetadata CurrentMetadata { get; }

    /// <summary>
    /// Gets the object currently being edited. This is either a prefab or a scene.
    /// </summary>
    IEntity CurrentlyEditing { get; }
    
    /// <summary>
    /// Gets the current scene if editing a scene.
    /// </summary>
    IScene CurrentScene { get; }
    
    /// <summary>
    /// Gets the current prefab if editing a prefab.
    /// </summary>
    IEntity CurrentPrefab { get; }

    /// <summary>
    /// Gets the default scene template.
    /// </summary>
    DefaultSceneTemplate DefaultSceneTemplate { get; }

    /// <summary>
    /// Gets the implied selected object.
    /// </summary>
    object ImpliedSelected { get; }

    /// <summary>
    /// Gets a value indicating whether the state of the program is in an entity context.
    /// </summary>
    bool IsEntityContext { get; }
    
    /// <summary>
    /// Gets a value indicating whether this is editing a prefab.
    /// </summary>
    bool IsEditingPrefab { get; }

    /// <summary>
    /// Gets a list of available scene templates.
    /// </summary>
    IReadOnlyCollection<SceneTemplate> Templates { get; }

    /// <summary>
    /// Raises the property changed event for the selected object.
    /// </summary>
    void RaiseSelectedChanged();

    /// <summary>
    /// Saves the current scene.
    /// </summary>
    void SaveScene();

    /// <summary>
    /// Saves the provided scene and metadata.
    /// </summary>
    void SaveScene(ContentMetadata metadata, IScene scene);

    /// <summary>
    /// Tries to load a prefab.
    /// </summary>
    /// <param name="contentId">The content identifier of the prefab.</param>
    /// <returns>A value indicating whether the prefab was loaded.</returns>
    bool TryLoadPrefab(Guid contentId);

    /// <summary>
    /// Tries to load a scene.
    /// </summary>
    /// <param name="contentId">The content identifier of the scene.</param>
    /// <param name="sceneAsset">The scene asset.</param>
    /// <returns>A value indicating whether the scene was loaded.</returns>
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
    private ContentMetadata _currentMetadata;
    private object _impliedSelected;
    private bool _isEntityContext;
    private object _selected;

    /// <summary>
    /// Initializes a new instance of the <see cref="SceneService" /> class.
    /// </summary>
    /// <param name="assemblyService">The assembly service.</param>
    /// <param name="entityService">The entity service.</param>
    /// <param name="fileSystem">The file system service.</param>
    /// <param name="pathService">The path service.</param>
    /// <param name="serializer">The serializer.</param>
    /// <param name="settingsService">The settings service.</param>
    /// <param name="systemService">The system service.</param>
    public SceneService(
        IAssemblyService assemblyService,
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

        var templateTypes = assemblyService.LoadTypes(typeof(SceneTemplate)).Where(x => x.GetConstructor(Type.EmptyTypes) != null).ToList();
        this.Templates = templateTypes.Select(type => Activator.CreateInstance(type) as SceneTemplate).ToList();
        this.DefaultSceneTemplate = new DefaultSceneTemplate();
    }

    /// <inheritdoc />
    public ContentMetadata CurrentMetadata {
        get => this._currentMetadata;
        private set {
            this.RaiseAndSetIfChanged(ref this._currentMetadata, value);
            this.RaisePropertyChanged(nameof(this.CurrentlyEditing));
            this.IsEditingPrefab = this.CurrentMetadata?.Asset is PrefabAsset;
        }
    }

    /// <inheritdoc />
    public IEntity CurrentlyEditing {
        get {
            if (this._currentMetadata != null) {
                switch (this._currentMetadata.Asset) {
                    case SceneAsset sceneAsset:
                        return sceneAsset.Content;
                    case PrefabAsset prefabAsset:
                        return prefabAsset.Content;
                }
            }

            return null;
        }
    }

    /// <inheritdoc />
    public IScene CurrentScene => this.CurrentlyEditing as IScene ?? (this.CurrentlyEditing is { } entity ? entity.Scene : null);
    
    /// <inheritdoc />
    public IEntity CurrentPrefab => this.CurrentlyEditing is not IScene ? this.CurrentlyEditing : null;

    /// <inheritdoc />
    public DefaultSceneTemplate DefaultSceneTemplate { get; }

    /// <inheritdoc />
    public IReadOnlyCollection<ValueControlCollection> Editors {
        get {
            return this._selected switch {
                IEntity => this._entityService.Editors,
                IGameSystem => this._systemService.Editors,
                _ => null
            };
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

    private bool _isEditingPrefab;

    /// <inheritdoc />
    public bool IsEditingPrefab {
        get => this._isEditingPrefab;
        private set => this.RaiseAndSetIfChanged(ref this._isEditingPrefab, value);
    }

    /// <inheritdoc />
    public object Selected {
        get => this._selected;
        set {
            this.RaiseAndSetIfChanged(ref this._selected, value);

            switch (this._selected) {
                case IScene scene:
                    this._entityService.Selected = scene;
                    this._systemService.Selected = null;
                    this.ImpliedSelected = this._selected;
                    this.IsEntityContext = true;
                    break;
                case IGameSystem system:
                    this._systemService.Selected = system;
                    this._entityService.Selected = null;
                    this.ImpliedSelected = this._selected;
                    this.IsEntityContext = false;
                    break;
                case IEntity entity:
                    this._entityService.Selected = entity;
                    this._systemService.Selected = null;
                    this.ImpliedSelected = this._selected;
                    this.IsEntityContext = true;
                    break;
                case SystemCollection:
                    this._entityService.Selected = null;
                    this._systemService.Selected = null;
                    this.IsEntityContext = false;
                    this._entityService.Selected = null;
                    this.ImpliedSelected = this.CurrentlyEditing;
                    break;
                case EntityCollection:
                case null:
                    this._entityService.Selected = null;
                    this._systemService.Selected = null;
                    this.IsEntityContext = true;
                    this._entityService.Selected = null;
                    this.ImpliedSelected = this.CurrentlyEditing;
                    break;
            }

            this.RaisePropertyChanged(nameof(this.Editors));
        }
    }

    /// <inheritdoc />
    public IReadOnlyCollection<SceneTemplate> Templates { get; }

    /// <inheritdoc />
    public void RaiseSelectedChanged() {
        var originalSelected = this._selected;
        var originalImpliedSelected = this._impliedSelected;
        this._selected = null;
        this._impliedSelected = null;
        this.RaisePropertyChanged(nameof(this.Selected));
        this.RaisePropertyChanged(nameof(this.ImpliedSelected));
        this._selected = originalSelected;
        this._impliedSelected = originalImpliedSelected;
        this.RaisePropertyChanged(nameof(this.Selected));
        this.RaisePropertyChanged(nameof(this.ImpliedSelected));
    }

    /// <inheritdoc />
    public void SaveScene() {
        switch (this.CurrentMetadata.Asset) {
            case SceneAsset sceneAsset:
                this.SaveScene(this.CurrentMetadata, sceneAsset.Content);
                break;
            case PrefabAsset prefabAsset:
                this.SavePrefab(this.CurrentMetadata, prefabAsset.Content);
                break;
        }
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

    public bool TryLoadPrefab(Guid contentId) {
        if (this.TryGetMetadata<PrefabAsset, Entity>(contentId, out var metadata, out var prefabAsset, out var entity)) {
            var tempScene = new Scene();
            tempScene.AddChild(entity);
            this.SelectAndOpen(entity, metadata);
            return true;
        }

        return false;
    }

    /// <inheritdoc />
    public bool TryLoadScene(Guid contentId, out SceneAsset sceneAsset) {
        if (this.TryGetMetadata<SceneAsset, Scene>(contentId, out var metadata, out sceneAsset, out var scene)) {
            this.SelectAndOpen(scene, metadata);
        }
        else {
            sceneAsset = null;
        }

        return sceneAsset != null;
    }

    private void SelectAndOpen(IEntity entity, ContentMetadata metadata) {
        this.CurrentMetadata = metadata;
        this._settingsService.SetPreviouslyOpenedContent(metadata);
        this._entityService.Selected = entity;
        this._impliedSelected = entity;

        if (entity is IScene scene) {
            this._systemService.Selected = scene.Systems.FirstOrDefault();
        }
        else {
            this._systemService.Selected = null;
        }
    }

    private void SavePrefab(ContentMetadata metadata, IEntity prefab) {
        if (metadata != null && prefab != null && metadata.Asset is IAsset<Entity>) {
            var metadataPath = this._pathService.GetMetadataFilePath(metadata.ContentId);
            this._serializer.Serialize(metadata, metadataPath);

            var filePath = Path.Combine(this._pathService.ContentDirectoryPath, metadata.GetContentPath());
            this._serializer.Serialize(prefab, filePath);
        }
    }

    private bool TryGetMetadata<TAsset, TContent>(
        Guid contentId,
        [NotNullWhen(true)] out ContentMetadata metadata,
        [NotNullWhen(true)] out TAsset asset,
        [NotNullWhen(true)] out TContent content)
        where TAsset : class, IAsset<TContent>
        where TContent : class {
        metadata = null;
        asset = null;
        content = null;

        if (contentId == Guid.Empty) {
            return false;
        }

        var metadataFilePath = this._pathService.GetMetadataFilePath(contentId);
        if (this._fileSystem.DoesFileExist(metadataFilePath)) {
            metadata = this._serializer.Deserialize<ContentMetadata>(metadataFilePath);

            if (metadata != null) {
                asset = metadata.Asset as TAsset;

                if (asset != null) {
                    var contentPath = Path.Combine(this._pathService.ContentDirectoryPath, metadata.GetContentPath());

                    if (this._fileSystem.DoesFileExist(contentPath)) {
                        try {
                            content = this._serializer.Deserialize<TContent>(contentPath);
                            if (content != null) {
                                asset.LoadContent(content);
                            }
                        }
                        catch {
                            // TODO: might be good to show or log an error message here
                            asset = null;
                            content = null;
                        }
                    }
                }
            }
        }

        return metadata != null && asset != null && content != null;
    }
}