namespace Macabresoft.Macabre2D.Editor.Library.Services {

    /// <summary>
    /// Interface for a service which provides typical paths for the editor to access the project and its content.
    /// </summary>
    public interface IPathService {
        /// <summary>
        /// Gets the project directory path.
        /// </summary>
        string ProjectDirectoryPath { get; }
        
        /// <summary>
        /// Gets the content directory path.
        /// </summary>
        string ContentDirectoryPath { get; }
        
        /// <summary>
        /// Gets the metadata directory path.
        /// </summary>
        string MetadataDirectoryPath { get; }
        
        /// <summary>
        /// Gets the metadata archive directory path.
        /// </summary>
        string MetadataArchiveDirectoryPath { get; }
    }
    
    /// <summary>
    /// A service which provides typical paths for the editor to access the project and its content.
    /// </summary>
    public class PathService {

    }
}