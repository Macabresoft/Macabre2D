namespace Macabresoft.Macabre2D.UI.Editor;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Threading;
using DynamicData;
using Macabresoft.AvaloniaEx;
using Macabresoft.Macabre2D.Common;
using Macabresoft.Macabre2D.Framework;
using Macabresoft.Macabre2D.UI.Common;
using ReactiveUI;
using Unity;

/// <summary>
/// A view model for the content tree.
/// </summary>
public class ProjectTreeViewModel : FilterableViewModel<IContentNode> {
    private readonly IAssemblyService _assemblyService;
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
    public ProjectTreeViewModel() : base() {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectTreeViewModel" /> class.
    /// </summary>
    /// <param name="assemblyService">The assembly service.</param>
    /// <param name="assetSelectionService">The asset selection service.</param>
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
        IAssemblyService assemblyService,
        IAssetSelectionService assetSelectionService,
        IContentService contentService,
        ICommonDialogService dialogService,
        IEditorService editorService,
        IFileSystemService fileSystem,
        IProjectService projectService,
        ISaveService saveService,
        ISceneService sceneService,
        IUndoService undoService) : base() {
        this._assemblyService = assemblyService;
        this.AssetSelectionService = assetSelectionService;
        this.ContentService = contentService;
        this._dialogService = dialogService;
        this._editorService = editorService;
        this._fileSystem = fileSystem;
        this.ProjectService = projectService;
        this._saveService = saveService;
        this._sceneService = sceneService;
        this._undoService = undoService;

        this.AssetSelectionService.PropertyChanged += this.AssetSelectionService_PropertyChanged;

        this.AddCommand = ReactiveCommand.CreateFromTask<object>(
            this.AddNode,
            this.AssetSelectionService.WhenAny(x => x.Selected, x => !this.IsFiltered && CanAddNode(x.Value)));

        var whenIsContentDirectory = this.AssetSelectionService.WhenAny(x => x.Selected, x => x.Value is IContentDirectory && !this.IsFiltered);
        this.AddDirectoryCommand = ReactiveCommand.Create<object>(this.AddDirectory, whenIsContentDirectory);
        this.AddSceneCommand = ReactiveCommand.Create<object>(this.AddScene, whenIsContentDirectory);

        this.FindContentUsagesCommand = ReactiveCommand.CreateFromTask<IContentNode>(
            this.FindContentUsages,
            this.ContentService.WhenAny(x => x.Selected, y => !this.IsFiltered && y.Value != null));

        this.ForceSaveCommand = ReactiveCommand.Create<IContentNode>(
            this.ForceSave,
            this.ContentService.WhenAny(x => x.Selected, y => y.Value != null));

        this.ImportCommand = ReactiveCommand.CreateFromTask<object>(x => this.ContentService.ImportContent(x as IContentDirectory), whenIsContentDirectory);

        this.OpenCommand = ReactiveCommand.CreateFromTask<IContentNode>(
            this.OpenSelectedContent,
            this.ContentService.WhenAny(x => x.Selected, y => !this.IsFiltered && CanOpenContent(y.Value)));

        this.OpenContentLocationCommand = ReactiveCommand.Create<IContentNode>(
            this.OpenContentLocation,
            this.ContentService.WhenAny(x => x.Selected, y => !this.IsFiltered && y.Value != null));

        this.RemoveContentCommand = ReactiveCommand.Create<object>(
            this.RemoveContent,
            this.AssetSelectionService.WhenAny(x => x.Selected, y => !this.IsFiltered && this.CanRemoveContent(y.Value)));

        this.RenameContentCommand = ReactiveCommand.CreateFromTask<string>(async x => await this.RenameContent(x));

        this.MoveDownCommand = ReactiveCommand.Create<object>(this.MoveDown, this.WhenAny(x => x.IsMoveDownEnabled, x => x.Value));

        this.MoveUpCommand = ReactiveCommand.Create<object>(this.MoveUp, this.WhenAny(x => x.IsMoveUpEnabled, x => x.Value));

        this.CloneCommand = ReactiveCommand.Create<object>(this.Clone, this.AssetSelectionService.WhenAny(
            x => x.Selected,
            x => !this.IsFiltered && CanClone(x.Value)));
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
    /// Gets a value indicating whether a <see cref="SpriteSheetMember" /> is selected.
    /// </summary>
    public bool CanMoveOrClone => CanClone(this.AssetSelectionService.Selected);

    /// <summary>
    /// Gets a command to clone an entity.
    /// </summary>
    public ICommand CloneCommand { get; }

    /// <summary>
    /// Gets the content service.
    /// </summary>
    public IContentService ContentService { get; }

    /// <summary>
    /// Gets a command that finds usages of the selected content in the current scene.
    /// </summary>
    public ICommand FindContentUsagesCommand { get; }

    /// <summary>
    /// Forces the current asset to be saved.
    /// </summary>
    public ICommand ForceSaveCommand { get; }

    /// <summary>
    /// Gets the import command.
    /// </summary>
    public ICommand ImportCommand { get; }

    /// <summary>
    /// Gets a value indicating whether move up is enabled.
    /// </summary>
    public bool IsMoveDownEnabled => !this.IsFiltered && this.AssetSelectionService.Selected is { } selected && this.CanMoveDown(selected);

    /// <summary>
    /// Gets a value indicating whether move up is enabled.
    /// </summary>
    public bool IsMoveUpEnabled => !this.IsFiltered && this.AssetSelectionService.Selected is { } selected && this.CanMoveUp(selected);

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
                var originalParent = sourceNode.Parent;
                this._undoService.Do(() => { Dispatcher.UIThread.Post(() => this.ContentService.MoveContent(sourceNode, targetDirectory)); }, () => { Dispatcher.UIThread.Post(() => this.ContentService.MoveContent(sourceNode, originalParent)); });
            }
        }
    }

    /// <inheritdoc />
    protected override IContentNode GetActualSelected() => this.AssetSelectionService.Selected as IContentNode;

    /// <inheritdoc />
    protected override IEnumerable<IContentNode> GetNodesAvailableToFilter() {
        this.AssetSelectionService.Selected = null;
        return this.ContentService.RootContentDirectory != null ? this.ContentService.RootContentDirectory.GetAllContentFiles() : Enumerable.Empty<IContentNode>();
    }

    protected override void OnFilterChanged() {
        base.OnFilterChanged();
        this.RaisePropertyChanged(nameof(this.IsMoveDownEnabled));
        this.RaisePropertyChanged(nameof(this.IsMoveUpEnabled));
    }

    /// <inheritdoc />
    protected override void SetActualSelected(IContentNode selected) {
        this.AssetSelectionService.Selected = selected;
    }

    private void AddDirectory(object parent) {
        if (parent is IContentDirectory directory && this.ContentService.AddDirectory(directory) is { } newDirectory) {
            this.SetActualSelected(newDirectory);
        }
    }

    private void AddNewProjectShader() {
        var newShader = new ScreenShader();
        var originalSelected = this.GetActualSelected();
        this._undoService.Do(
            () =>
            {
                this.ProjectService.CurrentProject.ScreenShaders.Add(newShader);
                this.AssetSelectionService.Selected = newShader;
            },
            () =>
            {
                this.ProjectService.CurrentProject.ScreenShaders.Remove(newShader);
                this.AssetSelectionService.Selected = originalSelected;
            });
    }

    private async Task AddNewSpriteSheetMember(ISpriteSheetMemberCollection collection) {
        if (!collection.TryCreateNewMember(out var newMember)) {
            var types = this._assemblyService.LoadTypes(collection.MemberType).ToList();
            if (types.Any()) {
                var selectedType = await this._dialogService.OpenTypeSelectionDialog(types, types.First(), "Make a Selection");
                if (Activator.CreateInstance(selectedType) is SpriteSheetMember member) {
                    newMember = member;
                }
            }
        }

        if (newMember != null) {
            var originalSelected = this.GetActualSelected();
            this._undoService.Do(
                () =>
                {
                    collection.AddMember(newMember);
                    this.AssetSelectionService.Selected = newMember;
                },
                () =>
                {
                    collection.RemoveMember(newMember);
                    this.AssetSelectionService.Selected = originalSelected;
                });
        }
    }

    private async Task AddNode(object parent) {
        switch (parent) {
            case IContentDirectory directory:
                this.AddDirectory(directory);
                break;
            case ISpriteSheetMemberCollection memberCollection:
                await this.AddNewSpriteSheetMember(memberCollection);
                break;
            case SpriteSheetMember { SpriteSheet: { } spriteSheet } member:
                if (spriteSheet.GetMemberCollection(member.GetType()) is { } anotherMemberCollection) {
                    await this.AddNewSpriteSheetMember(anotherMemberCollection);
                }

                break;
            case ScreenShader:
            case ScreenShaderCollection:
                this.AddNewProjectShader();
                break;
        }
    }

    private void AddScene(object parent) {
        if (parent is IContentDirectory directory && this.ContentService.AddScene(directory) is { } scene) {
            this.SetActualSelected(scene);
        }
    }

    private void AssetSelectionService_PropertyChanged(object sender, PropertyChangedEventArgs e) {
        if (e.PropertyName == nameof(this.AssetSelectionService.Selected)) {
            this.RaisePropertyChanged(nameof(this.CanMoveOrClone));
            this.RaisePropertyChanged(nameof(this.IsMoveDownEnabled));
            this.RaisePropertyChanged(nameof(this.IsMoveUpEnabled));
        }
    }

    private static bool CanAddNode(object parent) => parent is IContentDirectory or
        AutoTileSetCollection or
        SpriteAnimationCollection or
        SpriteSheetFontCollection or
        SpriteSheetIconSetCollection or
        SpriteSheetMember or
        ScreenShader or
        ScreenShaderCollection;

    private static bool CanClone(object selected) => selected is SpriteSheetMember or ScreenShader or ContentFile;

    private bool CanMoveDown(object selected) {
        var result = false;

        if (selected is SpriteSheetMember { SpriteSheet: { } spriteSheet } member && spriteSheet.GetMemberCollection(member.GetType()) is { } collection) {
            var index = collection.IndexOf(member);
            var maxIndex = collection.Count - 1;
            result = index < maxIndex;
        }
        else if (selected is ScreenShader shader) {
            var index = this.ProjectService.CurrentProject.ScreenShaders.IndexOf(shader);
            var maxIndex = this.ProjectService.CurrentProject.ScreenShaders.Count;
            result = index < maxIndex;
        }

        return result;
    }

    private bool CanMoveUp(object selected) {
        var result = false;

        if (selected is SpriteSheetMember { SpriteSheet: { } spriteSheet } member && spriteSheet.GetMemberCollection(member.GetType()) is { } collection) {
            var index = collection.IndexOf(member);
            result = index > 0;
        }
        else if (selected is ScreenShader shader) {
            var index = this.ProjectService.CurrentProject.ScreenShaders.IndexOf(shader);
            result = index > 0;
        }

        return result;
    }

    private static bool CanOpenContent(IContentNode node) => node is ContentFile { Asset: SceneAsset };

    private bool CanRemoveContent(object node) =>
        node != null &&
        node is not RootContentDirectory &&
        node is not INameableCollection &&
        (this._sceneService.CurrentSceneMetadata == null || !(node is ContentFile { Asset: SceneAsset asset } && asset.ContentId == this._sceneService.CurrentSceneMetadata.ContentId));

    private void Clone(object selected) {
        if (selected is SpriteSheetMember { SpriteSheet: { } spriteSheet } member && spriteSheet.GetMemberCollection(member.GetType()) is { } collection && member.TryClone(out var clonedMember)) {
            this._undoService.Do(() =>
            {
                collection.AddMember(clonedMember);
                this.AssetSelectionService.Selected = clonedMember;
            }, () =>
            {
                collection.RemoveMember(clonedMember);
                this.AssetSelectionService.Selected = member;
            });
        }
        else if (selected is ScreenShader shader && shader.TryClone(out var clonedShader)) {
            var shaders = this.ProjectService.CurrentProject.ScreenShaders;
            this._undoService.Do(() =>
            {
                shaders.Add(clonedShader);
                this.AssetSelectionService.Selected = clonedShader;
            }, () =>
            {
                shaders.Remove(clonedShader);
                this.AssetSelectionService.Selected = shader;
            });
        }
        else if (selected is ContentFile { Asset: { } asset, Parent: { } directory } file) {
            var assetJson = Serializer.Instance.SerializeToString(asset);
            if (Serializer.Instance.DeserializeFromString(assetJson, asset.GetType()) is IAsset clone) {
                clone.SetNewIds();

                var newFileName = file.NameWithoutExtension;
                var files = this._fileSystem.GetFiles(directory.GetFullPath()).Select(Path.GetFileNameWithoutExtension).ToList();
                var count = 1;

                while (files.Contains(newFileName)) {
                    newFileName = $"{file.NameWithoutExtension} ({count})";
                    count++;
                }

                var newContentPath = Path.Combine(directory.GetContentPath(), newFileName).Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries);
                var newMetaData = new ContentMetadata(clone, newContentPath, file.Metadata.ContentFileExtension);
                var newContentFile = new ContentFile(directory, newMetaData);

                this._undoService.Do(() =>
                {
                    this._fileSystem.CopyFile(file.GetFullPath(), newContentFile.GetFullPath());
                    directory.AddChild(newContentFile);
                    this.AssetSelectionService.Selected = newContentFile;
                }, () =>
                {
                    this._fileSystem.DeleteFile(newContentFile.GetFullPath());
                    directory.RemoveChild(newContentFile);
                    this.AssetSelectionService.Selected = file;
                });
            }
        }
    }

    private async Task FindContentUsages(IContentNode node) {
        if (node is ContentFile && node.Id != Guid.Empty) {
            if (this._sceneService.CurrentScene.GetDescendantsWithContent(node.Id).Any()) {
                var entity = await this._dialogService.OpenEntitySelectionDialog(node.Id, "Select an Entity");

                if (!Entity.IsNullOrEmpty(entity, out var found)) {
                    Dispatcher.UIThread.Post(() =>
                    {
                        this._editorService.SelectedTab = EditorTabs.Scene;
                        this._sceneService.Selected = found;
                    });
                }
            }
            else {
                await this._dialogService.ShowWarningDialog("No Usages Found", "This content is not used in the current scene. It may still be used by unopened scenes.");
            }
        }
    }

    private void ForceSave(IContentNode node) {
        if (node is ContentFile { Metadata: not null } file) {
            this.ContentService.SaveMetadata(file.Metadata);
            file.HasChanges = false;
        }
    }

    private void MoveDown(object selected) {
        if (selected is SpriteSheetMember { SpriteSheet: { } spriteSheet } member && spriteSheet.GetMemberCollection(member.GetType()) is { } collection) {
            var originalIndex = collection.IndexOf(member);
            if (originalIndex < collection.Count - 1 && originalIndex >= 0) {
                var newIndex = originalIndex + 1;
                this._undoService.Do(() =>
                {
                    collection.Move(originalIndex, newIndex);
                    this.RaisePropertyChanged(nameof(this.IsMoveDownEnabled));
                    this.RaisePropertyChanged(nameof(this.IsMoveUpEnabled));
                }, () =>
                {
                    collection.Move(newIndex, originalIndex);
                    this.RaisePropertyChanged(nameof(this.IsMoveDownEnabled));
                    this.RaisePropertyChanged(nameof(this.IsMoveUpEnabled));
                });
            }
        }
        else if (selected is ScreenShader shader) {
            var shaders = this.ProjectService.CurrentProject.ScreenShaders;
            var originalIndex = shaders.IndexOf(shader);
            if (originalIndex < shaders.Count - 1 && originalIndex >= 0) {
                var newIndex = originalIndex + 1;
                this._undoService.Do(() =>
                {
                    shaders.Move(originalIndex, newIndex);
                    this.RaisePropertyChanged(nameof(this.IsMoveDownEnabled));
                    this.RaisePropertyChanged(nameof(this.IsMoveUpEnabled));
                }, () =>
                {
                    shaders.Move(newIndex, originalIndex);
                    this.RaisePropertyChanged(nameof(this.IsMoveDownEnabled));
                    this.RaisePropertyChanged(nameof(this.IsMoveUpEnabled));
                });
            }
        }
    }

    private void MoveUp(object selected) {
        if (selected is SpriteSheetMember { SpriteSheet: { } spriteSheet } member && spriteSheet.GetMemberCollection(member.GetType()) is { } collection) {
            var originalIndex = collection.IndexOf(member);
            if (originalIndex > 0) {
                var newIndex = originalIndex - 1;
                this._undoService.Do(() =>
                {
                    collection.Move(originalIndex, newIndex);
                    this.RaisePropertyChanged(nameof(this.IsMoveDownEnabled));
                    this.RaisePropertyChanged(nameof(this.IsMoveUpEnabled));
                }, () =>
                {
                    collection.Move(newIndex, originalIndex);
                    this.RaisePropertyChanged(nameof(this.IsMoveDownEnabled));
                    this.RaisePropertyChanged(nameof(this.IsMoveUpEnabled));
                });
            }
        }
        else if (selected is ScreenShader shader) {
            var shaders = this.ProjectService.CurrentProject.ScreenShaders;
            var originalIndex = shaders.IndexOf(shader);
            if (originalIndex > 0) {
                var newIndex = originalIndex - 1;
                this._undoService.Do(() =>
                {
                    shaders.Move(originalIndex, newIndex);
                    this.RaisePropertyChanged(nameof(this.IsMoveDownEnabled));
                    this.RaisePropertyChanged(nameof(this.IsMoveUpEnabled));
                }, () =>
                {
                    shaders.Move(newIndex, originalIndex);
                    this.RaisePropertyChanged(nameof(this.IsMoveDownEnabled));
                    this.RaisePropertyChanged(nameof(this.IsMoveUpEnabled));
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
            case ScreenShader shader:
                if (this.ProjectService.CurrentProject.ScreenShaders is var shaders && shaders.Contains(shader)) {
                    var index = shaders.IndexOf(shader);
                    this._undoService.Do(() => shaders.Remove(shader),
                        () => shaders.Insert(index, shader));
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

                        if (this._fileSystem.DoesFileExist(updatedPath) || this._fileSystem.DoesDirectoryExist(updatedPath)) {
                            await this._dialogService.ShowWarningDialog($"Invalid {typeName} Name", $"A {typeName.ToLower()} named '{updatedName}' already exists.");
                        }
                        else {
                            var originalNodeName = node.Name;
                            var isCurrentScene = node.Id == this._sceneService.CurrentSceneMetadata.ContentId;

                            if (!isCurrentScene) {
                                this._undoService.Do(() => { node.Name = updatedName; }, () => { node.Name = originalNodeName; });
                            }
                            else {
                                // Necessary so the name refreshes.
                                parent.RemoveChild(node);
                                parent.AddChild(node);
                                this.AssetSelectionService.Selected = node;

                                await this._dialogService.ShowWarningDialog(
                                    "Close Current Scene",
                                    "Rename cannot be performed on an open scene. Please close the scene and try again.");
                            }
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