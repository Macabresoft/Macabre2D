namespace Macabresoft.Macabre2D.UI.Editor;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
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
/// A view model for the scene view.
/// </summary>
public sealed class SceneTreeViewModel : FilterableViewModel<INameable> {
    private readonly IContentService _contentService;
    private readonly ICommonDialogService _dialogService;
    private readonly ILoopService _loopService;
    private readonly IUndoService _undoService;

    /// <summary>
    /// Initializes a new instance of the <see cref="SceneTreeViewModel" /> class.
    /// </summary>
    /// <remarks>This constructor only exists for design time XAML.</remarks>
    public SceneTreeViewModel() : base() {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SceneTreeViewModel" /> class.
    /// </summary>
    /// <param name="contentService">The content service.</param>
    /// <param name="dialogService">The dialog service.</param>
    /// <param name="editorService">The editor service.</param>
    /// <param name="entityService">The selection service.</param>
    /// <param name="sceneService">The scene service.</param>
    /// <param name="loopService">The loop service.</param>
    /// <param name="undoService">The undo service.</param>
    [InjectionConstructor]
    public SceneTreeViewModel(
        IContentService contentService,
        ICommonDialogService dialogService,
        IEditorService editorService,
        IEntityService entityService,
        ISceneService sceneService,
        ILoopService loopService,
        IUndoService undoService) : base() {
        this._contentService = contentService;
        this._dialogService = dialogService;
        this.EditorService = editorService;
        this.EntityService = entityService;
        this.SceneService = sceneService;
        this._loopService = loopService;
        this._undoService = undoService;

        this.SceneService.PropertyChanged += this.SceneService_PropertyChanged;

        this.AddCommand = ReactiveCommand.CreateFromTask<Type>(async x => await this.AddChild(x), this.WhenAny(
            x => x.IsFiltered,
            _ => this.CanAdd()));
        this.RemoveCommand = ReactiveCommand.Create<object>(this.RemoveChild, this.SceneService.WhenAny(
            x => x.ImpliedSelected,
            x => this.CanRemove(x.Value)));
        this.MoveDownCommand = ReactiveCommand.Create<object>(this.MoveDown, this.SceneService.WhenAny(
            x => x.ImpliedSelected,
            x => this.CanMoveDown(x.Value)));
        this.MoveUpCommand = ReactiveCommand.Create<object>(this.MoveUp, this.SceneService.WhenAny(
            x => x.ImpliedSelected,
            x => this.CanMoveUp(x.Value)));
        this.RenameCommand = ReactiveCommand.Create<string>(this.RenameChild);
        this.CloneCommand = ReactiveCommand.Create<object>(this.Clone, this.SceneService.WhenAny(
            x => x.ImpliedSelected,
            x => this.CanClone(x.Value)));
        this.ConvertToInstanceCommand = ReactiveCommand.Create<IEntity>(this.ConvertToInstance);
        this.CreatePrefabCommand = ReactiveCommand.CreateFromTask<IEntity>(async x => await this.CreateFromPrefab(x));
        this.ReinitializeCommand = ReactiveCommand.Create<IEntity>(this.Reinitialize);
        this.EnableCommand = ReactiveCommand.Create<IEntity>(x => this.SetIsEnabled(x, true));
        this.DisableCommand = ReactiveCommand.Create<IEntity>(x => this.SetIsEnabled(x, false));
        this.RevealCommand = ReactiveCommand.Create<IEntity>(x => this.SetIsVisible(x, true));
        this.HideCommand = ReactiveCommand.Create<IEntity>(x => this.SetIsVisible(x, false));

        this.AddEntityModels = this.EntityService.AvailableTypes.OrderBy(x => x.Name)
            .Select(x => new MenuItemModel(x.Name, x.FullName, this.AddCommand, x)).ToList();
        this.AddLoopModels = this._loopService.AvailableTypes.OrderBy(x => x.Name)
            .Select(x => new MenuItemModel(x.Name, x.FullName, this.AddCommand, x)).ToList();
    }

    /// <summary>
    /// Gets a command to add a loop or an entity.
    /// </summary>
    public ICommand AddCommand { get; }

    /// <summary>
    /// Gets a collection of <see cref="MenuItemModel" /> for adding entities.
    /// </summary>
    public IReadOnlyCollection<MenuItemModel> AddEntityModels { get; }

    /// <summary>
    /// Gets a collection of <see cref="MenuItemModel" /> for adding loops.
    /// </summary>
    public IReadOnlyCollection<MenuItemModel> AddLoopModels { get; }

    /// <summary>
    /// Gets a command to clone an entity.
    /// </summary>
    public ICommand CloneCommand { get; }

    /// <summary>
    /// Gets a command to convert a prefab into an instance.
    /// </summary>
    public ICommand ConvertToInstanceCommand { get; }

    /// <summary>
    /// Gets a command to create a prefab.
    /// </summary>
    public ICommand CreatePrefabCommand { get; }

    /// <summary>
    /// Gets a command to disable an entity and its descendants.
    /// </summary>
    public ICommand DisableCommand { get; }

    /// <summary>
    /// Gets the editor service.
    /// </summary>
    public IEditorService EditorService { get; }

    /// <summary>
    /// Gets a command to enable an entity and its descendants.
    /// </summary>
    public ICommand EnableCommand { get; }

    /// <summary>
    /// Gets the selection service.
    /// </summary>
    public IEntityService EntityService { get; }

    /// <summary>
    /// Gets a command to hide an entity and its descendants.
    /// </summary>
    public ICommand HideCommand { get; }

    /// <summary>
    /// Gets or sets a value indicating whether an entity or loop is selected.
    /// </summary>
    public bool IsEntityOrLoopSelected => this.CanClone(this.SceneService.Selected);

    /// <summary>
    /// Gets a command to move a child down.
    /// </summary>
    public ICommand MoveDownCommand { get; }

    /// <summary>
    /// Gets a command to move a child up.
    /// </summary>
    public ICommand MoveUpCommand { get; }

    /// <summary>
    /// Gets a command to re-initialize the selected entity.
    /// </summary>
    public ICommand ReinitializeCommand { get; }

    /// <summary>
    /// Gets a command to remove a child.
    /// </summary>
    public ICommand RemoveCommand { get; }

    /// <summary>
    /// Gets a command for renaming an entity or loop.
    /// </summary>
    public ICommand RenameCommand { get; }

    /// <summary>
    /// Gets a command to reveal an entity and its descendants.
    /// </summary>
    public ICommand RevealCommand { get; }

    /// <summary>
    /// Gets the scene service.
    /// </summary>
    public ISceneService SceneService { get; }

    /// <summary>
    /// Moves the source entity to be a child of the target entity.
    /// </summary>
    /// <param name="sourceEntity">The source entity.</param>
    /// <param name="targetEntity">The target entity.</param>
    public void MoveEntity(IEntity sourceEntity, IEntity targetEntity) {
        if (CanMoveEntity(sourceEntity, targetEntity)) {
            var originalParent = sourceEntity.Parent;
            var worldPosition = sourceEntity.WorldPosition;
            this._undoService.Do(() =>
            {
                targetEntity.AddChild(sourceEntity);
                sourceEntity.SetWorldPosition(worldPosition);
                this.SceneService.RaiseSelectedChanged();
            }, () =>
            {
                originalParent.AddChild(sourceEntity);
                sourceEntity.SetWorldPosition(worldPosition);
                this.SceneService.RaiseSelectedChanged();
            });
        }
    }

    /// <summary>
    /// Moves the source entity to be a child of the target entity.
    /// </summary>
    /// <param name="sourceEntity">The source entity.</param>
    /// <param name="targetEntity">The target entity.</param>
    /// <param name="index">The index at which to insert.</param>
    public void MoveEntity(IEntity sourceEntity, IEntity targetEntity, int index) {
        if (CanMoveEntity(sourceEntity, targetEntity)) {
            var originalParent = sourceEntity.Parent;
            var originalIndex = originalParent.Children.IndexOf(sourceEntity);
            var worldPosition = sourceEntity.WorldPosition;
            this._undoService.Do(() =>
            {
                if (originalParent != targetEntity) {
                    targetEntity.InsertChild(index, sourceEntity);
                }
                else {
                    this.MoveEntityByIndex(sourceEntity, targetEntity, index);
                }

                sourceEntity.SetWorldPosition(worldPosition);
                this.SceneService.RaiseSelectedChanged();
            }, () =>
            {
                if (originalParent != targetEntity) {
                    originalParent.InsertChild(originalIndex, sourceEntity);
                }
                else {
                    this.MoveEntityByIndex(sourceEntity, originalParent, originalIndex);
                }

                sourceEntity.SetWorldPosition(worldPosition);
                this.SceneService.RaiseSelectedChanged();
            });
        }
    }

    /// <summary>
    /// Moves a loop.
    /// </summary>
    /// <param name="loop">The loop.</param>
    /// <param name="newIndex">The new index.</param>
    public void MoveLoop(ILoop loop, int newIndex) {
        if (this.SceneService.CurrentScene.Loops is LoopCollection collection) {
            var originalIndex = collection.IndexOf(loop);
            this._undoService.Do(() =>
            {
                collection.Move(originalIndex, newIndex);
                this.SceneService.RaiseSelectedChanged();
            }, () =>
            {
                collection.Move(newIndex, originalIndex);
                this.SceneService.RaiseSelectedChanged();
            });
        }
    }

    /// <inheritdoc />
    protected override INameable GetActualSelected() => this.SceneService.Selected as INameable;

    /// <inheritdoc />
    protected override IEnumerable<INameable> GetNodesAvailableToFilter() {
        if (!Scene.IsNullOrEmpty(this.SceneService.CurrentScene)) {
            var nodes = new List<INameable> {
                this.SceneService.CurrentScene
            };

            nodes.AddRange(this.SceneService.CurrentScene.Loops);
            nodes.AddRange(this.SceneService.CurrentScene.GetDescendants<IEntity>());
            return nodes;
        }

        return Enumerable.Empty<INameable>();
    }

    /// <inheritdoc />
    protected override void SetActualSelected(INameable selected) {
        this.SceneService.Selected = selected;
    }

    private async Task AddChild(Type type) {
        if (type == null) {
            if (this.SceneService.Selected is ILoop or LoopCollection) {
                await this.AddLoop(null);
            }
            else if (this.SceneService.ImpliedSelected is IScene or IEntity) {
                await this.AddEntity(null);
            }
        }
        else if (type.IsAssignableTo(typeof(IEntity))) {
            await this.AddEntity(type);
        }
        else if (type.IsAssignableTo(typeof(ILoop))) {
            await this.AddLoop(type);
        }
    }

    private async Task AddEntity(Type type) {
        if (type == null || type.IsInterface) {
            type = await this._dialogService.OpenTypeSelectionDialog(this.EntityService.AvailableTypes);
        }

        if (type != null && Activator.CreateInstance(type) is IEntity child) {
            if (type.GetCustomAttribute(typeof(DataContractAttribute)) is DataContractAttribute attribute) {
                child.Name = string.IsNullOrEmpty(attribute.Name) ? type.Name : attribute.Name;
            }
            else {
                child.Name = type.Name;
            }

            var parent = this.EntityService.Selected ?? this.SceneService.CurrentScene;

            if (parent != null) {
                this._undoService.Do(() =>
                {
                    Dispatcher.UIThread.Post(() =>
                    {
                        parent.AddChild(child);
                        this.SceneService.Selected = child;
                    });
                }, () =>
                {
                    Dispatcher.UIThread.Post(() =>
                    {
                        parent.RemoveChild(child);
                        this.SceneService.Selected = parent;
                    });
                });
            }
        }
    }

    private async Task AddLoop(Type type) {
        if (this.SceneService.CurrentScene is { } scene) {
            if (type == null || type.IsInterface) {
                type = await this._dialogService.OpenTypeSelectionDialog(this._loopService.AvailableTypes);
            }

            if (type != null && Activator.CreateInstance(type) is ILoop loop) {
                var originallySelected = this._loopService.Selected;
                this._undoService.Do(() =>
                {
                    Dispatcher.UIThread.Post(() =>
                    {
                        scene.AddLoop(loop);
                        this.SceneService.Selected = loop;
                    });
                }, () =>
                {
                    Dispatcher.UIThread.Post(() =>
                    {
                        scene.RemoveLoop(loop);
                        this.SceneService.Selected = originallySelected;
                    });
                });
            }
        }
    }

    private bool CanAdd() => !this.IsFiltered;

    private bool CanClone(object selected) => !this.IsFiltered && (selected is IEntity entity && entity != this.SceneService.CurrentScene || selected is ILoop);

    private bool CanMoveDown(object selected) {
        if (this.IsFiltered) {
            return false;
        }

        return selected switch {
            IEntity entity and not IScene when !Entity.IsNullOrEmpty(entity.Parent, out var parent) => parent.Children.IndexOf(entity) < parent.Children.Count - 1,
            ILoop loop => this.SceneService.CurrentScene.Loops.IndexOf(loop) < this.SceneService.CurrentScene.Loops.Count - 1,
            _ => false
        };
    }

    private static bool CanMoveEntity(IEntity sourceEntity, IEntity targetEntity) =>
        sourceEntity != null &&
        targetEntity != null &&
        sourceEntity != targetEntity &&
        !targetEntity.IsDescendentOf(sourceEntity);

    private bool CanMoveUp(object selected) {
        if (this.IsFiltered) {
            return false;
        }

        return selected switch {
            IEntity entity and not IScene when !Entity.IsNullOrEmpty(entity.Parent, out var parent) => parent.Children.IndexOf(entity) != 0,
            ILoop loop => this.SceneService.CurrentScene.Loops.IndexOf(loop) != 0,
            _ => false
        };
    }

    private bool CanRemove(object value) => !this.IsFiltered && value is not IScene;

    private void Clone(object selected) {
        if (selected is IEntity entity) {
            if (entity != this.SceneService.CurrentScene && entity is { Parent: { } parent } && entity.TryClone(out var clone)) {
                this._undoService.Do(() =>
                {
                    parent.AddChild(clone);
                    this.SceneService.Selected = clone;
                }, () =>
                {
                    parent.RemoveChild(clone);
                    this.SceneService.Selected = entity;
                });
            }
        }
        else if (selected is ILoop loop) {
            if (loop.TryClone(out var clone)) {
                this._undoService.Do(() =>
                {
                    this.SceneService.CurrentScene.AddLoop(clone);
                    this.SceneService.Selected = clone;
                }, () =>
                {
                    this.SceneService.CurrentScene.RemoveLoop(clone);
                    this.SceneService.Selected = loop;
                });
            }
        }
    }

    private void ConvertToInstance(IEntity entity) {
        if (entity.Parent is { } parent &&
            !Entity.IsNullOrEmpty(parent, out _) &&
            entity is PrefabContainer { PrefabReference.Asset.Content: IEntity content } &&
            content.TryClone(out var clone)) {
            clone.LocalPosition = entity.LocalPosition;

            this._undoService.Do(() =>
            {
                parent.AddChild(clone);
                parent.RemoveChild(entity);
            }, () =>
            {
                parent.AddChild(entity);
                parent.RemoveChild(clone);
            });
        }
    }

    private async Task CreateFromPrefab(IEntity entity) {
        await this._contentService.CreatePrefab(entity);
    }

    private void MoveDown(object selected) {
        switch (selected) {
            case IEntity entity and not IScene when !Entity.IsNullOrEmpty(entity.Parent, out var parent): {
                var index = parent.Children.IndexOf(entity);
                this._undoService.Do(() => { this.MoveEntityByIndex(entity, parent, index + 1); }, () => { this.MoveEntityByIndex(entity, parent, index); });
                break;
            }
            case ILoop loop: {
                var index = this.SceneService.CurrentScene.Loops.IndexOf(loop);
                this._undoService.Do(() => { this.MoveLoopByIndex(loop, index + 1); }, () => { this.MoveLoopByIndex(loop, index); });
                break;
            }
        }
    }

    private void MoveEntityByIndex(IEntity entity, IEntity parent, int index) {
        parent.ReorderChild(entity, index);
        this.SceneService.RaiseSelectedChanged();
    }

    private void MoveLoopByIndex(ILoop loop, int index) {
        this.SceneService.CurrentScene.ReorderLoop(loop, index);
        this.SceneService.RaiseSelectedChanged();
    }

    private void MoveUp(object selected) {
        switch (selected) {
            case IEntity entity and not IScene when !Entity.IsNullOrEmpty(entity.Parent, out var parent): {
                var index = parent.Children.IndexOf(entity);
                this._undoService.Do(() => { this.MoveEntityByIndex(entity, parent, index - 1); }, () => { this.MoveEntityByIndex(entity, parent, index); });
                break;
            }
            case ILoop loop: {
                var index = this.SceneService.CurrentScene.Loops.IndexOf(loop);
                this._undoService.Do(() => { this.MoveLoopByIndex(loop, index - 1); }, () => { this.MoveLoopByIndex(loop, index); });
                break;
            }
        }
    }

    private void Reinitialize(IEntity entity) {
        entity?.Initialize(entity.Scene, entity.Parent);
    }

    private void RemoveChild(object child) {
        switch (child) {
            case IEntity entity and not IScene:
                this.RemoveEntity(entity);
                break;
            case ILoop loop:
                this.RemoveLoop(loop);
                break;
        }
    }

    private void RemoveEntity(object selected) {
        if (selected is IEntity entity) {
            var parent = entity.Parent;
            this._undoService.Do(() =>
            {
                parent.RemoveChild(entity);
                this.SceneService.Selected = null;
            }, () =>
            {
                parent.AddChild(entity);
                this.SceneService.Selected = entity;
            });
        }
    }

    private void RemoveLoop(object selected) {
        if (selected is ILoop loop && this.SceneService.CurrentScene is { } scene) {
            this._undoService.Do(() =>
            {
                scene.RemoveLoop(loop);
                this.SceneService.Selected = null;
            }, () =>
            {
                scene.AddLoop(loop);
                this.SceneService.Selected = loop;
            });
        }
    }

    private void RenameChild(string updatedName) {
        if (this.SceneService.ImpliedSelected is INameable nameable && nameable.Name != updatedName) {
            var originalName = nameable.Name;
            this._undoService.Do(
                () => { nameable.Name = updatedName; },
                () => { nameable.Name = originalName; });
        }
    }

    private void SceneService_PropertyChanged(object sender, PropertyChangedEventArgs e) {
        if (e.PropertyName == nameof(this.SceneService.Selected)) {
            this.RaisePropertyChanged(nameof(this.IsEntityOrLoopSelected));
        }
    }

    private void SetIsEnabled(IEntity entity, bool isEnabled) {
        var entityToIsEnabled = new List<(IEnableable Entity, bool IsEnabled)> { (entity, entity.IsEnabled) };
        entityToIsEnabled.AddRange(entity.GetDescendants<IEnableable>().Select(child => (child, child.IsEnabled)));

        this._undoService.Do(() =>
        {
            foreach (var entry in entityToIsEnabled) {
                entry.Entity.IsEnabled = isEnabled;
            }
        }, () =>
        {
            foreach (var entry in entityToIsEnabled) {
                entry.Entity.IsEnabled = entry.IsEnabled;
            }
        });
    }

    private void SetIsVisible(IEntity entity, bool isVisible) {
        var entityToIsVisible = new List<(IRenderableEntity Entity, bool IsVisible)>();
        if (entity is IRenderableEntity renderable) {
            entityToIsVisible.Add((renderable, renderable.IsVisible));
        }

        entityToIsVisible.AddRange(entity.GetDescendants<IRenderableEntity>().Select(child => (child, child.IsVisible)));

        this._undoService.Do(() =>
        {
            foreach (var entry in entityToIsVisible) {
                entry.Entity.IsVisible = isVisible;
            }
        }, () =>
        {
            foreach (var entry in entityToIsVisible) {
                entry.Entity.IsVisible = entry.IsVisible;
            }
        });
    }
}