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
                this.LoadAssets();
                // reset asset manager based on the content files found.
            }
        }

        /// <inheritdoc />
        public void MoveContent(ContentNode contentToMove, ContentDirectory newParent) {
            throw new NotImplementedException();
        }

        private IList<ContentMetadata> GetMetadata() {
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

        private void LoadAssets() {
            var metadata = this.GetMetadata();

            foreach (var singleMetadata in metadata) {
                this.ResolveContentFile(singleMetadata);
                
 
            }

            var newMetadata = new List<ContentMetadata>();
            var contentFiles = this.ResolveContentFiles(this._rootContentDirectory, metadata, newMetadata);
        }

        private void ResolveContentFile(ContentMetadata metadata) {
            var splitPath = metadata.SplitContentPath;

            if (this._rootContentDirectory.TryFindNode(splitPath.Take(splitPath.Count - 1).ToArray(), out var parent) && parent is IContentDirectory parentDirectory) {
                // TODO: add file
            }            
        }

        private IList<ContentFile> ResolveContentFiles(
            IContentDirectory directory,
            IList<ContentMetadata> unresolvedMetadata,
            IList<ContentMetadata> newMetadata) {
            var contentFiles = new List<ContentFile>();
            var path = directory.GetFullPath();
            if (Directory.Exists(path)) {
                var subdirectories = Directory.GetDirectories(path);
                foreach (var subdirectory in subdirectories) {
                    var subContentDirectory = new ContentDirectory(Path.GetDirectoryName(subdirectory), directory);
                    contentFiles.AddRange(this.ResolveContentFiles(subContentDirectory, unresolvedMetadata, newMetadata));
                }

                var files = this._fileSystemSystemService.GetFiles(path);
                foreach (var file in files.Where(x => !x.EndsWith(ContentFile.FileExtension))) {
                    if (this.TryGetContentFile(file, directory, out var contentFile) && contentFile != null) {
                    }
                }


                // TODO: find all directories under this and recursively resolve their content files
                // TODO: then find all files and create ContentFiles and fill them with metadata.
            }

            return contentFiles;
        }

        private bool TryGetContentFile(string filePath, IContentDirectory parent, out ContentFile contentFile) {
            contentFile = null;

            return contentFile != null;
        }
    }
}