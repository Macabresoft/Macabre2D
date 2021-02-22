namespace Macabresoft.Macabre2D.Editor.Library.Models.Content {
    using System.IO;
    using Macabresoft.Macabre2D.Editor.Library.Services;

    /// <summary>
    /// The root of the content directory tree.
    /// </summary>
    public class RootContentDirectory : ContentDirectory {
        private const string ContentNodeName = "Content";
        private readonly string _pathToContentDirectory;

        /// <summary>
        /// Initializes a new instance of the <see cref="RootContentDirectory" /> class.
        /// </summary>
        /// <param name="fileSystemService">The file service.</param>
        /// <param name="pathToContentDirectory">The path to the content directory.</param>
        public RootContentDirectory(IFileSystemService fileSystemService, string pathToContentDirectory) : base(ContentNodeName, null) {
            this._pathToContentDirectory = pathToContentDirectory;
            this.LoadChildDirectories(fileSystemService);
        }

        /// <inheritdoc />
        public override string GetContentPath() {
            return string.Empty;
        }

        /// <inheritdoc />
        public override string GetFullPath() {
            return this._pathToContentDirectory;
        }
    }
}