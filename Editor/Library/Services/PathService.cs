namespace Macabresoft.Macabre2D.Editor.Library.Services {
    using System.IO;
    using System.Reflection;
    using Macabresoft.Macabre2D.Framework;

    /// <summary>
    /// Interface for a service which provides typical paths for the editor to access the project and its content.
    /// </summary>
    public interface IPathService {
        /// <summary>
        /// Gets the content directory path.
        /// </summary>
        string ContentDirectoryPath { get; }

        /// <summary>
        /// Gets the editor's bin directory path.
        /// </summary>
        string EditorBinDirectoryPath { get; }

        /// <summary>
        /// Gets the metadata archive directory path.
        /// </summary>
        string MetadataArchiveDirectoryPath { get; }

        /// <summary>
        /// Gets the metadata directory path.
        /// </summary>
        string MetadataDirectoryPath { get; }

        /// <summary>
        /// Gets the project directory path.
        /// </summary>
        string ProjectDirectoryPath { get; }

        /// <summary>
        /// Gets the project file path.
        /// </summary>
        string ProjectFilePath { get; }
    }

    /// <summary>
    /// A service which provides typical paths for the editor to access the project and its content.
    /// </summary>
    public class PathService : IPathService {
        /// <summary>
        /// The name of the content directory.
        /// </summary>
        public const string ContentDirectoryName = "Content";

        /// <summary>
        /// Initializes a new instance of the <see cref="PathService" /> class.
        /// </summary>
        public PathService() : this(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PathService" /> class.
        /// </summary>
        /// <param name="editorBinDirectoryPath">Path to the editor binaries, used for building MGCB.</param>
        /// <param name="projectDirectoryPath">Path to the project directory.</param>
        public PathService(string editorBinDirectoryPath, string projectDirectoryPath) {
            this.EditorBinDirectoryPath = editorBinDirectoryPath;
            this.ProjectDirectoryPath = projectDirectoryPath;
            this.ContentDirectoryPath = Path.Combine(this.ProjectDirectoryPath, ContentDirectoryName);
            this.MetadataArchiveDirectoryPath = Path.Combine(this.ContentDirectoryPath, ContentMetadata.ArchiveDirectoryName);
            this.MetadataDirectoryPath = Path.Combine(this.ContentDirectoryPath, ContentMetadata.MetadataDirectoryName);
            this.ProjectFilePath = Path.Combine(this.ContentDirectoryPath, GameProject.ProjectFileName);
        }

        private PathService(string editorBinDirectoryPath) : this(
            editorBinDirectoryPath,
            new DirectoryInfo(Path.Combine(editorBinDirectoryPath, "..", "..", "..", "..", "..", "Project")).FullName) {
        }

        /// <inheritdoc />
        public string ContentDirectoryPath { get; }

        /// <inheritdoc />
        public string EditorBinDirectoryPath { get; }

        /// <inheritdoc />
        public string MetadataArchiveDirectoryPath { get; }

        /// <inheritdoc />
        public string MetadataDirectoryPath { get; }

        /// <inheritdoc />
        public string ProjectDirectoryPath { get; }

        /// <inheritdoc />
        public string ProjectFilePath { get; }
    }
}