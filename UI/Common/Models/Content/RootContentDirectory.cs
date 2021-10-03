namespace Macabresoft.Macabre2D.UI.Common {
    using System.Linq;
    using Macabresoft.Macabre2D.Framework;

    /// <summary>
    /// The root of the content directory tree.
    /// </summary>
    public sealed class RootContentDirectory : ContentDirectory {
        private static readonly string[] UnusedContentDirectories = {
            ContentMetadata.ArchiveDirectoryName,
            ContentMetadata.MetadataDirectoryName,
            PathService.BinDirectoryName,
            PathService.ObjDirectoryName
        };

        private readonly IPathService _pathService;

        /// <summary>
        /// Initializes a new instance of the <see cref="RootContentDirectory" /> class.
        /// </summary>
        /// <param name="fileSystemService">The file service.</param>
        /// <param name="pathService">The path service.</param>
        public RootContentDirectory(IFileSystemService fileSystemService, IPathService pathService) : base(PathService.ContentDirectoryName, null) {
            this._pathService = pathService;
            this.LoadChildDirectories(fileSystemService);
        }

        /// <inheritdoc />
        public override string GetContentPath() {
            return string.Empty;
        }

        /// <inheritdoc />
        public override string GetFullPath() {
            return this._pathService.ContentDirectoryPath;
        }

        /// <inheritdoc />
        public override void LoadChildDirectories(IFileSystemService fileSystemService) {
            var currentDirectoryPath = this.GetFullPath();

            if (fileSystemService.DoesDirectoryExist(currentDirectoryPath)) {
                var directories = fileSystemService.GetDirectories(currentDirectoryPath);

                foreach (var directory in directories.Where(x => !UnusedContentDirectories.Any(x.EndsWith))) {
                    this.LoadDirectory(fileSystemService, directory);
                }
            }
        }
    }
}