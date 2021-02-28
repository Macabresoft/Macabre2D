namespace Macabresoft.Macabre2D.Editor.Library.Services {
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using Macabresoft.Macabre2D.Editor.Library.Models.Content;
    using Macabresoft.Macabre2D.Framework;

    /// <summary>
    /// Interface for a service that handles MonoGame content for the editor.
    /// </summary>
    public interface IContentService {
        /// <summary>
        /// Gets the root content directory.
        /// </summary>
        IContentDirectory RootContentDirectory { get; }

        /// <summary>
        /// Builds the content.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>The exit code of the MGCB process.</returns>
        int Build(BuildContentArguments args);

        /// <summary>
        /// Initializes the service with a directory and the asset manager so it can construct a tree of content.
        /// </summary>
        /// <param name="pathToContentDirectory">The path to the content directory.</param>
        /// <param name="assetManager">The asset manager.</param>
        void Initialize(string pathToContentDirectory, IAssetManager assetManager);

        /// <summary>
        /// Moves the content to a new folder.
        /// </summary>
        /// <param name="contentToMove">The content to move.</param>
        /// <param name="newParent">The new parent.</param>
        void MoveContent(ContentNode contentToMove, ContentDirectory newParent);
    }

    /// <summary>
    /// A service that handles MonoGame content for the editor.
    /// </summary>
    public sealed class ContentService : IContentService {
        private static readonly IDictionary<string, Type> FileExtensionToAssetType = new Dictionary<string, Type>();

        /// <summary>
        /// Static constructor for <see cref="ContentService"/>.
        /// </summary>
        static ContentService() {
            foreach (var extension in SpriteSheet.ValidFileExtensions) {
                FileExtensionToAssetType.Add(extension, typeof(SpriteSheet));
            }
        }

        private readonly IFileSystemService _fileSystemSystemService;
        private readonly ISerializer _serializer;

        private IAssetManager _assetManager;
        private RootContentDirectory _rootContentDirectory;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentService" /> class.
        /// </summary>
        /// <param name="fileSystemSystemService">The file service.</param>
        /// <param name="serializer">The serializer.</param>
        public ContentService(IFileSystemService fileSystemSystemService, ISerializer serializer) {
            this._fileSystemSystemService = fileSystemSystemService;
            this._serializer = serializer;
        }

        /// <inheritdoc />
        public IContentDirectory RootContentDirectory => this._rootContentDirectory;

        /// <inheritdoc />
        public int Build(BuildContentArguments args) {
            var exitCode = -1;
            if (!string.IsNullOrWhiteSpace(args.ContentFilePath) && this._fileSystemSystemService.DoesFileExist(args.ContentFilePath)) {
                var startInfo = new ProcessStartInfo {
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    FileName = "mgcb",
                    WindowStyle = ProcessWindowStyle.Hidden,
                    Arguments = args.ToConsoleArguments(),
                    WorkingDirectory = Path.GetDirectoryName(args.ContentFilePath) ?? string.Empty
                };

                using var process = Process.Start(startInfo);
                if (process != null) {
                    process.WaitForExit();
                    exitCode = process.ExitCode;
                }
            }

            return exitCode;
        }

        /// <inheritdoc />
        public void Initialize(string pathToContentDirectory, IAssetManager assetManager) {
            this._assetManager = assetManager;

            if (!string.IsNullOrWhiteSpace(pathToContentDirectory) && this._fileSystemSystemService.DoesDirectoryExist(pathToContentDirectory)) {
                this._rootContentDirectory = new RootContentDirectory(this._fileSystemSystemService, pathToContentDirectory);

                foreach (var metadata in this.GetMetadata()) {
                    this.ResolveContentFile(metadata);
                }

                this.ResolveNewContentFiles(this._rootContentDirectory);
            }
        }

        /// <inheritdoc />
        public void MoveContent(ContentNode contentToMove, ContentDirectory newParent) {
            throw new NotImplementedException();
        }

        private ContentFile CreateContentFile(IContentDirectory parent, string fileName) {
            ContentFile contentFile;
            var extension = Path.GetExtension(fileName);

            if (FileExtensionToAssetType.TryGetValue(extension, out var assetType)) {
                var parentPath = parent.GetContentPath();
                var splitPath = parentPath.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries).ToList();
                splitPath.Add(Path.GetFileNameWithoutExtension(fileName));
                var asset = Activator.CreateInstance(assetType) as IAsset;
                var metadata = new ContentMetadata(asset, splitPath, extension);
                this.SaveMetadata(metadata);
                contentFile = new ContentFile(parent, metadata);
                this._assetManager.RegisterMetadata(metadata);
            }
            else {
                contentFile = null;
            }

            return contentFile;
        }

        private IEnumerable<ContentMetadata> GetMetadata() {
            var metadata = new List<ContentMetadata>();
            var metaDataDirectory = Path.Combine(this._rootContentDirectory.GetFullPath(), ContentMetadata.MetadataDirectoryName);
            if (this._fileSystemSystemService.DoesDirectoryExist(metaDataDirectory)) {
                var files = this._fileSystemSystemService.GetFiles(metaDataDirectory, $"*{ContentMetadata.FileExtension}");
                foreach (var file in files) {
                    try {
                        var contentMetadata = this._serializer.Deserialize<ContentMetadata>(file);
                        metadata.Add(contentMetadata);
                    }
                    catch {
                        // Archive the file since it can't seem to deserialize.
                        this._fileSystemSystemService.MoveFile(file, Path.Combine(metaDataDirectory, "..", ContentMetadata.ArchiveDirectoryName, Path.GetFileName(file)));
                    }
                }
            }

            return metadata;
        }

        private void ResolveContentFile(ContentMetadata metadata) {
            ContentFile contentNode = null;
            var splitPath = metadata.SplitContentPath;
            if (splitPath.Any()) {
                IContentDirectory parentDirectory;
                if (splitPath.Count == 1) {
                    parentDirectory = this._rootContentDirectory;
                }
                else {
                    parentDirectory = this._rootContentDirectory.FindNode(splitPath.Take(splitPath.Count - 1).ToArray()) as IContentDirectory;
                }

                if (parentDirectory != null) {
                    var contentFilePath = Path.Combine(parentDirectory.GetFullPath(), metadata.GetFileName());
                    if (this._fileSystemSystemService.DoesFileExist(contentFilePath)) {
                        contentNode = new ContentFile(parentDirectory, metadata);
                    }
                }
            }

            if (contentNode == null) {
                var rootContentDirectoryPath = this._rootContentDirectory.GetFullPath();
                var fileName = $"{metadata.ContentId}{ContentMetadata.FileExtension}";
                var current = Path.Combine(rootContentDirectoryPath, ContentMetadata.MetadataDirectoryName, fileName);
                var moveTo = Path.Combine(rootContentDirectoryPath, ContentMetadata.ArchiveDirectoryName, fileName);
                this._fileSystemSystemService.MoveFile(current, moveTo);
            }
            else {
                this._assetManager.RegisterMetadata(metadata);
            }
        }

        private void ResolveNewContentFiles(IContentDirectory currentDirectory) {
            var currentPath = currentDirectory.GetFullPath();
            var files = this._fileSystemSystemService.GetFiles(currentPath);
            var currentContentFiles = currentDirectory.Children.OfType<ContentFile>().ToList();

            foreach (var file in files) {
                var fileName = Path.GetFileName(file);
                if (currentContentFiles.All(x => x.Name != Path.GetFileName(file))) {
                    this.CreateContentFile(currentDirectory, fileName);
                }
            }

            var currentContentDirectories = currentDirectory.Children.OfType<IContentDirectory>();
            foreach (var child in currentContentDirectories) {
                this.ResolveNewContentFiles(child);
            }
        }

        private void SaveMetadata(ContentMetadata metadata) {
            var fullDirectoryPath = Path.Combine(this._rootContentDirectory.GetFullPath(), ContentMetadata.MetadataDirectoryName);

            if (this._fileSystemSystemService.DoesDirectoryExist(fullDirectoryPath)) {
                this._serializer.Serialize(metadata, Path.Combine(fullDirectoryPath, $"{metadata.ContentId}{ContentMetadata.FileExtension}"));
            }
        }
    }
}