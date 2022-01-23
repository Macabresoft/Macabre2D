namespace Macabresoft.Macabre2D.UI.Common;

using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using Macabresoft.AvaloniaEx;
using Macabresoft.Macabre2D.Framework;
using Macabresoft.Macabre2D.Gameplay;
using ReactiveUI;

/// <summary>
/// Interface for a service which loads, saves, and exposes a <see cref="GameProject" />.
/// </summary>
public interface IProjectService : INotifyPropertyChanged {
    /// <summary>
    /// Gets the currently loaded project.
    /// </summary>
    GameProject CurrentProject { get; }

    /// <summary>
    /// Tries to load the project.
    /// </summary>
    /// <returns>
    /// The loaded project.
    /// </returns>
    GameProject LoadProject();

    /// <summary>
    /// Saves the currently opened project.
    /// </summary>
    void SaveProject();
}

/// <summary>
/// A service which loads, saves, and exposes a <see cref="GameProject" />.
/// </summary>
public sealed class ProjectService : ReactiveObject, IProjectService {
    private const string DefaultSceneName = "Default";
    private readonly IContentService _contentService;
    private readonly IFileSystemService _fileSystem;
    private readonly IPathService _pathService;
    private readonly ISceneService _sceneService;
    private readonly ISerializer _serializer;
    private readonly IEditorSettingsService _settingsService;
    private GameProject _currentProject;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectService" /> class.
    /// </summary>
    /// <param name="contentService">The content service.</param>
    /// <param name="fileSystem">The file system service.</param>
    /// <param name="pathService">The path service.</param>
    /// <param name="sceneService">The scene service.</param>
    /// <param name="serializer">The serializer.</param>
    /// <param name="settingsService">The editor settings service.</param>
    public ProjectService(
        IContentService contentService,
        IFileSystemService fileSystem,
        IPathService pathService,
        ISceneService sceneService,
        ISerializer serializer,
        IEditorSettingsService settingsService) {
        this._contentService = contentService;
        this._fileSystem = fileSystem;
        this._pathService = pathService;
        this._sceneService = sceneService;
        this._serializer = serializer;
        this._settingsService = settingsService;
    }

    /// <inheritdoc />
    public GameProject CurrentProject {
        get => this._currentProject;
        private set => this.RaiseAndSetIfChanged(ref this._currentProject, value);
    }

    /// <inheritdoc />
    public GameProject LoadProject() {
        this._fileSystem.CreateDirectory(this._pathService.ContentDirectoryPath);
        this._fileSystem.CreateDirectory(this._pathService.MetadataArchiveDirectoryPath);
        this._fileSystem.CreateDirectory(this._pathService.MetadataDirectoryPath);
        
        var projectExists = this._fileSystem.DoesFileExist(this._pathService.ProjectFilePath);
        if (!projectExists) {
            this._contentService.RefreshContent(true);

            this.CurrentProject = new GameProject {
                StartupSceneContentId = this.CreateInitialScene()
            };

            this.SaveProjectFile(this.CurrentProject, this._pathService.ProjectFilePath);
        }
        else {
            this._contentService.RefreshContent(this._settingsService.Settings.ShouldRebuildContent);
            this.CurrentProject = this._serializer.Deserialize<GameProject>(this._pathService.ProjectFilePath);
            var sceneId = this._settingsService.Settings.LastSceneOpened != Guid.Empty ? this._settingsService.Settings.LastSceneOpened : this.CurrentProject.StartupSceneContentId;
            if (!this._sceneService.TryLoadScene(sceneId, out _)) {
                var scenes = this._contentService.RootContentDirectory.GetAllContentFiles().Where(x => x.Asset is SceneAsset).ToList();
                if (!scenes.Any()) {
                    this.CurrentProject.StartupSceneContentId = this.CreateInitialScene();
                    this.SaveProjectFile(this.CurrentProject, this._pathService.ProjectFilePath);
                }
            }
        }

        return this.CurrentProject;
    }

    /// <inheritdoc />
    public void SaveProject() {
        this.SaveProjectFile(this.CurrentProject, this._pathService.ProjectFilePath);
    }

    private Guid CreateInitialScene() {
        var parent = this._contentService.RootContentDirectory;
        var contentDirectoryPath = parent.GetFullPath();
        if (!this._fileSystem.DoesDirectoryExist(contentDirectoryPath)) {
            throw new DirectoryNotFoundException();
        }

        var filePath = Path.Combine(contentDirectoryPath, $"{DefaultSceneName}{SceneAsset.FileExtension}");
        if (this._fileSystem.DoesFileExist(filePath)) {
            throw new NotSupportedException("Whoa you already have a scene, this is a bug, so sorry!");
        }

        var scene = new Scene {
            BackgroundColor = DefinedColors.MacabresoftPurple,
            Name = DefaultSceneName
        };

        scene.AddSystem<UpdateSystem>();
        scene.AddSystem<RenderSystem>();
        scene.AddChild<Camera>();
        scene.AddChild<MyFirstEntity>();

        var sceneAsset = new SceneAsset();

        sceneAsset.LoadContent(scene);
        var contentPath = Path.Combine(parent.GetContentPath(), DefaultSceneName);
        var metadata = new ContentMetadata(
            sceneAsset,
            contentPath.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries).ToList(),
            SceneAsset.FileExtension);

        this._sceneService.SaveScene(metadata, scene);
        var content = new ContentFile(parent, metadata);
        return this._sceneService.TryLoadScene(content.Id, out var asset) ? asset.ContentId : Guid.Empty;
    }

    private void SaveProjectFile(IGameProject project, string projectFilePath) {
        if (project != null && !string.IsNullOrWhiteSpace(projectFilePath)) {
            this._serializer.Serialize(project, projectFilePath);
        }
    }
}