namespace Macabresoft.Macabre2D.UI.Common;

using System;
using System.IO;
using System.Reflection;
using Macabresoft.Macabre2D.Framework;
using Unity;

/// <summary>
/// Interface for a service which provides typical paths for the editor to access the project and its content.
/// </summary>
public interface IPathService {
    /// <summary>
    /// Gets the path to the content directory.
    /// </summary>
    string ContentDirectoryPath { get; }

    /// <summary>
    /// Gets the path to the editor's bin directory.
    /// </summary>
    string EditorBinDirectoryPath { get; }

    /// <summary>
    /// Gets the path to the editor's compiled content directory.
    /// </summary>
    string EditorContentDirectoryPath { get; }

    /// <summary>
    /// Gets the path to the editor's compiled content metadata directory.
    /// </summary>
    string EditorMetadataDirectoryPath { get; }

    /// <summary>
    /// Gets the path to the metadata directory.
    /// </summary>
    string MetadataDirectoryPath { get; }

    /// <summary>
    /// Gets the path to the platforms directory.
    /// </summary>
    string PlatformsDirectoryPath { get; }

    /// <summary>
    /// Gets the path to the project directory.
    /// </summary>
    string ProjectDirectoryPath { get; }

    /// <summary>
    /// Gets the path to the project file.
    /// </summary>
    string ProjectFilePath { get; }

    /// <summary>
    /// Gets the path to a metadata file within the editor's bin directory with the specified content identifier.
    /// </summary>
    /// <param name="contentId">The content identifier.</param>
    /// <returns>The path to the metadata file.</returns>
    string GetEditorMetadataFilePath(Guid contentId);

    /// <summary>
    /// Gets the path to a metadata file with the specified content identifier.
    /// </summary>
    /// <param name="contentId">The content identifier.</param>
    /// <returns>The path to the metadata file.</returns>
    string GetMetadataFilePath(Guid contentId);
}

/// <summary>
/// A service which provides typical paths for the editor to access the project and its content.
/// </summary>
public class PathService : IPathService {
    /// <summary>
    /// The name of the bin directory.
    /// </summary>
    public const string BinDirectoryName = "bin";

    /// <summary>
    /// The name of the content directory.
    /// </summary>
    public const string ContentDirectoryName = "Content";

    /// <summary>
    /// The name of DesktopGL's platform directory.
    /// </summary>
    public const string DesktopGLName = "DesktopGL";

    /// <summary>
    /// The name of the obj directory.
    /// </summary>
    public const string ObjDirectoryName = "obj";

    /// <summary>
    /// The name of the project directory.
    /// </summary>
    public const string ProjectDirectoryName = "Project";

    /// <summary>
    /// Initializes a new instance of the <see cref="PathService" /> class.
    /// </summary>
    [InjectionConstructor]
    public PathService() : this(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)) {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PathService" /> class.
    /// </summary>
    /// <param name="editorBinDirectoryPath">Path to the editor binaries, used for building MGCB.</param>
    /// <param name="platformsDirectoryPath">Path to the platforms directory.</param>
    public PathService(string editorBinDirectoryPath, string platformsDirectoryPath) : this(
        editorBinDirectoryPath,
        platformsDirectoryPath,
        new DirectoryInfo(Path.Combine(platformsDirectoryPath, "..", ContentDirectoryName)).FullName) {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PathService" /> class.
    /// </summary>
    /// <param name="editorBinDirectoryPath">Path to the editor binaries, used for building MGCB.</param>
    /// <param name="platformsDirectoryPath">Path to the platforms directory.</param>
    /// <param name="contentDirectoryPath">Path to the content directory.</param>
    public PathService(string editorBinDirectoryPath, string platformsDirectoryPath, string contentDirectoryPath) {
        this.EditorBinDirectoryPath = editorBinDirectoryPath;
        this.PlatformsDirectoryPath = platformsDirectoryPath;
        this.ContentDirectoryPath = contentDirectoryPath;
        this.EditorContentDirectoryPath = Path.Combine(this.EditorBinDirectoryPath, DesktopGLName, ContentDirectoryName);
        this.EditorMetadataDirectoryPath = Path.Combine(this.EditorContentDirectoryPath, ContentMetadata.MetadataDirectoryName);
        this.MetadataDirectoryPath = Path.Combine(this.ContentDirectoryPath, ContentMetadata.MetadataDirectoryName);
        this.ProjectFilePath = Path.Combine(this.ContentDirectoryPath, GameProject.ProjectFileName);
        this.ProjectDirectoryPath = new DirectoryInfo(Path.Combine(platformsDirectoryPath, "..")).FullName;
    }

    private PathService(string editorBinDirectoryPath) : this(
        editorBinDirectoryPath,
        new DirectoryInfo(Path.Combine(editorBinDirectoryPath, "..", "..", ProjectDirectoryName, "Platforms")).FullName) {
    }

    /// <inheritdoc />
    public string ContentDirectoryPath { get; }

    /// <inheritdoc />
    public string EditorBinDirectoryPath { get; }

    /// <inheritdoc />
    public string EditorContentDirectoryPath { get; }

    /// <inheritdoc />
    public string EditorMetadataDirectoryPath { get; }

    /// <inheritdoc />
    public string MetadataDirectoryPath { get; }

    /// <inheritdoc />
    public string PlatformsDirectoryPath { get; }

    /// <inheritdoc />
    public string ProjectDirectoryPath { get; }

    /// <inheritdoc />
    public string ProjectFilePath { get; }

    /// <inheritdoc />
    public string GetEditorMetadataFilePath(Guid contentId) => Path.Combine(this.EditorContentDirectoryPath, ContentMetadata.GetMetadataPath(contentId));

    /// <inheritdoc />
    public string GetMetadataFilePath(Guid contentId) => Path.Combine(this.ContentDirectoryPath, ContentMetadata.GetMetadataPath(contentId));
}