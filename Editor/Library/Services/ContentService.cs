namespace Macabresoft.Macabre2D.Editor.Library.Services {
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
        /// Builds the content.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>The exit code of the MGCB process.</returns>
        int Build(BuildContentArguments args);
        
        /// <summary>
        /// Gets the root content directory.
        /// </summary>
        IContentDirectory RootContentDirectory { get; }

        /// <summary>
        /// Moves the content to a new folder.
        /// </summary>
        /// <param name="contentToMove">The content to move.</param>
        /// <param name="newParent">The new parent.</param>
        void MoveContent(ContentNode contentToMove, ContentDirectory newParent);

        /// <summary>
        /// Initializes the service with a directory and the asset manager so it can construct a tree of content.
        /// </summary>
        /// <param name="pathToContentDirectory">The path to the content directory.</param>
        /// <param name="assetManager">The asset manager.</param>
        void Initialize(string pathToContentDirectory, IAssetManager assetManager);
    }

    /// <summary>
    /// A service that handles MonoGame content for the editor.
    /// </summary>
    public sealed class ContentService : IContentService {
        private RootContentDirectory _rootContentDirectory;
        private IAssetManager _assetManager;

        /// <inheritdoc />
        public IContentDirectory RootContentDirectory => this._rootContentDirectory;

        /// <inheritdoc />
        public int Build(BuildContentArguments args) {
            var exitCode = -1;
            if (!string.IsNullOrWhiteSpace(args.ContentFilePath) && File.Exists(args.ContentFilePath)) {
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
        public void MoveContent(ContentNode contentToMove, ContentDirectory newParent) {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public void Initialize(string pathToContentDirectory, IAssetManager assetManager) {
            this._assetManager = assetManager;

            if (!string.IsNullOrWhiteSpace(pathToContentDirectory) && Directory.Exists(pathToContentDirectory)) {
                this._rootContentDirectory = new RootContentDirectory(pathToContentDirectory);
                var contentFiles = this.ResolveContentFiles(this._rootContentDirectory);
                // reset asset manager based on the content files found.
            }
        }

        private IList<ContentFile> ResolveContentFiles(IContentDirectory directory) {
            var contentFiles = new List<ContentFile>();
            var path = directory.GetFullPath();
            if (Directory.Exists(path)) {
                var subdirectories = Directory.GetDirectories(path);
                foreach (var subdirectory in subdirectories) {
                    var subContentDirectory = new ContentDirectory(Path.GetDirectoryName(subdirectory), directory);
                    contentFiles.AddRange(this.ResolveContentFiles(subContentDirectory));
                }

                var files = Directory.GetFiles(path);
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