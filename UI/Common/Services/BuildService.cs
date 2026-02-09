namespace Macabre2D.UI.Common;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Macabresoft.AvaloniaEx;
using Macabresoft.Core;
using Macabre2D.Common;
using Macabre2D.Framework;

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
    /// <returns>The exit code of the MGCB process.</returns>
    int BuildContentFromScratch(IContentDirectory contentDirectory, bool forceRebuild);

    /// <summary>
    /// Creates the MGCB file.
    /// </summary>
    /// <param name="contentDirectory">The content directory.</param>
    /// <param name="outputDirectoryPath">The output directory path.</param>
    /// <returns>The build arguments and the output directory path.</returns>
    BuildContentArguments CreateMGCBFile(IContentDirectory contentDirectory, out string outputDirectoryPath);
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

        foreach (var extension in SpriteSheet.ValidFileExtensions) {
            FileExtensionToAssetType.Add(extension, typeof(SpriteSheet));
        }

        foreach (var extension in AudioClip.ValidFileExtensions) {
            FileExtensionToAssetType.Add(extension, typeof(AudioClip));
        }

        FileExtensionToAssetType.Add(ShaderAsset.FileExtension, typeof(ShaderAsset));
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
    public int BuildContentFromScratch(IContentDirectory contentDirectory, bool forceRebuild) {
        foreach (var metadata in this.GetMetadata()) {
            this.ResolveContentFile(contentDirectory, metadata);
        }

        if (this.ResolveNewContentFiles(contentDirectory) || forceRebuild) {
            var buildArgs = this.CreateMGCBFile(contentDirectory, out var outputDirectoryPath);
            return this.BuildContent(buildArgs, outputDirectoryPath);
        }

        return -1;
    }

    public BuildContentArguments CreateMGCBFile(IContentDirectory contentDirectory, out string outputDirectoryPath) {
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

        var contentFiles = contentDirectory.GetAllContentFiles();
        foreach (var contentFile in contentFiles.Where(x => !x.Asset.IgnoreInBuild)) {
            mgcbStringBuilder.AppendLine(contentFile.Metadata.GetContentBuildCommands());
        }

        var mgcbText = mgcbStringBuilder.ToString();
        this._fileSystem.WriteAllText(mgcbFilePath, mgcbText);

        return buildArgs;
    }

    private bool CreateContentFile(IContentDirectory parent, string fileName) {
        var result = false;
        var extension = Path.GetExtension(fileName);

        if (FileExtensionToAssetType.TryGetValue(extension, out var assetType)) {
            var parentPath = parent.GetContentPath();
            var splitPath = parentPath.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries).ToList();
            splitPath.Add(Path.GetFileNameWithoutExtension(fileName));

            if (Activator.CreateInstance(assetType) is IAsset asset) {
                var metadata = new ContentMetadata(asset, splitPath, extension);
                this._assemblyService.CreateContentFileObject(parent, metadata);
                result = true;
            }
        }

        return result;
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
                catch {
                    // ignored
                }
            }
        }

        return metadata;
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
}