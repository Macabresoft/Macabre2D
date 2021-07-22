namespace Macabresoft.Macabre2D.UI.Common.ViewModels {
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Reactive;
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
        private readonly IContentService _contentService;
        private readonly IDialogService _dialogService;
        private readonly IFileSystemService _fileSystem;
        private readonly ISceneService _sceneService;
        private readonly ObservableCollection<IContentDirectory> _treeRoot = new();
        private IContentNode _selectedNode;

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
            this._contentService = contentService;
            this._dialogService = dialogService;
            this._fileSystem = fileSystem;
            this._sceneService = sceneService;
            this.ResetRoot();
            this._contentService.PropertyChanged += this.ProjectService_PropertyChanged;

            this.AddFolderCommand = ReactiveCommand.Create<IContentNode, Unit>(
                this.AddFolder,
                this.WhenAny(x => x.SelectedNode, y => y.Value is IContentDirectory));

            this.OpenContentLocationCommand = ReactiveCommand.Create<IContentNode, Unit>(
                this.OpenContentLocation,
                this.WhenAny(x => x.SelectedNode, y => y.Value != null));

            this.RemoveContentCommand = ReactiveCommand.Create<IContentNode, Unit>(
                this.RemoveContent,
                this.WhenAny(x => x.SelectedNode, y => y.Value is { } and not RootContentDirectory));

            this.RenameContentCommand = ReactiveCommand.CreateFromTask<string>(
                async x => await this.RenameContent(x),
                this.WhenAny(x => x.SelectedNode, y => y.Value != null));
        }

        /// <summary>
        /// Gets the add folder command.
        /// </summary>
        public ICommand AddFolderCommand { get; }

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
        /// Gets the root of the asset tree.
        /// </summary>
        public IReadOnlyCollection<IContentDirectory> Root => this._treeRoot;

        /// <summary>
        /// Gets or sets the
        /// </summary>
        public IContentNode SelectedNode {
            get => this._selectedNode;
            set => this.RaiseAndSetIfChanged(ref this._selectedNode, value);
        }

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
                    Dispatcher.UIThread.Post(() => this._contentService.MoveContent(sourceNode, targetDirectory));
                }
            }
        }

        private Unit AddFolder(IContentNode parent) {
            if (parent is IContentDirectory parentDirectory) {
                this.CreateDirectory("New Folder", parentDirectory);
            }

            return Unit.Default;
        }

        private IContentDirectory CreateDirectory(string name, IContentDirectory parent) {
            var parentPath = parent.GetFullPath();
            var fullPath = Path.Combine(parentPath, name);
            var currentCount = 0;

            while (this._fileSystem.DoesDirectoryExist(fullPath)) {
                currentCount++;
                if (currentCount >= 100) {
                    throw new NotSupportedException("What the hell are you even doing with 100 folders named New Folder????");
                }

                name = $"New Folder ({currentCount})";
                fullPath = Path.Combine(parentPath, name);
            }

            this._fileSystem.CreateDirectory(fullPath);
            return new ContentDirectory(name, parent);
        }

        private Unit OpenContentLocation(IContentNode node) {
            var directory = node as IContentDirectory ?? node?.Parent;
            if (directory != null) {
                this._fileSystem.OpenDirectoryInFileExplorer(directory.GetFullPath());
            }

            return Unit.Default;
        }

        private void ProjectService_PropertyChanged(object sender, PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(IContentService.RootContentDirectory)) {
                this.ResetRoot();
            }
        }

        private Unit RemoveContent(IContentNode node) {
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

            return Unit.Default;
        }

        private async Task RenameContent(string updatedName) {
            if (this.SelectedNode is IContentNode node and not RootContentDirectory && node.Name != updatedName) {
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
                            this._selectedNode = node;
                        }
                    }
                }
            }
        }

        private void ResetRoot() {
            this._treeRoot.Clear();

            if (this._contentService.RootContentDirectory != null) {
                this._treeRoot.Add(this._contentService.RootContentDirectory);
            }
        }
    }
}