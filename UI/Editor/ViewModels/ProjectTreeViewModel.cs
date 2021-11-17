namespace Macabresoft.Macabre2D.UI.Editor {
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using Avalonia.Threading;
    using Macabresoft.Macabre2D.Framework;
    using Macabresoft.Macabre2D.UI.Common;
    using ReactiveUI;
    using Unity;

    /// <summary>
    /// A view model for the content tree.
    /// </summary>
    public class ProjectTreeViewModel : BaseViewModel {
        private readonly ICommonDialogService _dialogService;
        private readonly IEditorService _editorService;
        private readonly IFileSystemService _fileSystem;
        private readonly ISaveService _saveService;
        private readonly ISceneService _sceneService;
        private readonly IUndoService _undoService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectTreeViewModel" /> class.
        /// </summary>
        /// <remarks>This constructor only exists for design time XAML.</remarks>
        public ProjectTreeViewModel() {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectTreeViewModel" /> class.
        /// </summary>
        /// <param name="contentService">The content service.</param>
        /// <param name="dialogService">The dialog service.</param>
        /// <param name="editorService">The editor service.</param>
        /// <param name="fileSystem">The file system.</param>
        /// <param name="projectService">The project service.</param>
        /// <param name="saveService">The save service.</param>
        /// <param name="sceneService">The scene service.</param>
        /// <param name="undoService">The undo service.</param>
        [InjectionConstructor]
        public ProjectTreeViewModel(
            IContentService contentService,
            ICommonDialogService dialogService,
            IEditorService editorService,
            IFileSystemService fileSystem,
            IProjectService projectService,
            ISaveService saveService,
            ISceneService sceneService,
            IUndoService undoService) {
            this.ContentService = contentService;
            this._dialogService = dialogService;
            this._editorService = editorService;
            this._fileSystem = fileSystem;
            this.ProjectService = projectService;
            this._saveService = saveService;
            this._sceneService = sceneService;
            this._undoService = undoService;

            this.AddDirectoryCommand = ReactiveCommand.Create<IContentDirectory>(this.ContentService.AddDirectory);
            this.AddSceneCommand = ReactiveCommand.Create<IContentDirectory>(this.ContentService.AddScene);
            this.ImportCommand = ReactiveCommand.CreateFromTask<IContentDirectory>(this.ContentService.ImportContent);

            this.OpenCommand = ReactiveCommand.CreateFromTask<IContentNode>(
                this.OpenSelectedContent,
                this.ContentService.WhenAny(x => x.Selected, y => CanOpenContent(y.Value)));

            this.OpenContentLocationCommand = ReactiveCommand.Create<IContentNode>(
                this.OpenContentLocation,
                this.ContentService.WhenAny(x => x.Selected, y => y.Value != null));

            this.RemoveContentCommand = ReactiveCommand.Create<IContentNode>(
                this.RemoveContent,
                this.ContentService.WhenAny(x => x.Selected, y => y.Value is { } and not RootContentDirectory));

            this.RenameContentCommand = ReactiveCommand.CreateFromTask<string>(async x => await this.RenameContent(x));
        }

        /// <summary>
        /// Gets the add directory command.
        /// </summary>
        public ICommand AddDirectoryCommand { get; }

        /// <summary>
        /// Gets the add scene command.
        /// </summary>
        public ICommand AddSceneCommand { get; }

        /// <summary>
        /// Gets the content service.
        /// </summary>
        public IContentService ContentService { get; }

        /// <summary>
        /// Gets the import command.
        /// </summary>
        public ICommand ImportCommand { get; }

        /// <summary>
        /// Gets the open command.
        /// </summary>
        public ICommand OpenCommand { get; }

        /// <summary>
        /// Gets a command to open the file explorer to the content's location.
        /// </summary>
        public ICommand OpenContentLocationCommand { get; }

        /// <summary>
        /// Gets the project service.
        /// </summary>
        public IProjectService ProjectService { get; }

        /// <summary>
        /// Gets the remove content command.
        /// </summary>
        public ICommand RemoveContentCommand { get; }

        /// <summary>
        /// Gets a command for renaming content.
        /// </summary>
        public ICommand RenameContentCommand { get; }

        /// <summary>
        /// Moves the source content node to be a child of the target directory.
        /// </summary>
        /// <param name="sourceNode">The source node.</param>
        /// <param name="targetDirectory">The target directory.</param>
        public async Task MoveNode(IContentNode sourceNode, IContentDirectory targetDirectory) {
            if (targetDirectory != null &&
                sourceNode != null &&
                targetDirectory != sourceNode &&
                sourceNode.Parent != targetDirectory) {
                if (targetDirectory.Children.Any(x => string.Equals(x.Name, sourceNode.Name, StringComparison.OrdinalIgnoreCase))) {
                    await this._dialogService.ShowWarningDialog(
                        "Error",
                        $"Directory '{targetDirectory.Name}' already contains a folder named '{sourceNode.Name}'.");
                }
                else {
                    Dispatcher.UIThread.Post(() => this.ContentService.MoveContent(sourceNode, targetDirectory));
                }
            }
        }

        private static bool CanOpenContent(IContentNode node) {
            return node is ContentFile { Asset: SceneAsset };
        }

        private void OpenContentLocation(IContentNode node) {
            var directory = node as IContentDirectory ?? node?.Parent;
            if (directory != null) {
                this._fileSystem.OpenDirectoryInFileExplorer(directory.GetFullPath());
            }
        }

        private async Task OpenSelectedContent(IContentNode node) {
            if (CanOpenContent(node) && await this._saveService.RequestSave() != YesNoCancelResult.Cancel && this._sceneService.TryLoadScene(node.Id, out _)) {
                this._editorService.SelectedTab = EditorTabs.Scene;
            }
        }

        private void RemoveContent(IContentNode node) {
            var openSceneMetadataId = this._sceneService.CurrentSceneMetadata?.ContentId ?? Guid.Empty;
            switch (node) {
                case RootContentDirectory:
                    this._dialogService.ShowWarningDialog("Cannot Delete", "Cannot delete the root.");
                    break;
                case IContentDirectory directory when directory.ContainsMetadata(openSceneMetadataId):
                    this._dialogService.ShowWarningDialog("Cannot Delete", "This directory cannot be deleted, because the open scene is a descendent.");
                    break;
                case IContentDirectory:
                    this._fileSystem.DeleteDirectory(node.GetFullPath());
                    node.Parent?.RemoveChild(node);
                    break;
                case ContentFile { Metadata: { } } file when file.Metadata.ContentId == openSceneMetadataId:
                    this._dialogService.ShowWarningDialog("Cannot Delete", "The currently opened scene cannot be deleted.");
                    break;
                default:
                    this._fileSystem.DeleteFile(node.GetFullPath());
                    node.Parent?.RemoveChild(node);
                    break;
            }
        }

        private async Task RenameContent(string updatedName) {
            switch (this.ProjectService.Selected) {
                case RootContentDirectory:
                    return;
                case IContentNode node when node.Name != updatedName:
                    var typeName = node is IContentDirectory ? "Directory" : "File";
                    if (this._fileSystem.IsValidFileOrDirectoryName(updatedName)) {
                        await this._dialogService.ShowWarningDialog($"Invalid {typeName} Name", $"'{updatedName}' contains invalid characters.");
                    }
                    else {
                        if (node.Parent is IContentDirectory parent) {
                            var parentPath = parent.GetFullPath();
                            var updatedPath = Path.Combine(parentPath, updatedName);

                            if (File.Exists(updatedPath) || Directory.Exists(updatedPath)) {
                                await this._dialogService.ShowWarningDialog($"Invalid {typeName} Name", $"A {typeName.ToLower()} named '{updatedName}' already exists.");
                            }
                            else {
                                var originalNodeName = node.Name;
                                this._undoService.Do(() => {
                                    node.Name = updatedName;
                                }, () => {
                                    node.Name = originalNodeName;
                                });
                                node.Name = updatedName;
                            }
                        }
                    }

                    break;
                case INameable nameable when nameable.Name != updatedName:
                    var originalName = nameable.Name;
                    this._undoService.Do(() => {
                        nameable.Name = updatedName;
                    }, () => {
                        nameable.Name = originalName;
                    });
                    break;
            }
        }
    }
}