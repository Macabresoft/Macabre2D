namespace Macabresoft.Macabre2D.Editor.Library.Services {
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Text;
    using Macabresoft.Macabre2D.Editor.Library.Models;
    using Macabresoft.Macabre2D.Editor.Library.Models.Content;
    using Macabresoft.Macabre2D.Framework;
    using ReactiveUI;

    /// <summary>
    /// Interface for a service which loads, saves, and exposes a <see cref="GameProject" />.
    /// </summary>
    public interface IProjectService : INotifyPropertyChanged {
        /// <summary>
        /// Gets the currently loaded project.
        /// </summary>
        IGameProject CurrentProject { get; }

        /// <summary>
        /// Gets the root content directory.
        /// </summary>
        IContentDirectory RootContentDirectory { get; }

        /// <summary>
        /// Gets or sets the current scene.
        /// </summary>
        /// <value>The current scene.</value>
        public IGameScene CurrentScene { get; set; }

        /// <summary>
        /// Gets or sets a value which indicates whether or not the project has changes which require saving.
        /// </summary>
        bool HasChanges { get; set; }

        /// <summary>
        /// Creates a project at the specified path.
        /// </summary>
        /// <param name="projectDirectoryPath">Path to the directory where the project exists.</param>
        /// <returns>The created project.</returns>
        IGameProject CreateProject(string projectDirectoryPath);

        /// <summary>
        /// Loads the project at the specified path.
        /// </summary>
        /// <param name="projectDirectoryPath">Path to the directory where the project exists.</param>
        /// <returns>The loaded project.</returns>
        IGameProject LoadProject(string projectDirectoryPath);

        /// <summary>
        /// Moves the content to a new folder.
        /// </summary>
        /// <param name="contentToMove">The content to move.</param>
        /// <param name="newParent">The new parent.</param>
        void MoveContent(IContentNode contentToMove, IContentDirectory newParent);

        /// <summary>
        /// Saves the currently opened project.
        /// </summary>
        void SaveProject();
    }

    /// <summary>
    /// A service which loads, saves, and exposes a <see cref="GameProject" />.
    /// </summary>
    public sealed class ProjectService : ReactiveObject, IProjectService {
        public const string ContentDirectory = "Content";
        private const string ContentBinDirectory = ".contentbin";
        private const string MGCBFileName = "Content.Editor.mgcb";

        private static readonly string[] RequiredReferences = {
            "Newtonsoft.Json.dll",
            "Macabre2D.Framework.dll"
        };

        public static readonly string[] ReservedDirectories = {
            Path.Combine(ContentDirectory, ContentMetadata.MetadataDirectoryName),
            Path.Combine(ContentDirectory, ContentMetadata.ArchiveDirectoryName),
            ContentDirectory,
            ContentBinDirectory
        };

        private static readonly IDictionary<string, Type> FileExtensionToAssetType = new Dictionary<string, Type>();


        private readonly IBuildService _buildService;
        private readonly IFileSystemService _fileSystem;
        private readonly ILoggingService _loggingService;
        private readonly ISceneService _sceneService;
        private readonly ISerializer _serializer;

        private readonly IUndoService _undoService;
        private IGameProject _currentProject;
        private IGameScene _currentScene;
        private bool _hasChanges;
        private string _projectDirectoryPath = string.Empty;
        private RootContentDirectory _rootContentDirectory;

        /// <summary>
        /// Static constructor for <see cref="ProjectService" />.
        /// </summary>
        static ProjectService() {
            FileExtensionToAssetType.Add(SceneAsset.FileExtension, typeof(SceneAsset));

            foreach (var extension in SpriteSheet.ValidFileExtensions) {
                FileExtensionToAssetType.Add(extension, typeof(SpriteSheet));
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectService" /> class.
        /// </summary>
        /// <param name="buildService">The build service.</param>
        /// <param name="fileSystem">The file system service.</param>
        /// <param name="loggingService">The logging service.</param>
        /// <param name="sceneService">The scene service.</param>
        /// <param name="serializer">The serializer.</param>
        /// <param name="undoService">The undo service.</param>
        public ProjectService(
            IBuildService buildService,
            IFileSystemService fileSystem,
            ILoggingService loggingService,
            ISceneService sceneService,
            ISerializer serializer,
            IUndoService undoService) {
            this._buildService = buildService;
            this._fileSystem = fileSystem;
            this._loggingService = loggingService;
            this._sceneService = sceneService;
            this._serializer = serializer;
            this._undoService = undoService;
        }

        /// <inheritdoc />
        public IContentDirectory RootContentDirectory => this._rootContentDirectory;

        /// <inheritdoc />
        public IGameProject CurrentProject {
            get => this._currentProject;
            private set => this.RaiseAndSetIfChanged(ref this._currentProject, value);
        }

        /// <inheritdoc />
        public IGameScene CurrentScene {
            get => this._currentScene;
            set => this.RaiseAndSetIfChanged(ref this._currentScene, value);
        }

        /// <inheritdoc />
        public bool HasChanges {
            get => this._hasChanges;
            set => this.RaiseAndSetIfChanged(ref this._hasChanges, value);
        }

        /// <inheritdoc />
        public IGameProject CreateProject(string projectDirectoryPath) {
            var projectFilePath = Path.Combine(projectDirectoryPath, ContentDirectory, GameProject.ProjectFileName);

            if (!this._fileSystem.DoesFileExist(projectFilePath)) {
                foreach (var directory in ReservedDirectories) {
                    this._fileSystem.CreateDirectory(Path.Combine(projectDirectoryPath, directory));
                }

                var project = new GameProject();
                var sceneAsset = this._sceneService.CreateNewScene(projectDirectoryPath, Path.Combine(ContentDirectory, "Default Scene"));
                project.StartupSceneContentId = sceneAsset.ContentId;
                this.SaveProjectFile(project, projectFilePath);
            }

            return this.LoadProject(projectDirectoryPath);
        }

        /// <inheritdoc />
        public IGameProject LoadProject(string projectDirectoryPath) {
            this._projectDirectoryPath = this._fileSystem.DoesDirectoryExist(projectDirectoryPath) ? projectDirectoryPath : throw new NotSupportedException();
            var projectFilePath = this.GetProjectFilePath();
            this.CurrentProject = this._serializer.Deserialize<GameProject>(projectFilePath);
            this.LoadContent();
            return this.CurrentProject;
        }


        /// <inheritdoc />
        public void MoveContent(IContentNode contentToMove, IContentDirectory newParent) {
            var originalParent = contentToMove.Parent;
            this._undoService.Do(() => { contentToMove.ChangeParent(newParent); }, () => { contentToMove.ChangeParent(originalParent); });
        }

        /// <inheritdoc />
        public void SaveProject() {
            this.SaveProjectFile(this.CurrentProject, this.GetProjectFilePath());
        }

        private void BuildContentForProject() {
            var mgcbStringBuilder = new StringBuilder();
            var mgcbFilePath = Path.Combine(this._projectDirectoryPath, ContentDirectory, MGCBFileName);
            var buildArgs = new BuildContentArguments(
                mgcbFilePath,
                this._projectDirectoryPath,
                "DesktopGL",
                true);

            mgcbStringBuilder.AppendLine("#----------------------------- Global Properties ----------------------------#");
            mgcbStringBuilder.AppendLine();

            foreach (var argument in buildArgs.GetMGCBFileArguments()) {
                mgcbStringBuilder.AppendLine(argument);
            }

            mgcbStringBuilder.AppendLine();
            mgcbStringBuilder.AppendLine(@"#-------------------------------- References --------------------------------#");
            mgcbStringBuilder.AppendLine();

            foreach (var reference in RequiredReferences) {
                mgcbStringBuilder.AppendLine($@"/reference:{reference}");
            }

            mgcbStringBuilder.AppendLine();
            mgcbStringBuilder.AppendLine(@"#---------------------------------- Content ---------------------------------#");
            mgcbStringBuilder.AppendLine();

            var contentFiles = this.RootContentDirectory.GetAllContentFiles();
            foreach (var contentFile in contentFiles) {
                mgcbStringBuilder.AppendLine(contentFile.Metadata.GetContentBuildCommands());
                mgcbStringBuilder.AppendLine();
            }

            var mgcbText = mgcbStringBuilder.ToString();
            this._fileSystem.WriteAllText(mgcbFilePath, mgcbText);

            // TODO: handle different build configurations
            var desktopGLFilePath = Path.Combine(this._projectDirectoryPath, "Content.DesktopGL.mgcb");
            this._fileSystem.WriteAllText(desktopGLFilePath, mgcbText);

            this._buildService.BuildContent(buildArgs);
        }

        private void ContentNode_PathChanged(object sender, ValueChangedEventArgs<string> e) {
            switch (sender) {
                case IContentDirectory:
                    this._fileSystem.MoveDirectory(e.OriginalValue, e.UpdatedValue);
                    break;
                case ContentFile:
                    this._fileSystem.MoveFile(e.OriginalValue, e.UpdatedValue);
                    break;
            }
        }

        private void CreateContentFile(IContentDirectory parent, string fileName) {
            var extension = Path.GetExtension(fileName);

            if (FileExtensionToAssetType.TryGetValue(extension, out var assetType)) {
                var parentPath = parent.GetContentPath();
                var splitPath = parentPath.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries).ToList();
                splitPath.Add(Path.GetFileNameWithoutExtension(fileName));
                var asset = Activator.CreateInstance(assetType) as IAsset;
                var metadata = new ContentMetadata(asset, splitPath, extension);
                this.SaveMetadata(metadata);
                var contentFile = new ContentFile(parent, metadata);
                this.CurrentProject.Assets.RegisterMetadata(contentFile.Metadata);
            }
        }

        private IEnumerable<ContentMetadata> GetMetadata() {
            var metadata = new List<ContentMetadata>();
            var metadataDirectory = Path.Combine(this._projectDirectoryPath, ContentDirectory, ContentMetadata.MetadataDirectoryName);
            if (this._fileSystem.DoesDirectoryExist(metadataDirectory)) {
                var files = this._fileSystem.GetFiles(metadataDirectory, ContentMetadata.MetadataSearchPattern);
                foreach (var file in files) {
                    try {
                        var contentMetadata = this._serializer.Deserialize<ContentMetadata>(file);
                        metadata.Add(contentMetadata);
                    }
                    catch (Exception e) {
                        var fileName = Path.GetFileName(file);
                        var message = $"Archiving metadata '{fileName}' due to an exception";
                        this._loggingService.LogException(message, e);
                        var archiveDirectory = Path.Combine(this._projectDirectoryPath, ContentDirectory, ContentMetadata.ArchiveDirectoryName);
                        this._fileSystem.CreateDirectory(archiveDirectory);
                        this._fileSystem.MoveFile(file, Path.Combine(archiveDirectory, fileName));
                    }
                }
            }

            return metadata;
        }

        private string GetProjectFilePath() {
            return Path.Combine(this._projectDirectoryPath, ContentDirectory, GameProject.ProjectFileName);
        }

        private void LoadContent() {
            if (!string.IsNullOrWhiteSpace(this._projectDirectoryPath)) {
                this._fileSystem.CreateDirectory(this._projectDirectoryPath);

                if (this._rootContentDirectory != null) {
                    this._rootContentDirectory.PathChanged -= this.ContentNode_PathChanged;
                }

                this._rootContentDirectory = new RootContentDirectory(this._fileSystem, this._projectDirectoryPath);
                this._rootContentDirectory.PathChanged += this.ContentNode_PathChanged;

                foreach (var metadata in this.GetMetadata()) {
                    this.ResolveContentFile(metadata);
                }

                this.ResolveNewContentFiles(this._rootContentDirectory);
                this.BuildContentForProject();
            }
        }

        private void ResolveContentFile(ContentMetadata metadata) {
            ContentFile contentNode = null;
            var splitPath = metadata.SplitContentPath;
            if (splitPath.Any()) {
                IContentDirectory parentDirectory;
                if (splitPath.Count == this._rootContentDirectory.GetDepth() + 1) {
                    parentDirectory = this._rootContentDirectory;
                }
                else {
                    parentDirectory = this._rootContentDirectory.FindNode(splitPath.Take(splitPath.Count - 1).ToArray()) as IContentDirectory;
                }

                if (parentDirectory != null) {
                    var contentFilePath = Path.Combine(parentDirectory.GetFullPath(), metadata.GetFileName());
                    if (this._fileSystem.DoesFileExist(contentFilePath)) {
                        contentNode = new ContentFile(parentDirectory, metadata);
                    }
                }
            }

            if (contentNode == null) {
                var projectDirectoryPath = this._projectDirectoryPath;
                var fileName = $"{metadata.ContentId}{ContentMetadata.FileExtension}";
                var current = Path.Combine(projectDirectoryPath, ContentDirectory, ContentMetadata.MetadataDirectoryName, fileName);
                var moveTo = Path.Combine(projectDirectoryPath, ContentDirectory, ContentMetadata.ArchiveDirectoryName, fileName);
                this._fileSystem.MoveFile(current, moveTo);
            }
            else {
                this.CurrentProject.Assets.RegisterMetadata(metadata);
            }
        }

        private void ResolveNewContentFiles(IContentDirectory currentDirectory) {
            var currentPath = currentDirectory.GetFullPath();
            var files = this._fileSystem.GetFiles(currentPath);
            var currentContentFiles = currentDirectory.Children.OfType<ContentFile>().ToList();

            foreach (var file in files) {
                var fileName = Path.GetFileName(file);
                if (currentContentFiles.All(x => x.Name != Path.GetFileName(file))) {
                    this.CreateContentFile(currentDirectory, fileName);
                }
            }

            var currentContentDirectories = currentDirectory.Children.OfType<IContentDirectory>();
            foreach (var child in currentContentDirectories) {
                this.ResolveNewContentFiles(child);
            }
        }

        private void SaveMetadata(ContentMetadata metadata) {
            var fullDirectoryPath = Path.Combine(this._projectDirectoryPath, ContentDirectory, ContentMetadata.MetadataDirectoryName);

            if (this._fileSystem.DoesDirectoryExist(fullDirectoryPath)) {
                this._serializer.Serialize(metadata, Path.Combine(fullDirectoryPath, $"{metadata.ContentId}{ContentMetadata.FileExtension}"));
            }
        }

        private void SaveProjectFile(IGameProject project, string projectFilePath) {
            if (project != null && !string.IsNullOrWhiteSpace(projectFilePath)) {
                this._serializer.Serialize(project, projectFilePath);
            }
        }
    }
}