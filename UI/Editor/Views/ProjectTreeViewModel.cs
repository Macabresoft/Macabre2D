namespace Macabresoft.Macabre2D.UI.Editor;

using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Threading;
using DynamicData;
using Macabresoft.AvaloniaEx;
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
    /// <param name="assetSelectionService">The asset selection service.</param>
    /// <param name="contentService">The content service.</param>
    /// <param name="dialogService">The dialog service.</param>
    /// <param name="editorService">The editor service.</param>
    /// <param name="fileSystem">The file system.</param>
    /// <param name="saveService">The save service.</param>
    /// <param name="sceneService">The scene service.</param>
    /// <param name="undoService">The undo service.</param>
    [InjectionConstructor]
    public ProjectTreeViewModel(
        IAssetSelectionService assetSelectionService,
        IContentService contentService,
        ICommonDialogService dialogService,
        IEditorService editorService,
        IFileSystemService fileSystem,
        ISaveService saveService,
        ISceneService sceneService,
        IUndoService undoService) {
        this.AssetSelectionService = assetSelectionService;
        this.ContentService = contentService;
        this._dialogService = dialogService;
        this._editorService = editorService;
        this._fileSystem = fileSystem;
        this._saveService = saveService;
        this._sceneService = sceneService;
        this._undoService = undoService;

        this.AssetSelectionService.PropertyChanged += this.AssetSelectionService_PropertyChanged;

        this.AddCommand = ReactiveCommand.Create<object>(
            this.AddNode,
            this.AssetSelectionService.WhenAny(x => x.Selected, x => CanAddNode(x.Value)));

        var whenIsContentDirectory = this.AssetSelectionService.WhenAny(x => x.Selected, x => x.Value is IContentDirectory);
        this.AddDirectoryCommand = ReactiveCommand.Create<object>(x => this.ContentService.AddDirectory(x as IContentDirectory), whenIsContentDirectory);
        this.AddSceneCommand = ReactiveCommand.Create<object>(x => this.ContentService.AddScene(x as IContentDirectory), whenIsContentDirectory);
        this.ImportCommand = ReactiveCommand.CreateFromTask<object>(x => this.ContentService.ImportContent(x as IContentDirectory), whenIsContentDirectory);

        this.OpenCommand = ReactiveCommand.CreateFromTask<IContentNode>(
            this.OpenSelectedContent,
            this.ContentService.WhenAny(x => x.Selected, y => CanOpenContent(y.Value)));

        this.OpenContentLocationCommand = ReactiveCommand.Create<IContentNode>(
            this.OpenContentLocation,
            this.ContentService.WhenAny(x => x.Selected, y => y.Value != null));

        this.RemoveContentCommand = ReactiveCommand.Create<object>(
            this.RemoveContent,
            this.AssetSelectionService.WhenAny(x => x.Selected, y => this.CanRemoveContent(y.Value)));

        this.RenameContentCommand = ReactiveCommand.CreateFromTask<string>(async x => await this.RenameContent(x));

        this.MoveDownCommand = ReactiveCommand.Create<object>(this.MoveDown, this.AssetSelectionService.WhenAny(
            x => x.Selected,
            x => CanMoveDown(x.Value)));
        this.MoveUpCommand = ReactiveCommand.Create<object>(this.MoveUp, this.AssetSelectionService.WhenAny(
            x => x.Selected,
            x => CanMoveUp(x.Value)));
        this.CloneCommand = ReactiveCommand.Create<object>(this.Clone, this.AssetSelectionService.WhenAny(
            x => x.Selected,
            x => CanClone(x.Value)));
    }

    /// <summary>
    /// Gets the add command.
    /// </summary>
    public ICommand AddCommand { get; }

    /// <summary>
    /// Gets the add directory command.
    /// </summary>
    public ICommand AddDirectoryCommand { get; }

    /// <summary>
    /// Gets the add scene command.
    /// </summary>
    public ICommand AddSceneCommand { get; }

    /// <summary>
    /// Gets the asset selection service.
    /// </summary>
    public IAssetSelectionService AssetSelectionService { get; }

    /// <summary>
    /// Gets a command to clone an entity.
    /// </summary>
    public ICommand CloneCommand { get; }

    /// <summary>
    /// Gets the content service.
    /// </summary>
    public IContentService ContentService { get; }

    /// <summary>
    /// Gets the import command.
    /// </summary>
    public ICommand ImportCommand { get; }

    /// <summary>
    /// Gets a value indicating whether or not a <see cref="SpriteSheetMember" /> is selected.
    /// </summary>
    public bool IsSpriteSheetMemberSelected => CanClone(this.AssetSelectionService.Selected);

    /// <summary>
    /// Gets a command to move a child down.
    /// </summary>
    public ICommand MoveDownCommand { get; }

    /// <summary>
    /// Gets a command to move a child up.
    /// </summary>
    public ICommand MoveUpCommand { get; }

    /// <summary>
    /// Gets the open command.
    /// </summary>
    public ICommand OpenCommand { get; }

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

    private void AddNewSpriteSheetMember(ISpriteSheetMemberCollection collection) {
        var newMember = collection.CreateNewMember();
        this._undoService.Do(
            () => { collection.AddMember(newMember); },
            () => { collection.RemoveMember(newMember); });
    }

    private void AddNode(object parent) {
        switch (parent) {
            case IContentDirectory directory:
                this.ContentService.AddDirectory(directory);
                break;
            case ISpriteSheetMemberCollection memberCollection:
                this.AddNewSpriteSheetMember(memberCollection);
                break;
            case SpriteSheetMember { SpriteSheet: { } spriteSheet } member:
                if (spriteSheet.GetMemberCollection(member.GetType()) is { } anotherMemberCollection) {
                    this.AddNewSpriteSheetMember(anotherMemberCollection);
                }

                break;
        }
    }

    private void AssetSelectionService_PropertyChanged(object sender, PropertyChangedEventArgs e) {
        if (e.PropertyName == nameof(this.AssetSelectionService.Selected)) {
            this.RaisePropertyChanged(nameof(this.IsSpriteSheetMemberSelected));
        }
    }

    private static bool CanAddNode(object parent) {
        return parent is IContentDirectory or AutoTileSetCollection or SpriteAnimationCollection or SpriteSheetFontCollection or GamePadIconSetCollection or KeyboardIconSetCollection or SpriteSheetMember;
    }

    private static bool CanClone(object selected) {
        return selected is SpriteSheetMember;
    }

    private static bool CanMoveDown(object selected) {
        var result = false;

        if (selected is SpriteSheetMember { SpriteSheet: { } spriteSheet } member && spriteSheet.GetMemberCollection(member.GetType()) is { } collection) {
            var index = collection.IndexOf(member);
            var maxIndex = collection.Count - 1;
            result = index < maxIndex;
        }

        return result;
    }

    private static bool CanMoveUp(object selected) {
        var result = false;

        if (selected is SpriteSheetMember { SpriteSheet: { } spriteSheet } member && spriteSheet.GetMemberCollection(member.GetType()) is { } collection) {
            var index = collection.IndexOf(member);
            result = index > 0;
        }

        return result;
    }

    private static bool CanOpenContent(IContentNode node) {
        return node is ContentFile { Asset: SceneAsset };
    }

    private bool CanRemoveContent(object node) {
        return node is not RootContentDirectory &&
               node is not INameableCollection &&
               (this._sceneService.CurrentSceneMetadata == null || !(node is ContentFile { Asset: SceneAsset asset } && asset.ContentId == this._sceneService.CurrentSceneMetadata.ContentId));
    }

    private void Clone(object selected) {
        if (selected is SpriteSheetMember { SpriteSheet: { } spriteSheet } member && spriteSheet.GetMemberCollection(member.GetType()) is { } collection && member.TryClone(out var clone)) {
            this._undoService.Do(() =>
            {
                collection.AddMember(clone);
                this.AssetSelectionService.Selected = clone;
            }, () =>
            {
                collection.RemoveMember(clone);
                this.AssetSelectionService.Selected = member;
            });
        }
    }

    private void MoveDown(object selected) {
        if (selected is SpriteSheetMember { SpriteSheet: { } spriteSheet } member && spriteSheet.GetMemberCollection(member.GetType()) is { } collection) {
            var index = collection.IndexOf(member);
            if (index < collection.Count - 1 && index >= 0) {
                this._undoService.Do(() =>
                {
                    collection.RemoveMember(member);
                    collection.InsertMember(index + 1, member);
                    this.AssetSelectionService.Selected = member;
                }, () =>
                {
                    collection.RemoveMember(member);
                    collection.InsertMember(index, member);
                    this.AssetSelectionService.Selected = member;
                });
            }
        }
    }

    private void MoveUp(object selected) {
        if (selected is SpriteSheetMember { SpriteSheet: { } spriteSheet } member && spriteSheet.GetMemberCollection(member.GetType()) is { } collection) {
            var index = collection.IndexOf(member);
            if (index > 0) {
                this._undoService.Do(() =>
                {
                    collection.RemoveMember(member);
                    collection.InsertMember(index - 1, member);
                    this.AssetSelectionService.Selected = member;
                }, () =>
                {
                    collection.RemoveMember(member);
                    collection.InsertMember(index, member);
                    this.AssetSelectionService.Selected = member;
                });
            }
        }
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

    private void RemoveContent(object node) {
        var openSceneMetadataId = this._sceneService.CurrentSceneMetadata?.ContentId ?? Guid.Empty;
        switch (node) {
            case RootContentDirectory:
                this._dialogService.ShowWarningDialog("Cannot Delete", "Cannot delete the root.");
                break;
            case IContentDirectory directory when directory.ContainsMetadata(openSceneMetadataId):
                this._dialogService.ShowWarningDialog("Cannot Delete", "This directory cannot be deleted, because the open scene is a descendent.");
                break;
            case IContentDirectory directory:
                this._fileSystem.DeleteDirectory(directory.GetFullPath());
                directory.Parent?.RemoveChild(directory);
                break;
            case ContentFile { Metadata: not null } file when file.Metadata.ContentId == openSceneMetadataId:
                this._dialogService.ShowWarningDialog("Cannot Delete", "The currently opened scene cannot be deleted.");
                break;
            case IContentNode contentNode:
                this._fileSystem.DeleteFile(contentNode.GetFullPath());
                contentNode.Parent?.RemoveChild(contentNode);
                break;
            case SpriteSheetMember { SpriteSheet: { } spriteSheet } member:
                if (spriteSheet.GetMemberCollection(member.GetType()) is { } collection) {
                    var index = collection.IndexOf(member);
                    this._undoService.Do(() => collection.RemoveMember(member),
                        () => collection.InsertMember(index, member));
                }

                break;
        }
    }

    private async Task RenameContent(string updatedName) {
        switch (this.AssetSelectionService.Selected) {
            case RootContentDirectory:
                return;
            case IContentNode node when node.Name != updatedName:
                var typeName = node is IContentDirectory ? "Directory" : "File";
                if (this._fileSystem.IsValidFileOrDirectoryName(updatedName)) {
                    await this._dialogService.ShowWarningDialog($"Invalid {typeName} Name", $"'{updatedName}' contains invalid characters.");
                }
                else {
                    if (node.Parent is { } parent) {
                        var parentPath = parent.GetFullPath();
                        var updatedPath = Path.Combine(parentPath, updatedName);

                        if (File.Exists(updatedPath) || Directory.Exists(updatedPath)) {
                            await this._dialogService.ShowWarningDialog($"Invalid {typeName} Name", $"A {typeName.ToLower()} named '{updatedName}' already exists.");
                        }
                        else {
                            var originalNodeName = node.Name;
                            this._undoService.Do(() => { node.Name = updatedName; }, () => { node.Name = originalNodeName; });
                            node.Name = updatedName;
                        }
                    }
                }

                break;
            case INameable nameable when nameable.Name != updatedName:
                var originalName = nameable.Name;
                this._undoService.Do(() => { nameable.Name = updatedName; }, () => { nameable.Name = originalName; });
                break;
        }
    }
}