namespace Macabresoft.Macabre2D.UI.Common.ViewModels {
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using Avalonia.Threading;
    using Macabresoft.Macabre2D.UI.Common.Models.Content;
    using Macabresoft.Macabre2D.UI.Common.Services;
    using ReactiveUI;
    using Unity;

    /// <summary>
    /// A view model for the content tree.
    /// </summary>
    public class ContentTreeViewModel : ViewModelBase {
        private readonly IDialogService _dialogService;
        private readonly IFileSystemService _fileSystem;
        private readonly ISceneService _sceneService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentTreeViewModel" /> class.
        /// </summary>
        /// <remarks>This constructor only exists for design time XAML.</remarks>
        public ContentTreeViewModel() {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentTreeViewModel" /> class.
        /// </summary>
        /// <param name="contentService">The content service.</param>
        /// <param name="dialogService">The dialog service.</param>
        /// <param name="fileSystem">The file system.</param>
        /// <param name="sceneService">The scene service.</param>
        [InjectionConstructor]
        public ContentTreeViewModel(
            IContentService contentService,
            IDialogService dialogService,
            IFileSystemService fileSystem,
            ISceneService sceneService) {
            this.ContentService = contentService;
            this._dialogService = dialogService;
            this._fileSystem = fileSystem;
            this._sceneService = sceneService;

            this.AddDirectoryCommand = ReactiveCommand.Create(
                this.ContentService.AddDirectory,
                this.ContentService.WhenAny(x => x.Selected, y => y.Value is IContentDirectory));

            this.AddSceneCommand = ReactiveCommand.Create(
                this.ContentService.AddScene,
                this.ContentService.WhenAny(x => x.Selected, y => y.Value is IContentDirectory));

            this.ImportCommand = ReactiveCommand.Create(
                this.Import,
                this.ContentService.WhenAny(x => x.Selected, y => y.Value is IContentDirectory));

            this.OpenContentLocationCommand = ReactiveCommand.Create<IContentNode>(
                this.OpenContentLocation,
                this.ContentService.WhenAny(x => x.Selected, y => y.Value != null));

            this.RemoveContentCommand = ReactiveCommand.Create<IContentNode>(
                this.RemoveContent,
                this.ContentService.WhenAny(x => x.Selected, y => y.Value is { } and not RootContentDirectory));

            this.RenameContentCommand = ReactiveCommand.CreateFromTask<string>(
                async x => await this.RenameContent(x),
                this.ContentService.WhenAny(x => x.Selected, y => y.Value != null));
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
        /// Gets a command to open the file explorer to the content's location.
        /// </summary>
        public ICommand OpenContentLocationCommand { get; }

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

        private void Import() {
        }

        private void OpenContentLocation(IContentNode node) {
            var directory = node as IContentDirectory ?? node?.Parent;
            if (directory != null) {
                this._fileSystem.OpenDirectoryInFileExplorer(directory.GetFullPath());
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
            if (this.ContentService.Selected is IContentNode node and not RootContentDirectory && node.Name != updatedName) {
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
                            node.Name = updatedName;
                            this.ContentService.Selected = node;
                        }
                    }
                }
            }
        }
    }
}