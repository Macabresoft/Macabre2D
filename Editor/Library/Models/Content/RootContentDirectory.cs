namespace Macabresoft.Macabre2D.Editor.Library.Models.Content {
    using System.IO;
    using System.Linq;
    using Macabresoft.Macabre2D.Editor.Library.Services;
    using Macabresoft.Macabre2D.Framework;

    /// <summary>
    /// The root of the content directory tree.
    /// </summary>
    public sealed class RootContentDirectory : ContentDirectory {
        private static readonly string[] ReservedContentDirectories = {
            ContentMetadata.ArchiveDirectoryName,
            ContentMetadata.MetadataDirectoryName
        };
        
        private readonly string _pathToProjectDirectory;

        /// <summary>
        /// Initializes a new instance of the <see cref="RootContentDirectory" /> class.
        /// </summary>
        /// <param name="fileSystemService">The file service.</param>
        /// <param name="pathToProjectDirectory">The path to the project directory.</param>
        public RootContentDirectory(IFileSystemService fileSystemService, string pathToProjectDirectory) : base(ProjectService.ContentDirectory, null) {
            this._pathToProjectDirectory = pathToProjectDirectory;
            this.LoadChildDirectories(fileSystemService);
        }

        /// <inheritdoc />
        public override string GetContentPath() {
            return string.Empty;
        }

        /// <inheritdoc />
        public override string GetFullPath() {
            return Path.Combine(this._pathToProjectDirectory, ProjectService.ContentDirectory);
        }

        /// <inheritdoc />
        public override void LoadChildDirectories(IFileSystemService fileSystemService) {
            var currentDirectoryPath = this.GetFullPath();

            if (fileSystemService.DoesDirectoryExist(currentDirectoryPath)) {
                var directories = fileSystemService.GetDirectories(currentDirectoryPath).Where(x => !ReservedContentDirectories.Contains(Path.GetDirectoryName(x)));

                foreach (var directory in directories) {
                    this.LoadDirectory(fileSystemService, directory);
                }
            }
        }
    }
}