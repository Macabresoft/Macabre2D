namespace Macabresoft.Macabre2D.UI.Common.ViewModels.Content {
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.IO;
    using System.Reactive;
    using System.Threading.Tasks;
    using System.Windows.Input;
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
        [InjectionConstructor]
        public ContentTreeViewModel(IContentService contentService, IDialogService dialogService, IFileSystemService fileSystem) {
            this._contentService = contentService;
            this._dialogService = dialogService;
            this._fileSystem = fileSystem;
            this.ResetRoot();
            this._contentService.PropertyChanged += this.ProjectService_PropertyChanged;

            this.AddFolderCommand = ReactiveCommand.Create<IContentNode, Unit>(
                this.AddFolder,
                this.WhenAny(x => x.SelectedNode, y => y.Value != null));

            this.RemoveContentCommand = ReactiveCommand.Create<IContentNode, Unit>(
                this.RemoveContent,
                this.WhenAny(x => x.SelectedNode, y => y.Value != null && y.Value.Parent != y.Value));

            this.RenameContentCommand = ReactiveCommand.CreateFromTask<string>(
                async x => await this.RenameContent(x),
                this.WhenAny(x => x.SelectedNode, y => y.Value != null));
        }


        /// <summary>
        /// Gets the add folder command.
        /// </summary>
        public ICommand AddFolderCommand { get; }

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

        private Unit AddFolder(IContentNode parent) {
            var parentDirectory = parent as IContentDirectory ?? parent.Parent;

            if (parentDirectory != null) {
                // TODO: add
            }

            return Unit.Default;
        }

        private void ProjectService_PropertyChanged(object sender, PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(IContentService.RootContentDirectory)) {
                this.ResetRoot();
            }
        }

        private Unit RemoveContent(IContentNode node) {
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