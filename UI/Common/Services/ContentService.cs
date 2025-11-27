namespace Macabresoft.Macabre2D.UI.Common;

using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Macabresoft.AvaloniaEx;
using Macabresoft.Core;
using Macabresoft.Macabre2D.Common;
using Macabresoft.Macabre2D.Framework;
using Microsoft.Xna.Framework;
using ReactiveUI;

/// <summary>
/// An interface for a service which loads, imports, deletes, and moves content.
/// </summary>
public interface IContentService : ISelectionService<IContentNode> {
    /// <summary>
    /// Gets the root content directory.
    /// </summary>
    IContentDirectory RootContentDirectory { get; }

    /// <summary>
    /// Adds a directory as a child to selected directory.
    /// </summary>
    /// <param name="parent">The parent.</param>
    IContentDirectory AddDirectory(IContentDirectory parent);

    /// <summary>
    /// Adds a physics material to the selected directory.
    /// </summary>
    /// <param name="parent">The parent directory.</param>
    /// <returns>The content node for the physics material.</returns>
    IContentNode AddPhysicsMaterial(IContentDirectory parent);

    /// <summary>
    /// Adds a scene to the selected directory.
    /// </summary>
    /// <param name="parent">The parent.</param>
    /// <param name="template">The template</param>
    /// <returns>The content node for the scene.</returns>
    IContentNode AddScene(IContentDirectory parent, SceneTemplate template);

    /// <summary>
    /// Creates a prefab from the given entity.
    /// </summary>
    /// <param name="entity">The entity.</param>
    /// <returns>A task.</returns>
    Task CreatePrefab(IEntity entity);

    /// <summary>
    /// Creates a safe name for the given directory.
    /// </summary>
    /// <param name="baseName">The base name.</param>
    /// <param name="parent">The parent directory.</param>
    /// <returns>A safe name for a unique file or directory name under the parent.</returns>
    string CreateSafeName(string baseName, IContentDirectory parent);

    /// <summary>
    /// Imports content
    /// </summary>
    /// <param name="parent"></param>
    Task ImportContent(IContentDirectory parent);

    /// <summary>
    /// Moves the content to a new folder.
    /// </summary>
    /// <param name="contentToMove">The content to move.</param>
    /// <param name="newParent">The new parent.</param>
    void MoveContent(IContentNode contentToMove, IContentDirectory newParent);

    /// <summary>
    /// Refreshes the content.
    /// </summary>
    /// <param name="forceRebuild">A value indicating whether or not rebuild should be forced.</param>
    void RefreshContent(bool forceRebuild);

    /// <summary>
    /// Saves content with changes.
    /// </summary>
    void Save();

    /// <summary>
    /// Saves the metadata.
    /// </summary>
    /// <param name="metadata">The metadata.</param>
    void SaveMetadata(ContentMetadata metadata);
}

/// <summary>
/// A service which loads, imports, deletes, and moves content.
/// </summary>
public sealed class ContentService : SelectionService<IContentNode>, IContentService {
    private readonly IAssetManager _assetManager;
    private readonly IBuildService _buildService;
    private readonly ICommonDialogService _dialogService;
    private readonly IFileSystemService _fileSystem;
    private readonly IPathService _pathService;
    private readonly ISerializer _serializer;
    private readonly IEditorSettingsService _settingsService;

    private RootContentDirectory _rootContentDirectory;

    /// <summary>
    /// Initializes a new instance of the <see cref="ContentService" /> class.
    /// </summary>
    /// <param name="assemblyService">The assembly service.</param>
    /// <param name="assetManager">The asset manager.</param>
    /// <param name="buildService">The build service.</param>
    /// <param name="dialogService">The dialog service.</param>
    /// <param name="fileSystem">The file system service.</param>
    /// <param name="pathService">The path service.</param>
    /// <param name="serializer">The serializer.</param>
    /// <param name="settingsService">The settings service.</param>
    /// <param name="undoService">The undo service.</param>
    /// <param name="valueControlService">The value editor service.</param>
    public ContentService(
        IAssemblyService assemblyService,
        IAssetManager assetManager,
        IBuildService buildService,
        ICommonDialogService dialogService,
        IFileSystemService fileSystem,
        IPathService pathService,
        ISerializer serializer,
        IEditorSettingsService settingsService,
        IUndoService undoService,
        IValueControlService valueControlService) : base(assemblyService, undoService, valueControlService) {
        this._assetManager = assetManager;
        this._buildService = buildService;
        this._dialogService = dialogService;
        this._fileSystem = fileSystem;
        this._pathService = pathService;
        this._serializer = serializer;
        this._settingsService = settingsService;
    }

    /// <inheritdoc />
    public IContentDirectory RootContentDirectory => this._rootContentDirectory;

    /// <inheritdoc />
    public IContentDirectory AddDirectory(IContentDirectory parent) => parent != null ? this.CreateDirectory("New Directory", parent) : null;

    /// <inheritdoc />
    public IContentNode AddPhysicsMaterial(IContentDirectory parent) {
        ContentFile contentFile = null;
        if (parent != null) {
            contentFile = this.CreateAndBuildContentFile(
                new PhysicsMaterial(),
                parent,
                "New Physics Material",
                PhysicsMaterialAsset.FileExtension);
        }

        return contentFile;
    }

    /// <inheritdoc />
    public IContentNode AddScene(IContentDirectory parent, SceneTemplate template) {
        ContentFile contentFile = null;
        if (parent != null) {
            var scene = new Scene {
                Name = "New Scene"
            };

            foreach (var systemType in template.DefaultSystems) {
                if (Activator.CreateInstance(systemType) is IGameSystem system) {
                    scene.AddSystem(system);
                }
            }

            contentFile = this.CreateAndBuildContentFile(scene, parent, scene.Name, SceneAsset.FileExtension);
        }

        return contentFile;
    }

    /// <inheritdoc />
    public async Task CreatePrefab(IEntity entity) {
        if (entity?.TryClone(out var prefabChild) == true) {
            prefabChild.LocalPosition = Vector2.Zero;

            var prefab = new Entity {
                Name = !string.IsNullOrEmpty(prefabChild.Name) ? $"{prefabChild.Name}" : "Prefab"
            };

            prefab.AddChild(prefabChild);

            var result = await this._dialogService.OpenContentSelectionDialog(typeof(PrefabAsset), true, "Save the Prefab");
            var parent = result as IContentDirectory ?? result?.Parent;

            if (parent != null) {
                var fileName = result is IContentDirectory ? $"{this.CreateSafeName(prefab.Name, parent)}{PrefabAsset.FileExtension}" : result.Name;
                var fullPath = Path.Combine(parent.GetFullPath(), fileName);
                this._serializer.Serialize(prefab, fullPath);

                ContentFile contentFile = null;
                switch (result) {
                    case IContentDirectory: {
                        contentFile = this.CreateContentFile(parent, fileName);
                        break;
                    }
                    case ContentFile { Asset: PrefabAsset asset } file:
                        asset.LoadContent(prefab);
                        this.SaveMetadata(file.Metadata);
                        contentFile = file;
                        break;
                }

                if (contentFile != null) {
                    this.CopyToEditorBin(parent, contentFile);
                    this._buildService.CreateMGCBFile(this.RootContentDirectory, out _);
                    this._settingsService.Settings.ShouldRebuildContent = true;
                }
            }
        }
    }

    /// <inheritdoc />
    public string CreateSafeName(string baseName, IContentDirectory parent) {
        var name = baseName;
        var parentPath = parent.GetFullPath();
        var currentCount = 0;
        var files = this._fileSystem.GetFiles(parentPath).ToList();

        while (files.Any(x => string.Equals(Path.GetFileNameWithoutExtension(x), name, StringComparison.OrdinalIgnoreCase))) {
            currentCount++;
            if (currentCount >= 100) {
                throw new NotSupportedException("What the hell are you even doing with 100 files named the same????");
            }

            name = $"{baseName} ({currentCount})";
        }

        return name;
    }

    /// <inheritdoc />
    public async Task ImportContent(IContentDirectory parent) {
        if (parent != null) {
            var filePath = await this._dialogService.ShowSingleFileSelectionDialog("Import an Asset");

            if (!string.IsNullOrEmpty(filePath)) {
                var parentDirectoryPath = parent.GetFullPath();
                var fileName = $"{this.CreateSafeName(Path.GetFileNameWithoutExtension(filePath), parent)}{Path.GetExtension(filePath)}";
                var newFilePath = Path.Combine(parentDirectoryPath, fileName);

                await Task.Run(() => { this._fileSystem.CopyFile(filePath, newFilePath); });

                this.ImportContentFile(parent, fileName);
            }
        }
    }

    /// <inheritdoc />
    public void MoveContent(IContentNode contentToMove, IContentDirectory newParent) {
        contentToMove.ChangeParent(newParent);
    }

    /// <inheritdoc />
    public void RefreshContent(bool forceRebuild) {
        if (!string.IsNullOrWhiteSpace(this._pathService.PlatformsDirectoryPath)) {
            this._fileSystem.CreateDirectory(this._pathService.PlatformsDirectoryPath);
            this._fileSystem.CreateDirectory(this._pathService.ContentDirectoryPath);
            this._fileSystem.CreateDirectory(this._pathService.EditorContentDirectoryPath);

            if (this._rootContentDirectory != null) {
                this._rootContentDirectory.PathChanged -= this.ContentNode_PathChanged;
            }

            this._rootContentDirectory = new RootContentDirectory(this._fileSystem, this._pathService);
            this._rootContentDirectory.PathChanged += this.ContentNode_PathChanged;

            var exitCode = this._buildService.BuildContentFromScratch(this._rootContentDirectory, forceRebuild || this.CheckForMetadataChanges());

            foreach (var metadata in this._rootContentDirectory.GetAllContentFiles().Select(x => x.Metadata)) {
                this._assetManager.RegisterMetadata(metadata);
            }

            if (exitCode == 0) {
                this.ResetAssets();
                if (this._settingsService.Settings is { } settings) {
                    settings.ShouldRebuildContent = false;
                }
            }

            this.RaisePropertyChanged(nameof(this.RootContentDirectory));
        }
    }

    /// <inheritdoc />
    public void Save() {
        var files = this.RootContentDirectory.GetAllContentFiles().Where(x => x.HasChanges);
        foreach (var file in files) {
            this.SaveMetadata(file.Metadata);
            file.HasChanges = false;
        }

        this.RefreshMgcbFiles();
    }

    /// <inheritdoc />
    public void SaveMetadata(ContentMetadata metadata) {
        if (this._fileSystem.DoesDirectoryExist(this._pathService.MetadataDirectoryPath)) {
            this._serializer.Serialize(metadata, Path.Combine(this._pathService.MetadataDirectoryPath, $"{metadata.ContentId}{ContentMetadata.FileExtension}"));
        }
    }

    /// <inheritdoc />
    protected override bool ShouldLoadEditors() => this.Selected != this.RootContentDirectory && base.ShouldLoadEditors();

    private bool CheckForMetadataChanges() {
        var editorMetadataFiles = this._fileSystem.DoesDirectoryExist(this._pathService.EditorMetadataDirectoryPath) ? this._fileSystem.GetFiles(this._pathService.EditorMetadataDirectoryPath).ToList() : null;
        return editorMetadataFiles == null ||
               editorMetadataFiles.Count != this._assetManager.LoadedMetadata.Count ||
               this._assetManager.LoadedMetadata.Any(metadata => !editorMetadataFiles.Contains(metadata.GetFileName()));
    }

    private void ContentNode_PathChanged(object sender, ValueChangedEventArgs<string> e) {
        switch (sender) {
            case IContentDirectory:
                this._fileSystem.MoveDirectory(e.OriginalValue, e.UpdatedValue);
                break;
            case ContentFile contentFile:
                this._fileSystem.MoveFile(e.OriginalValue, e.UpdatedValue);
                this.SaveMetadata(contentFile.Metadata);
                break;
        }
    }


    private void CopyToEditorBin(IContentNode parent, ContentFile file) {
        var binPath = Path.Combine(this._pathService.EditorContentDirectoryPath, parent.GetContentPath());
        this._fileSystem.CreateDirectory(binPath);
        var binFilePath = Path.Combine(binPath, file.Name);
        this._fileSystem.DeleteFile(binFilePath);
        this._fileSystem.CopyFile(file.GetFullPath(), binFilePath);

        if (file.Id != Guid.Empty) {
            var metadataPath = this._pathService.GetMetadataFilePath(file.Id);
            var binMetadataPath = this._pathService.GetEditorMetadataFilePath(file.Id);

            if (this._fileSystem.DoesFileExist(metadataPath)) {
                this._fileSystem.CreateDirectory(Path.GetDirectoryName(binMetadataPath));
                this._fileSystem.DeleteFile(binMetadataPath);
                this._fileSystem.CopyFile(metadataPath, binMetadataPath);
            }
        }
    }

    private ContentFile CreateAndBuildContentFile<TContent>(TContent content, IContentDirectory parent, string name, string fileExtension) {
        name = this.CreateSafeName(name, parent);

        var fileName = $"{name}{fileExtension}";
        var fullPath = Path.Combine(parent.GetFullPath(), fileName);
        this._serializer.Serialize(content, fullPath);
        var contentFile = this.CreateContentFile(parent, fileName);

        if (contentFile != null) {
            this.CopyToEditorBin(parent, contentFile);
            this._buildService.CreateMGCBFile(this.RootContentDirectory, out _);
            this._settingsService.Settings.ShouldRebuildContent = true;
        }

        return contentFile;
    }

    private ContentFile CreateContentFile(IContentDirectory parent, string fileName) {
        var extension = Path.GetExtension(fileName);
        ContentFile contentFile = null;

        if (BuildService.FileExtensionToAssetType.TryGetValue(extension, out var assetType)) {
            var parentPath = parent.GetContentPath();
            var splitPath = parentPath.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries).ToList();
            splitPath.Add(Path.GetFileNameWithoutExtension(fileName));

            if (Activator.CreateInstance(assetType) is IAsset asset) {
                var metadata = new ContentMetadata(asset, splitPath, extension);
                this.SaveMetadata(metadata);
                contentFile = this.AssemblyService.CreateContentFileObject(parent, metadata);
                this._assetManager.RegisterMetadata(contentFile.Metadata);
            }
        }

        return contentFile;
    }

    private IContentDirectory CreateDirectory(string baseName, IContentDirectory parent) {
        var name = baseName;
        var parentPath = parent.GetFullPath();
        var fullPath = Path.Combine(parentPath, name);
        var currentCount = 0;

        while (this._fileSystem.DoesDirectoryExist(fullPath)) {
            currentCount++;
            if (currentCount >= 100) {
                throw new NotSupportedException("What the hell are you even doing with 100 directories named the same????");
            }

            name = $"{baseName} ({currentCount})";
            fullPath = Path.Combine(parentPath, name);
        }

        this._fileSystem.CreateDirectory(fullPath);
        return new ContentDirectory(name, parent);
    }

    private void ImportContentFile(IContentDirectory parent, string fileName) => this.CreateContentFile(parent, fileName);


    private void RefreshMgcbFiles() {
        this._buildService.CreateMGCBFile(this.RootContentDirectory, out _);
    }

    private void ResetAssets() {
        this._assetManager.Unload();
        var contentFiles = this._rootContentDirectory.GetAllContentFiles();
        foreach (var metadata in contentFiles.Select(x => x.Metadata).Where(x => x != null)) {
            this._assetManager.RegisterMetadata(metadata);
        }
    }
}