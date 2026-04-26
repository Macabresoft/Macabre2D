namespace Macabre2D.UI.Common;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Macabre2D.Common;
using Macabre2D.Framework;
using Macabresoft.AvaloniaEx;
using Macabresoft.Core;

/// <summary>
/// Interface that abstracts out building content and projects.
/// </summary>
public interface IBuildService {
    /// <summary>
    /// Builds the content.
    /// </summary>
    /// <param name="args">The arguments.</param>
    /// <returns>The exit code of the MGCB process.</returns>
    int BuildContent(BuildContentArguments args);

    /// <summary>
    /// Builds the content with an output directory in mind.
    /// </summary>
    /// <param name="args">The arguments.</param>
    /// <param name="outputDirectoryPath">The path to the output directory.</param>
    /// <returns>The exit code of the MGCB process.</returns>
    int BuildContent(BuildContentArguments args, string outputDirectoryPath);

    /// <summary>
    /// Builds the content from scratch.
    /// </summary>
    /// <param name="contentDirectory">The content directory.</param>
    /// <param name="forceRebuild">A value indicating whether a rebuild should be forced.</param>
    /// <param name="newMetadata">Any new metadata that was created during tyhe build process.</param>
    /// <returns>The exit code of the MGCB process.</returns>
    int BuildContentFromScratch(IContentDirectory contentDirectory, bool forceRebuild, out IEnumerable<ContentMetadata> newMetadata);

    /// <summary>
    /// Gets the metadata from files.
    /// </summary>
    /// <returns></returns>
    IEnumerable<ContentMetadata> GetMetadata();

    /// <summary>
    /// Creates the MGCB file.
    /// </summary>
    /// <param name="contentDirectory">The content directory.</param>
    /// <param name="outputDirectoryPath">The output directory path.</param>
    /// <param name="buildArgs">The build arguments and the output directory path.</param>
    /// <returns>A value indicating whether a new MGCB file was created.</returns>
    bool TryCreateMGCBFile(IContentDirectory contentDirectory, out string outputDirectoryPath, out BuildContentArguments buildArgs);
}

/// <summary>
/// Service which abstracts out building content and projects.
/// </summary>
public class BuildService : IBuildService {
    /// <summary>
    /// Maps file extensions to asset types.
    /// </summary>
    public static readonly IDictionary<string, Type> FileExtensionToAssetType = new Dictionary<string, Type>();

    private readonly IAssemblyService _assemblyService;
    private readonly IFileSystemService _fileSystem;
    private readonly IPathService _pathService;
    private readonly IProcessService _processService;
    private readonly ISerializer _serializer;

    /// <summary>
    /// Static constructor for <see cref="BuildService" />.
    /// </summary>
    static BuildService() {
        FileExtensionToAssetType.Add(SceneAsset.FileExtension, typeof(SceneAsset));
        FileExtensionToAssetType.Add(PrefabAsset.FileExtension, typeof(PrefabAsset));
        FileExtensionToAssetType.Add(ShaderAsset.FileExtension, typeof(ShaderAsset));
        FileExtensionToAssetType.Add(LegacyFontAsset.FileExtension, typeof(LegacyFontAsset));

        foreach (var extension in SpriteSheet.ValidFileExtensions) {
            FileExtensionToAssetType.Add(extension, typeof(SpriteSheet));
        }

        foreach (var extension in AudioClip.ValidFileExtensions) {
            FileExtensionToAssetType.Add(extension, typeof(AudioClip));
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BuildService" /> class.
    /// </summary>
    /// <param name="assemblyService">The assembly service.</param>
    /// <param name="fileSystem">The file system service.</param>
    /// <param name="pathService">The path service.</param>
    /// <param name="processService">The process service.</param>
    /// <param name="serializer">The serializer.</param>
    public BuildService(
        IAssemblyService assemblyService,
        IFileSystemService fileSystem,
        IPathService pathService,
        IProcessService processService,
        ISerializer serializer) {
        this._assemblyService = assemblyService;
        this._fileSystem = fileSystem;
        this._pathService = pathService;
        this._processService = processService;
        this._serializer = serializer;
    }

    /// <inheritdoc />
    public int BuildContent(BuildContentArguments args) => this.BuildContent(args, null);

    /// <inheritdoc />
    public int BuildContent(BuildContentArguments args, string outputDirectoryPath) {
        var exitCode = -1;
        if (!string.IsNullOrWhiteSpace(args.ContentFilePath) && this._fileSystem.DoesFileExist(args.ContentFilePath)) {
            var startInfo = new ProcessStartInfo {
                CreateNoWindow = true,
                UseShellExecute = true,
                FileName = "mgcb",
                WindowStyle = ProcessWindowStyle.Hidden,
                WorkingDirectory = Path.GetDirectoryName(args.ContentFilePath) ?? string.Empty
            };

            var arguments = !string.IsNullOrEmpty(outputDirectoryPath) ? args.ToConsoleArguments(outputDirectoryPath) : args.ToConsoleArguments();

            startInfo.ArgumentList.AddRange(arguments);
            exitCode = this._processService.StartProcess(startInfo);
        }

        return exitCode;
    }

    /// <inheritdoc />
    public int BuildContentFromScratch(IContentDirectory contentDirectory, bool forceRebuild, out IEnumerable<ContentMetadata> newMetadata) {
        var currentMetadata = this.GetMetadata();
        foreach (var metadata in currentMetadata) {
            this.ResolveContentFile(contentDirectory, metadata);
        }

        var requiresRebuild = this.ResolveNewContentFiles(contentDirectory, out newMetadata);
        if (requiresRebuild || forceRebuild) {
            if (this.TryCreateMGCBFile(contentDirectory, out var outputDirectoryPath, out var buildArgs) || forceRebuild) {
                return this.BuildContent(buildArgs, outputDirectoryPath);
            }

            return 0;
        }

        return -1;
    }

    /// <inheritdoc />
    public IEnumerable<ContentMetadata> GetMetadata() {
        var metadata = new List<ContentMetadata>();
        if (this._fileSystem.DoesDirectoryExist(this._pathService.MetadataDirectoryPath)) {
            var files = this._fileSystem.GetFiles(this._pathService.MetadataDirectoryPath, ContentMetadata.MetadataSearchPattern);
            var filesToDelete = new List<string>();
            foreach (var file in files) {
                try {
                    var contentMetadata = this._serializer.Deserialize<ContentMetadata>(file);
                    metadata.Add(contentMetadata);
                }
                catch {
                    // If you got a serialization problem, I feel bad for you, but I must delete your file.
                    // Reminder to use source control so you can catch when this happens.
                    filesToDelete.Add(file);
                }
            }

            foreach (var file in filesToDelete) {
                this._fileSystem.DeleteFile(file);
            }
        }

        return metadata;
    }

    /// <inheritdoc />
    public bool TryCreateMGCBFile(IContentDirectory contentDirectory, out string outputDirectoryPath, out BuildContentArguments buildArgs) {
        const string Platform = "DesktopGL";
        var mgcbStringBuilder = new StringBuilder();
        var mgcbFilePath = Path.Combine(this._pathService.ContentDirectoryPath, $"Content.{Platform}.mgcb");
        buildArgs = new BuildContentArguments(
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

        var contentFiles = contentDirectory.GetAllContentFiles();
        foreach (var contentFile in contentFiles) {
            mgcbStringBuilder.AppendLine(contentFile.Metadata.GetContentBuildCommands());
        }

        var mgcbText = mgcbStringBuilder.ToString();
        var shouldRebuild = false;

        if (this._fileSystem.DoesFileExist(mgcbFilePath)) {
            var existingText = File.ReadAllText(mgcbFilePath);
            if (!string.Equals(existingText, mgcbText)) {
                shouldRebuild = true;
                this._fileSystem.WriteAllText(mgcbFilePath, mgcbText);
            }
        }
        else {
            shouldRebuild = true;
            this._fileSystem.WriteAllText(mgcbFilePath, mgcbText);
        }

        return shouldRebuild;
    }

    private bool CreateContentFile(IContentDirectory parent, string fileName, out ContentMetadata? metadata) {
        var result = false;
        var extension = Path.GetExtension(fileName);
        metadata = null;

        if (FileExtensionToAssetType.TryGetValue(extension, out var assetType)) {
            var parentPath = parent.GetContentPath();
            var splitPath = parentPath.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries).ToList();
            splitPath.Add(Path.GetFileNameWithoutExtension(fileName));

            if (Activator.CreateInstance(assetType) is IAsset asset) {
                metadata = new ContentMetadata(asset, splitPath, extension);
                this._assemblyService.CreateContentFileObject(parent, metadata);
                result = true;
            }
        }

        return result;
    }

    private void ResolveContentFile(IContentDirectory rootContentDirectory, ContentMetadata metadata) {
        ContentFile contentNode = null;
        var splitPath = metadata.SplitContentPath;
        if (splitPath.Any()) {
            IContentDirectory parentDirectory;
            if (splitPath.Count == rootContentDirectory.GetDepth() + 1) {
                parentDirectory = rootContentDirectory;
            }
            else {
                parentDirectory = rootContentDirectory.TryFindNode(splitPath.Take(splitPath.Count - 1).ToArray()) as IContentDirectory;
            }

            if (parentDirectory != null) {
                var contentFilePath = Path.Combine(parentDirectory.GetFullPath(), metadata.GetFileName());
                if (this._fileSystem.DoesFileExist(contentFilePath)) {
                    contentNode = this._assemblyService.CreateContentFileObject(parentDirectory, metadata);
                }
            }
        }

        if (contentNode == null) {
            var fileName = $"{metadata.ContentId}{ContentMetadata.FileExtension}";
            var current = Path.Combine(this._pathService.MetadataDirectoryPath, fileName);
            this._fileSystem.DeleteFile(current);
        }
    }

    private bool ResolveNewContentFiles(IContentDirectory currentDirectory, out IEnumerable<ContentMetadata> metadata) {
        var foundMetadata = new List<ContentMetadata>();
        var result = false;
        var currentPath = currentDirectory.GetFullPath();
        var files = this._fileSystem.GetFiles(currentPath);
        var currentContentFiles = currentDirectory.Children.OfType<ContentFile>().ToList();

        foreach (var file in files.Select(Path.GetFileName).Where(fileName => currentContentFiles.All(x => x.Name != fileName))) {
            var tempResult = this.CreateContentFile(currentDirectory, file, out var createdMetadata);
            result = tempResult || result;
            if (createdMetadata != null && tempResult) {
                foundMetadata.Add(createdMetadata);
            }
        }

        var currentContentDirectories = currentDirectory.Children.OfType<IContentDirectory>();
        foreach (var child in currentContentDirectories) {
            result = this.ResolveNewContentFiles(child, out var childMetadata) || result;
            foundMetadata.AddRange(childMetadata);
        }

        metadata = foundMetadata;
        return result;
    }
}