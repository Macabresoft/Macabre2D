namespace Macabresoft.Macabre2D.Editor.Library.Models.Content {
    using System.IO;
    using System.Linq;
    using Macabresoft.Macabre2D.Editor.Library.Services;

    /// <summary>
    /// The root of the content directory tree.
    /// </summary>
    public sealed class RootContentDirectory : ContentDirectory {
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
            return ProjectService.ContentDirectory;
        }

        /// <inheritdoc />
        public override string GetFullPath() {
            return Path.Combine(this._pathToProjectDirectory, this.GetContentPath());
        }

        /// <inheritdoc />
        public override void LoadChildDirectories(IFileSystemService fileSystemService) {
            var currentDirectoryPath = this.GetFullPath();

            if (fileSystemService.DoesDirectoryExist(currentDirectoryPath)) {
                var directories = fileSystemService.GetDirectories(currentDirectoryPath);

                foreach (var directory in directories) {
                    this.LoadDirectory(fileSystemService, directory);
                }
            }
        }
    }
}