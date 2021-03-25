namespace Macabresoft.Macabre2D.Editor.Library.Services {
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
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
        /// Builds the content.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>The exit code of the MGCB process.</returns>
        int Build(BuildContentArguments args);

        /// <summary>
        /// Creates a project at the specified path.
        /// </summary>
        /// <param name="projectDirectoryPath">Path to the directory where the project should be created.</param>
        /// <returns>The created project.</returns>
        IGameProject CreateProject(string projectDirectoryPath);

        /// <summary>
        /// Gets the project directory path.
        /// </summary>
        string GetProjectDirectoryPath();

        /// <summary>
        /// Loads the project at the specified path.
        /// </summary>
        /// <param name="projectFilePath">The project file path.</param>
        /// <returns>The loaded project.</returns>
        IGameProject LoadProject(string projectFilePath);

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
        public const string ContentDirectory = "content";
        private const string CompiledContentDirectory = ".compiled";
        private const string MgcbFileName = "editor.mgcb";
        private const string SourceDirectory = "src";

        public static readonly string[] ReservedDirectories = {
            ContentMetadata.MetadataDirectoryName,
            ContentMetadata.ArchiveDirectoryName,
            ContentDirectory,
            SourceDirectory,
            CompiledContentDirectory
        };

        private static readonly IDictionary<string, Type> FileExtensionToAssetType = new Dictionary<string, Type>();

        private readonly IFileSystemService _fileSystem;
        private readonly ILoggingService _loggingService;
        private readonly IProcessService _processService;
        private readonly ISceneService _sceneService;
        private readonly ISerializer _serializer;

        private readonly IUndoService _undoService;
        private IGameProject _currentProject;
        private IGameScene _currentScene;
        private bool _hasChanges;
        private string _projectFilePath;
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
        /// <param name="fileSystem">The file system service.</param>
        /// <param name="loggingService">The logging service.</param>
        /// <param name="processService">The process service.</param>
        /// <param name="sceneService">The scene service.</param>
        /// <param name="serializer">The serializer.</param>
        /// <param name="undoService"></param>
        public ProjectService(
            IFileSystemService fileSystem,
            ILoggingService loggingService,
            IProcessService processService,
            ISceneService sceneService,
            ISerializer serializer,
            IUndoService undoService) : base() {
            this._fileSystem = fileSystem;
            this._loggingService = loggingService;
            this._processService = processService;
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
        public int Build(BuildContentArguments args) {
            var exitCode = -1;
            if (!string.IsNullOrWhiteSpace(args.ContentFilePath) && this._fileSystem.DoesFileExist(args.ContentFilePath)) {
                var startInfo = new ProcessStartInfo {
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    FileName = "mgcb",
                    WindowStyle = ProcessWindowStyle.Hidden,
                    Arguments = args.ToConsoleArguments(),
                    WorkingDirectory = Path.GetDirectoryName(args.ContentFilePath) ?? string.Empty
                };

                exitCode = this._processService.StartProcess(startInfo);
            }

            return exitCode;
        }

        /// <inheritdoc />
        public IGameProject CreateProject(string projectDirectoryPath) {
            var projectFilePath = Path.Combine(projectDirectoryPath, GameProject.ProjectFileExtension);

            if (this._fileSystem.DoesFileExist(projectFilePath)) {
                throw new NotSupportedException();
            }

            foreach (var directory in ReservedDirectories) {
                this._fileSystem.CreateDirectory(Path.Combine(projectDirectoryPath, directory));
            }

            var project = new GameProject();
            var sceneAsset = this._sceneService.CreateNewScene(projectDirectoryPath, Path.Combine(ContentDirectory, "Default Scene"));
            project.StartupSceneContentId = sceneAsset.ContentId;
            this.SaveProjectFile(project, projectFilePath);
            return this.LoadProject(projectFilePath);
        }

        /// <inheritdoc />
        public string GetProjectDirectoryPath() {
            return !string.IsNullOrEmpty(this._projectFilePath) ? Path.GetDirectoryName(this._projectFilePath) : string.Empty;
        }

        /// <inheritdoc />
        public IGameProject LoadProject(string projectFilePath) {
            this._projectFilePath = this._fileSystem.DoesFileExist(projectFilePath) ? projectFilePath : throw new NotSupportedException();
            this.CurrentProject = this._serializer.Deserialize<GameProject>(projectFilePath);
            this.LoadContent();
            // TODO: compile content
            return this.CurrentProject;
        }


        /// <inheritdoc />
        public void MoveContent(IContentNode contentToMove, IContentDirectory newParent) {
            var originalParent = contentToMove.Parent;
            this._undoService.Do(() => { contentToMove.ChangeParent(newParent); }, () => { contentToMove.ChangeParent(originalParent); });
        }

        /// <inheritdoc />
        public void SaveProject() {
            this.SaveProjectFile(this.CurrentProject, this._projectFilePath);
        }

        private void BuildContentForProject() {
            var mgcbContents = new StringBuilder();
            var mgcbFilePath = Path.Combine(this.GetProjectDirectoryPath(), MgcbFileName);
            var buildArgs = new BuildContentArguments(mgcbFilePath, "DesktopGL", true);

            mgcbContents.AppendLine("#----------------------------- Global Properties ----------------------------#");
            mgcbContents.AppendLine();

            foreach (var argument in buildArgs.GetConsoleArguments()) {
                mgcbContents.AppendLine(argument);
            }

            mgcbContents.AppendLine();
            mgcbContents.AppendLine(@"#-------------------------------- References --------------------------------#");
            mgcbContents.AppendLine();

            // TODO: add references
            /*foreach (var referencePath in referencePaths) {
                mgcbContents.AppendLine($@"/reference:{referencePath}");
            }*/

            mgcbContents.AppendLine();
            mgcbContents.AppendLine(@"#---------------------------------- Content ---------------------------------#");
            mgcbContents.AppendLine();

            var contentFiles = this.RootContentDirectory.GetAllContentFiles();
            foreach (var contentFile in contentFiles) {
                mgcbContents.AppendLine(contentFile.Metadata.GetContentBuildCommands());
                mgcbContents.AppendLine();
            }

            this._fileSystem.WriteAllText(mgcbFilePath, mgcbContents.ToString());
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
            var projectDirectoryPath = this.GetProjectDirectoryPath();
            var metadataDirectory = Path.Combine(projectDirectoryPath, ContentMetadata.MetadataDirectoryName);
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
                        var archiveDirectory = Path.Combine(projectDirectoryPath, ContentMetadata.ArchiveDirectoryName);
                        this._fileSystem.CreateDirectory(archiveDirectory);
                        this._fileSystem.MoveFile(file, Path.Combine(archiveDirectory, fileName));
                    }
                }
            }

            return metadata;
        }

        private void LoadContent() {
            var projectDirectoryPath = this.GetProjectDirectoryPath();
            if (!string.IsNullOrWhiteSpace(projectDirectoryPath)) {
                this._fileSystem.CreateDirectory(projectDirectoryPath);

                if (this._rootContentDirectory != null) {
                    this._rootContentDirectory.PathChanged -= this.ContentNode_PathChanged;
                }

                this._rootContentDirectory = new RootContentDirectory(this._fileSystem, projectDirectoryPath);
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
                var projectDirectoryPath = this.GetProjectDirectoryPath();
                var fileName = $"{metadata.ContentId}{ContentMetadata.FileExtension}";
                var current = Path.Combine(projectDirectoryPath, ContentMetadata.MetadataDirectoryName, fileName);
                var moveTo = Path.Combine(projectDirectoryPath, ContentMetadata.ArchiveDirectoryName, fileName);
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
            var fullDirectoryPath = Path.Combine(this.GetProjectDirectoryPath(), ContentMetadata.MetadataDirectoryName);

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