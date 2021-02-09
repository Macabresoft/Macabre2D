namespace Macabresoft.Macabre2D.Editor.Library.Models.Content {
    /// <summary>
    /// The root of the content directory tree.
    /// </summary>
    public class RootContentDirectory : ContentDirectory {
        private const string ContentNodeName = "Content";
        private readonly string _pathToContentDirectory;

        /// <summary>
        /// Initializes a new instance of the <see cref="RootContentDirectory" /> class.
        /// </summary>
        public RootContentDirectory(string pathToContentDirectory) : base(ContentNodeName, null) {
            this._pathToContentDirectory = pathToContentDirectory;
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