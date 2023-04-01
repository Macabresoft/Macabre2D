namespace Macabresoft.Macabre2D.UI.Editor;

using System;
using System.Collections.Generic;
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
public sealed class SceneTreeViewModel : BaseViewModel {
    private readonly IContentService _contentService;
    private readonly ICommonDialogService _dialogService;
    private readonly ILoopService _loopService;
    private readonly IUndoService _undoService;

    /// <summary>
    /// Initializes a new instance of the <see cref="SceneTreeViewModel" /> class.
    /// </summary>
    /// <remarks>This constructor only exists for design time XAML.</remarks>
    public SceneTreeViewModel() {
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
        IUndoService undoService) {
        this._contentService = contentService;
        this._dialogService = dialogService;
        this.EditorService = editorService;
        this.EntityService = entityService;
        this.SceneService = sceneService;
        this._loopService = loopService;
        this._undoService = undoService;

        this.AddCommand = ReactiveCommand.CreateFromTask<Type>(async x => await this.AddChild(x));
        this.RemoveCommand = ReactiveCommand.Create<object>(this.RemoveChild, this.SceneService.WhenAny(
            x => x.ImpliedSelected,
            x => x.Value is not IScene));
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
    /// Gets the editor service.
    /// </summary>
    public IEditorService EditorService { get; }

    /// <summary>
    /// Gets the selection service.
    /// </summary>
    public IEntityService EntityService { get; }

    /// <summary>
    /// Gets a command to move a child down.
    /// </summary>
    public ICommand MoveDownCommand { get; }

    /// <summary>
    /// Gets a command to move a child up.
    /// </summary>
    public ICommand MoveUpCommand { get; }

    /// <summary>
    /// Gets a command to remove a child.
    /// </summary>
    public ICommand RemoveCommand { get; }

    /// <summary>
    /// Gets a command for renaming an entity or loop.
    /// </summary>
    public ICommand RenameCommand { get; }

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

    private bool CanClone(object selected) {
        return (selected is IEntity entity && entity != this.SceneService.CurrentScene) || selected is ILoop;
    }

    private bool CanMoveDown(object selected) {
        return selected switch {
            IEntity entity and not IScene when !Entity.IsNullOrEmpty(entity.Parent, out var parent) => parent.Children.IndexOf(entity) < parent.Children.Count - 1,
            ILoop loop => this.SceneService.CurrentScene.Loops.IndexOf(loop) < this.SceneService.CurrentScene.Loops.Count - 1,
            _ => false
        };
    }

    private static bool CanMoveEntity(IEntity sourceEntity, IEntity targetEntity) {
        return sourceEntity != null &&
               targetEntity != null &&
               sourceEntity != targetEntity &&
               !targetEntity.IsDescendentOf(sourceEntity);
    }

    private bool CanMoveUp(object selected) {
        return selected switch {
            IEntity entity and not IScene when !Entity.IsNullOrEmpty(entity.Parent, out var parent) => parent.Children.IndexOf(entity) != 0,
            ILoop loop => this.SceneService.CurrentScene.Loops.IndexOf(loop) != 0,
            _ => false
        };
    }

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
}