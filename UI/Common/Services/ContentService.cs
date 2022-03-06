namespace Macabresoft.Macabre2D.UI.Common;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Macabresoft.AvaloniaEx;
using Macabresoft.Macabre2D.Framework;
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
    /// Adds a scene to the selected directory.
    /// </summary>
    /// <param name="parent">The parent.</param>
    void AddScene(IContentDirectory parent);

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
    void RefreshContent(bool forceRebuild);

    /// <summary>
    /// Saves content with changes.
    /// </summary>
    void Save();
}

/// <summary>
/// A service which loads, imports, deletes, and moves content.
/// </summary>
public sealed class ContentService : SelectionService<IContentNode>, IContentService {
    private static readonly IDictionary<string, Type> FileExtensionToAssetType = new Dictionary<string, Type>();
    private readonly IAssetManager _assetManager;
    private readonly IBuildService _buildService;
    private readonly ICommonDialogService _dialogService;
    private readonly IFileSystemService _fileSystem;
    private readonly ILoggingService _loggingService;
    private readonly IPathService _pathService;
    private readonly ISerializer _serializer;
    private readonly IEditorSettingsService _settingsService;

    private RootContentDirectory _rootContentDirectory;

    /// <summary>
    /// Static constructor for <see cref="ContentService" />.
    /// </summary>
    static ContentService() {
        FileExtensionToAssetType.Add(SceneAsset.FileExtension, typeof(SceneAsset));
        FileExtensionToAssetType.Add(PrefabAsset.FileExtension, typeof(PrefabAsset));

        foreach (var extension in SpriteSheetAsset.ValidFileExtensions) {
            FileExtensionToAssetType.Add(extension, typeof(SpriteSheetAsset));
        }

        foreach (var extension in AudioClipAsset.ValidFileExtensions) {
            FileExtensionToAssetType.Add(extension, typeof(AudioClipAsset));
        }

        FileExtensionToAssetType.Add(ShaderAsset.FileExtension, typeof(ShaderAsset));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ContentService" /> class.
    /// </summary>
    /// <param name="assemblyService">The assembly service.</param>
    /// <param name="assetManager">The asset manager.</param>
    /// <param name="buildService">The build service.</param>
    /// <param name="dialogService">The dialog service.</param>
    /// <param name="fileSystem">The file system service.</param>
    /// <param name="loggingService">The logging service.</param>
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
        ILoggingService loggingService,
        IPathService pathService,
        ISerializer serializer,
        IEditorSettingsService settingsService,
        IUndoService undoService,
        IValueControlService valueControlService) : base(assemblyService, undoService, valueControlService) {
        this._assetManager = assetManager;
        this._buildService = buildService;
        this._dialogService = dialogService;
        this._fileSystem = fileSystem;
        this._loggingService = loggingService;
        this._pathService = pathService;
        this._serializer = serializer;
        this._settingsService = settingsService;
    }

    /// <inheritdoc />
    public IContentDirectory RootContentDirectory => this._rootContentDirectory;

    /// <inheritdoc />
    public IContentDirectory AddDirectory(IContentDirectory parent) {
        return parent != null ? this.CreateDirectory("New Directory", parent) : null;
    }

    /// <inheritdoc />
    public void AddScene(IContentDirectory parent) {
        if (parent != null) {
            var name = this.CreateSafeName("New Scene", parent);
            var scene = new Scene {
                Name = name
            };

            var fileName = $"{name}{SceneAsset.FileExtension}";
            var fullPath = Path.Combine(parent.GetFullPath(), fileName);
            this._serializer.Serialize(scene, fullPath);
            this.CreateContentFile(parent, fileName, out var contentFile);

            if (contentFile != null) {
                this.CopyToEditorBin(parent, contentFile);
                this.CreateMGCBFile(out _);
                this._settingsService.Settings.ShouldRebuildContent = true;
            }
        }
    }

    /// <inheritdoc />
    public async Task CreatePrefab(IEntity entity) {
        if (entity?.TryClone(out var prefabChild) == true) {
            var prefab = new Entity {
                Name = !string.IsNullOrEmpty(prefabChild.Name) ? $"{prefabChild.Name} Prefab" : "Prefab"
            };

            prefab.AddChild(prefabChild);

            var result = await this._dialogService.OpenAssetSelectionDialog(typeof(PrefabAsset), true);
            var parent = result as IContentDirectory ?? result.Parent;

            if (parent != null) {
                var fileName = result is IContentDirectory ? $"{this.CreateSafeName(prefab.Name, parent)}{PrefabAsset.FileExtension}" : result.Name;
                var fullPath = Path.Combine(parent.GetFullPath(), fileName);
                this._serializer.Serialize(prefab, fullPath);

                ContentFile contentFile = null;
                switch (result) {
                    case IContentDirectory: {
                        this.CreateContentFile(parent, fileName, out contentFile);
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
                    this.CreateMGCBFile(out _);
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

                this.CreateContentFile(parent, fileName);
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

            foreach (var metadata in this.GetMetadata()) {
                this.ResolveContentFile(metadata);
            }

            if (this.ResolveNewContentFiles(this._rootContentDirectory) || forceRebuild || this.CheckForMetadataChanges()) {
                this.ResetAssets();
                this.BuildContentForProject();
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

        this.RefreshMGCBFiles();
    }

    /// <inheritdoc />
    protected override bool ShouldLoadEditors() {
        return this.Selected != this.RootContentDirectory && base.ShouldLoadEditors();
    }

    private void BuildContentForProject() {
        var buildArgs = this.CreateMGCBFile(out var outputDirectoryPath);
        this._buildService.BuildContent(buildArgs, outputDirectoryPath);

        if (this._settingsService.Settings is { } settings) {
            settings.ShouldRebuildContent = false;
        }
    }

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

    private bool CreateContentFile(IContentDirectory parent, string fileName) {
        return this.CreateContentFile(parent, fileName, out _);
    }

    private bool CreateContentFile(IContentDirectory parent, string fileName, out ContentFile contentFile) {
        var result = false;
        var extension = Path.GetExtension(fileName);
        contentFile = null;

        if (FileExtensionToAssetType.TryGetValue(extension, out var assetType)) {
            var parentPath = parent.GetContentPath();
            var splitPath = parentPath.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries).ToList();
            splitPath.Add(Path.GetFileNameWithoutExtension(fileName));

            if (Activator.CreateInstance(assetType) is IAsset asset) {
                var metadata = new ContentMetadata(asset, splitPath, extension);
                this.SaveMetadata(metadata);
                contentFile = this.CreateContentFileObject(parent, metadata);
                this._assetManager.RegisterMetadata(contentFile.Metadata);
                result = true;
            }
        }

        return result;
    }

    private ContentFile CreateContentFileObject(IContentDirectory parent, ContentMetadata metadata) {
        ContentFile contentFile = null;
        var contentFileType = this.AssemblyService.LoadFirstGenericType(typeof(ContentFile<>), metadata.Asset.GetType());
        if (contentFileType != null) {
            contentFile = this.AssemblyService.CreateObjectFromType(contentFileType, parent, metadata) as ContentFile;
        }

        return contentFile ?? new ContentFile(parent, metadata);
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

    private BuildContentArguments CreateMGCBFile(out string outputDirectoryPath) {
        const string Platform = "DesktopGL";
        var mgcbStringBuilder = new StringBuilder();
        var mgcbFilePath = Path.Combine(this._pathService.ContentDirectoryPath, $"Content.{Platform}.mgcb");
        var buildArgs = new BuildContentArguments(
            mgcbFilePath,
            Platform,
            true);

        outputDirectoryPath = Path.GetRelativePath(this._pathService.ContentDirectoryPath, this._pathService.EditorContentDirectoryPath);

        mgcbStringBuilder.AppendLine("#----------------------------- Global Properties ----------------------------#");
        mgcbStringBuilder.AppendLine();

        foreach (var argument in buildArgs.ToGlobalProperties(outputDirectoryPath)) {
            mgcbStringBuilder.AppendLine(argument);
        }

        mgcbStringBuilder.AppendLine();
        mgcbStringBuilder.AppendLine(@"#-------------------------------- References --------------------------------#");
        mgcbStringBuilder.AppendLine();
        mgcbStringBuilder.AppendLine();
        mgcbStringBuilder.AppendLine(@"#---------------------------------- Content ---------------------------------#");
        mgcbStringBuilder.AppendLine();

        mgcbStringBuilder.AppendLine($"#begin {GameProject.ProjectFileName}");
        mgcbStringBuilder.AppendLine($@"/copy:{GameProject.ProjectFileName}");
        mgcbStringBuilder.AppendLine($"#end {GameProject.ProjectFileName}");
        mgcbStringBuilder.AppendLine();

        var contentFiles = this.RootContentDirectory.GetAllContentFiles();
        foreach (var contentFile in contentFiles) {
            mgcbStringBuilder.AppendLine(contentFile.Metadata.GetContentBuildCommands());
            mgcbStringBuilder.AppendLine();
        }

        var mgcbText = mgcbStringBuilder.ToString();
        this._fileSystem.WriteAllText(mgcbFilePath, mgcbText);

        return buildArgs;
    }

    private IEnumerable<ContentMetadata> GetMetadata() {
        var metadata = new List<ContentMetadata>();
        if (this._fileSystem.DoesDirectoryExist(this._pathService.MetadataDirectoryPath)) {
            var files = this._fileSystem.GetFiles(this._pathService.MetadataDirectoryPath, ContentMetadata.MetadataSearchPattern);
            foreach (var file in files) {
                try {
                    var contentMetadata = this._serializer.Deserialize<ContentMetadata>(file);
                    metadata.Add(contentMetadata);
                }
                catch (Exception e) {
                    var fileName = Path.GetFileName(file);
                    var message = $"Archiving metadata '{fileName}' due to an exception";
                    this._loggingService.LogException(message, e);
                    this._fileSystem.CreateDirectory(this._pathService.MetadataArchiveDirectoryPath);
                    this._fileSystem.MoveFile(file, Path.Combine(this._pathService.MetadataArchiveDirectoryPath, fileName));
                }
            }
        }

        return metadata;
    }

    private void RefreshMGCBFiles() {
        this.CreateMGCBFile(out _);
    }

    private void ResetAssets() {
        this._assetManager.Unload();
        var contentFiles = this._rootContentDirectory.GetAllContentFiles();
        foreach (var metadata in contentFiles.Select(x => x.Metadata).Where(x => x != null)) {
            this._assetManager.RegisterMetadata(metadata);
        }
    }


    private void ResolveContentFile(ContentMetadata metadata) {
        ContentFile contentNode = null;
        var splitPath = metadata.SplitContentPath;
        if (splitPath.Any()) {
            IContentDirectory parentDirectory;
            if (splitPath.Count == this._rootContentDirectory.GetDepth() + 1) {
                parentDirectory = this._rootContentDirectory;
            }
            else {
                parentDirectory = this._rootContentDirectory.TryFindNode(splitPath.Take(splitPath.Count - 1).ToArray()) as IContentDirectory;
            }

            if (parentDirectory != null) {
                var contentFilePath = Path.Combine(parentDirectory.GetFullPath(), metadata.GetFileName());
                if (this._fileSystem.DoesFileExist(contentFilePath)) {
                    contentNode = this.CreateContentFileObject(parentDirectory, metadata);
                }
            }
        }

        if (contentNode == null) {
            var fileName = $"{metadata.ContentId}{ContentMetadata.FileExtension}";
            var current = Path.Combine(this._pathService.MetadataDirectoryPath, fileName);
            var moveTo = Path.Combine(this._pathService.MetadataArchiveDirectoryPath, fileName);

            if (!this._fileSystem.DoesDirectoryExist(this._pathService.MetadataArchiveDirectoryPath)) {
                this._fileSystem.CreateDirectory(this._pathService.MetadataArchiveDirectoryPath);
            }

            if (this._fileSystem.DoesFileExist(current)) {
                this._fileSystem.MoveFile(current, moveTo);
            }
        }
        else {
            this._assetManager.RegisterMetadata(metadata);
        }
    }

    private bool ResolveNewContentFiles(IContentDirectory currentDirectory) {
        var result = false;
        var currentPath = currentDirectory.GetFullPath();
        var files = this._fileSystem.GetFiles(currentPath);
        var currentContentFiles = currentDirectory.Children.OfType<ContentFile>().ToList();

        foreach (var file in files.Select(Path.GetFileName).Where(fileName => currentContentFiles.All(x => x.Name != fileName))) {
            result = this.CreateContentFile(currentDirectory, file) || result;
        }

        var currentContentDirectories = currentDirectory.Children.OfType<IContentDirectory>();
        foreach (var child in currentContentDirectories) {
            result = this.ResolveNewContentFiles(child) || result;
        }

        return result;
    }

    private void SaveMetadata(ContentMetadata metadata) {
        if (this._fileSystem.DoesDirectoryExist(this._pathService.MetadataDirectoryPath)) {
            this._serializer.Serialize(metadata, Path.Combine(this._pathService.MetadataDirectoryPath, $"{metadata.ContentId}{ContentMetadata.FileExtension}"));
        }
    }
}