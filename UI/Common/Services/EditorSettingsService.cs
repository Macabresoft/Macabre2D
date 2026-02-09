namespace Macabre2D.UI.Common;

using System.IO;
using Macabresoft.AvaloniaEx;
using Macabre2D.Common;
using Macabre2D.Framework;

/// <summary>
/// Interface for a service which handles editor settings and saved values
/// </summary>
public interface IEditorSettingsService {
    /// <summary>
    /// Gets the settings.
    /// </summary>
    EditorSettings Settings { get; }

    /// <summary>
    /// Initializes the settings.
    /// </summary>
    void Initialize();

    /// <summary>
    /// Sets the previously opened content.
    /// </summary>
    /// <param name="metadata"></param>
    void SetPreviouslyOpenedContent(ContentMetadata metadata);

    /// <summary>
    /// Saves the settings.
    /// </summary>
    void Save();
}

/// <summary>
/// A service which handles editor settings and saved values.
/// </summary>
public sealed class EditorSettingsService : IEditorSettingsService {
    private readonly IFileSystemService _fileSystem;
    private readonly IPathService _pathService;
    private readonly ISerializer _serializer;

    /// <summary>
    /// Initializes a new instance of the <see cref="EditorSettingsService" /> class.
    /// </summary>
    /// <param name="fileSystem">The file system service.</param>
    /// <param name="pathService">The path service.</param>
    /// <param name="serializer">The serializer.</param>
    public EditorSettingsService(
        IFileSystemService fileSystem,
        IPathService pathService,
        ISerializer serializer) {
        this._fileSystem = fileSystem;
        this._pathService = pathService;
        this._serializer = serializer;
    }

    /// <inheritdoc />
    public EditorSettings Settings { get; private set; } = new();

    /// <inheritdoc />
    public void Initialize() {
        var filePath = this.GetSettingsFilePath();
        if (this._fileSystem.DoesFileExist(filePath)) {
            this.Settings = this._serializer.Deserialize<EditorSettings>(filePath) ?? new EditorSettings();
        }
    }

    /// <inheritdoc />
    public void SetPreviouslyOpenedContent(ContentMetadata metadata) {
        if (metadata != null) {
            this.Settings.LastContentOpenedId = metadata.ContentId;
            this.Settings.WasSceneOpenedLast = metadata.Asset is SceneAsset;
        }
    }

    /// <inheritdoc />
    public void Save() {
        if (this.Settings != null) {
            var filePath = this.GetSettingsFilePath();
            this._serializer.Serialize(this.Settings, filePath);
        }
    }

    private string GetSettingsFilePath() {
        return Path.Combine(this._pathService.ProjectDirectoryPath, EditorSettings.FileName);
    }
}