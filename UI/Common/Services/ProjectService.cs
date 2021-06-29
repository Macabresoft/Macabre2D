namespace Macabresoft.Macabre2D.UI.Common.Services {
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Text;
    using Macabresoft.Macabre2D.UI.Common.Models;
    using Macabresoft.Macabre2D.UI.Common.Models.Content;
    using Macabresoft.Macabre2D.Framework;
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
        /// Gets the root content directory.
        /// </summary>
        IContentDirectory RootContentDirectory { get; }

        /// <summary>
        /// Gets or sets a value which indicates whether or not the project has changes which require saving.
        /// </summary>
        bool HasChanges { get; set; }

        /// <summary>
        /// Loads the project.
        /// </summary>
        /// <returns>The loaded project.</returns>
        GameProject LoadProject();

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
        private static readonly IDictionary<string, Type> FileExtensionToAssetType = new Dictionary<string, Type>();

        private readonly IAssetManager _assetManager;
        private readonly IBuildService _buildService;
        private readonly IFileSystemService _fileSystem;
        private readonly ILoggingService _loggingService;
        private readonly IPathService _pathService;
        private readonly ISceneService _sceneService;
        private readonly ISerializer _serializer;

        private readonly IUndoService _undoService;
        private GameProject _currentProject;
        private bool _hasChanges;
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
        /// <param name="assetManager">The asset manager.</param>
        /// <param name="buildService">The build service.</param>
        /// <param name="fileSystem">The file system service.</param>
        /// <param name="loggingService">The logging service.</param>
        /// <param name="pathService">The path service.</param>
        /// <param name="sceneService">The scene service.</param>
        /// <param name="serializer">The serializer.</param>
        /// <param name="undoService">The undo service.</param>
        public ProjectService(
            IAssetManager assetManager,
            IBuildService buildService,
            IFileSystemService fileSystem,
            ILoggingService loggingService,
            IPathService pathService,
            ISceneService sceneService,
            ISerializer serializer,
            IUndoService undoService) {
            this._assetManager = assetManager;
            this._buildService = buildService;
            this._fileSystem = fileSystem;
            this._loggingService = loggingService;
            this._pathService = pathService;
            this._sceneService = sceneService;
            this._serializer = serializer;
            this._undoService = undoService;
        }

        /// <inheritdoc />
        public IContentDirectory RootContentDirectory => this._rootContentDirectory;

        /// <inheritdoc />
        public GameProject CurrentProject {
            get => this._currentProject;
            private set => this.RaiseAndSetIfChanged(ref this._currentProject, value);
        }

        /// <inheritdoc />
        public bool HasChanges {
            get => this._hasChanges;
            set => this.RaiseAndSetIfChanged(ref this._hasChanges, value);
        }

        /// <inheritdoc />
        public GameProject LoadProject() {
            this.LoadContent();

            var projectExists = this._fileSystem.DoesFileExist(this._pathService.ProjectFilePath);
            if (!projectExists) {
                this._fileSystem.CreateDirectory(this._pathService.ContentDirectoryPath);
                this._fileSystem.CreateDirectory(this._pathService.MetadataArchiveDirectoryPath);
                this._fileSystem.CreateDirectory(this._pathService.MetadataDirectoryPath);

                this.CurrentProject = new GameProject {
                    StartupSceneContentId = this.CreateInitialScene()
                };

                this.SaveProjectFile(this.CurrentProject, this._pathService.ProjectFilePath);
            }
            else {
                this.CurrentProject = this._serializer.Deserialize<GameProject>(this._pathService.ProjectFilePath);
                if (!this._sceneService.TryLoadScene(this.CurrentProject.StartupSceneContentId, out var sceneAsset) && sceneAsset != null) {
                    this.CurrentProject.StartupSceneContentId = this.CreateInitialScene();
                }
            }
            
            return this.CurrentProject;
        }

        /// <inheritdoc />
        public void MoveContent(IContentNode contentToMove, IContentDirectory newParent) {
            var originalParent = contentToMove.Parent;
            this._undoService.Do(() => { contentToMove.ChangeParent(newParent); }, () => { contentToMove.ChangeParent(originalParent); });
        }

        /// <inheritdoc />
        public void SaveProject() {
            this.SaveProjectFile(this.CurrentProject, this._pathService.ProjectFilePath);
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

                if (Activator.CreateInstance(assetType) is IAsset asset) {
                    var metadata = new ContentMetadata(asset, splitPath, extension);
                    this.SaveMetadata(metadata);
                    var contentFile = new ContentFile(parent, metadata);
                    this._assetManager.RegisterMetadata(contentFile.Metadata);
                }
            }
        }

        private Guid CreateInitialScene() {
            var newSceneAsset = this._sceneService.CreateNewScene(this._rootContentDirectory, "Default Scene");
            return newSceneAsset.ContentId;
        }

        private IEnumerable<ContentMetadata> GetMetadata() {
            var metadata = new List<ContentMetadata>();
            if (this._fileSystem.DoesDirectoryExist(this._pathService.MetadataDirectoryPath)) {
                var files = this._fileSystem.GetFiles(this._pathService.MetadataDirectoryPath, ContentMetadata.MetadataSearchPattern);
                foreach (var file in files) {
                    try {
                        var contentMetadata = this._serializer.Deserialize<ContentMetadata>(file);
                        metadata.Add(contentMetadata);
                    }
                    catch (Exception e) {
                        var fileName = Path.GetFileName(file);
                        var message = $"Archiving metadata '{fileName}' due to an exception";
                        this._loggingService.LogException(message, e);
                        this._fileSystem.CreateDirectory(this._pathService.MetadataArchiveDirectoryPath);
                        this._fileSystem.MoveFile(file, Path.Combine(this._pathService.MetadataArchiveDirectoryPath, fileName));
                    }
                }
            }

            return metadata;
        }

        private void LoadContent() {
            if (!string.IsNullOrWhiteSpace(this._pathService.PlatformsDirectoryPath)) {
                this._fileSystem.CreateDirectory(this._pathService.PlatformsDirectoryPath);
                this._fileSystem.CreateDirectory(this._pathService.ContentDirectoryPath);
                this._fileSystem.CreateDirectory(this._pathService.EditorContentDirectoryPath);

                if (this._rootContentDirectory != null) {
                    this._rootContentDirectory.PathChanged -= this.ContentNode_PathChanged;
                }

                this._rootContentDirectory = new RootContentDirectory(this._fileSystem, this._pathService);
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
                var fileName = $"{metadata.ContentId}{ContentMetadata.FileExtension}";
                var current = Path.Combine(this._pathService.MetadataDirectoryPath, fileName);
                var moveTo = Path.Combine(this._pathService.MetadataArchiveDirectoryPath, fileName);
                this._fileSystem.MoveFile(current, moveTo);
            }
            else {
                this._assetManager.RegisterMetadata(metadata);
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
        
        private void BuildContentForProject() {
            var platform = "DesktopGL";
            var mgcbStringBuilder = new StringBuilder();
            var mgcbFilePath = Path.Combine(this._pathService.ContentDirectoryPath, $"Content.{platform}.mgcb");
            var buildArgs = new BuildContentArguments(
                mgcbFilePath,
                platform,
                true);

            var outputDirectoryPath = Path.GetRelativePath(this._pathService.ContentDirectoryPath, this._pathService.EditorContentDirectoryPath);

            mgcbStringBuilder.AppendLine("#----------------------------- Global Properties ----------------------------#");
            mgcbStringBuilder.AppendLine();

            foreach (var argument in buildArgs.ToGlobalProperties(outputDirectoryPath)) {
                mgcbStringBuilder.AppendLine(argument);
            }

            mgcbStringBuilder.AppendLine();
            mgcbStringBuilder.AppendLine(@"#-------------------------------- References --------------------------------#");
            mgcbStringBuilder.AppendLine();
            mgcbStringBuilder.AppendLine();
            mgcbStringBuilder.AppendLine(@"#---------------------------------- Content ---------------------------------#");
            mgcbStringBuilder.AppendLine();

            mgcbStringBuilder.AppendLine($"#begin {GameProject.ProjectFileName}");
            mgcbStringBuilder.AppendLine($@"/copy:{GameProject.ProjectFileName}");
            mgcbStringBuilder.AppendLine($"#end {GameProject.ProjectFileName}");
            mgcbStringBuilder.AppendLine();

            var contentFiles = this.RootContentDirectory.GetAllContentFiles();
            foreach (var contentFile in contentFiles) {
                mgcbStringBuilder.AppendLine(contentFile.Metadata.GetContentBuildCommands());
                mgcbStringBuilder.AppendLine();
            }

            var mgcbText = mgcbStringBuilder.ToString();
            this._fileSystem.WriteAllText(mgcbFilePath, mgcbText);
            this._buildService.BuildContent(buildArgs, outputDirectoryPath);
        }

        private void SaveMetadata(ContentMetadata metadata) {
            if (this._fileSystem.DoesDirectoryExist(this._pathService.MetadataDirectoryPath)) {
                this._serializer.Serialize(metadata, Path.Combine(this._pathService.MetadataDirectoryPath, $"{metadata.ContentId}{ContentMetadata.FileExtension}"));
            }
        }

        private void SaveProjectFile(IGameProject project, string projectFilePath) {
            if (project != null && !string.IsNullOrWhiteSpace(projectFilePath)) {
                this._serializer.Serialize(project, projectFilePath);
            }
        }
    }
}