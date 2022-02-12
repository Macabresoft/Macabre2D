namespace Macabresoft.Macabre2D.UI.Editor;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
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
    private readonly ISystemService _systemService;
    private readonly IUndoService _undoService;
    private readonly ReactiveCommand<object, Unit> _moveUpCommand;
    private readonly ReactiveCommand<object, Unit> _moveDownCommand;

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
    /// <param name="systemService">The system service.</param>
    /// <param name="undoService">The undo service.</param>
    [InjectionConstructor]
    public SceneTreeViewModel(
        IContentService contentService,
        ICommonDialogService dialogService,
        IEditorService editorService,
        IEntityService entityService,
        ISceneService sceneService,
        ISystemService systemService,
        IUndoService undoService) {
        this._contentService = contentService;
        this._dialogService = dialogService;
        this.EditorService = editorService;
        this.EntityService = entityService;
        this.SceneService = sceneService;
        this._systemService = systemService;
        this._undoService = undoService;

        this.AddCommand = ReactiveCommand.CreateFromTask<Type>(async x => await this.AddChild(x));
        this.RemoveCommand = ReactiveCommand.Create<object>(this.RemoveChild, this.SceneService.WhenAny(
            x => x.ImpliedSelected,
            x => x.Value is not IScene));
        this._moveDownCommand = ReactiveCommand.Create<object>(this.MoveDown, this.SceneService.WhenAny(
            x => x.ImpliedSelected,
            x => this.CanMoveDown(x.Value)));
        this._moveUpCommand = ReactiveCommand.Create<object>(this.MoveUp, this.SceneService.WhenAny(
            x => x.ImpliedSelected,
            x => this.CanMoveUp(x.Value)));
        this.RenameCommand = ReactiveCommand.Create<string>(this.RenameChild);
        this.CloneEntityCommand = ReactiveCommand.Create<IEntity>(this.CloneEntity);
        this.ConvertToInstanceCommand = ReactiveCommand.Create<IEntity>(this.ConvertToInstance);
        this.CreatePrefabCommand = ReactiveCommand.CreateFromTask<IEntity>(async x => await this.CreateFromPrefab(x));

        this.AddEntityModels = this.EntityService.AvailableTypes.OrderBy(x => x.Name)
            .Select(x => new MenuItemModel(x.Name, x.FullName, this.AddCommand, x)).ToList();
        this.AddSystemModels = this._systemService.AvailableTypes.OrderBy(x => x.Name)
            .Select(x => new MenuItemModel(x.Name, x.FullName, this.AddCommand, x)).ToList();
    }

    /// <summary>
    /// Gets a command to add a system or an entity.
    /// </summary>
    public ICommand AddCommand { get; }

    /// <summary>
    /// Gets a collection of <see cref="MenuItemModel" /> for adding entities.
    /// </summary>
    public IReadOnlyCollection<MenuItemModel> AddEntityModels { get; }

    /// <summary>
    /// Gets a collection of <see cref="MenuItemModel" /> for adding systems.
    /// </summary>
    public IReadOnlyCollection<MenuItemModel> AddSystemModels { get; }

    /// <summary>
    /// Gets a command to clone an entity.
    /// </summary>
    public ICommand CloneEntityCommand { get; }

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
    public ICommand MoveDownCommand => this._moveDownCommand;

    /// <summary>
    /// Gets a command to move a child up.
    /// </summary>
    public ICommand MoveUpCommand => this._moveUpCommand;

    /// <summary>
    /// Gets a command to remove a child.
    /// </summary>
    public ICommand RemoveCommand { get; }

    /// <summary>
    /// Gets a command for renaming an entity or system..
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
            this._undoService.Do(() => {
                targetEntity.AddChild(sourceEntity);
                this.SceneService.RaiseSelectedChanged();
            }, () => {
                originalParent.AddChild(sourceEntity);
                this.SceneService.RaiseSelectedChanged();
            });
        }
    }

    private async Task AddChild(Type type) {
        if (type == null) {
            if (this.SceneService.ImpliedSelected is IScene or IEntity) {
                await this.AddEntity(null);
            }
            else if (this.SceneService.Selected is IUpdateableSystem or SystemCollection) {
                await this.AddSystem(null);
            }
        }
        else if (type.IsAssignableTo(typeof(IEntity))) {
            await this.AddEntity(null);
        }
        else if (type.IsAssignableTo(typeof(IUpdateableSystem))) {
            await this.AddSystem(null);
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
                this._undoService.Do(() => {
                    Dispatcher.UIThread.Post(() => {
                        parent.AddChild(child);
                        this.SceneService.Selected = child;
                    });
                }, () => {
                    Dispatcher.UIThread.Post(() => {
                        parent.RemoveChild(child);
                        this.SceneService.Selected = parent;
                    });
                });
            }
        }
    }

    private async Task AddSystem(Type type) {
        if (this.SceneService.CurrentScene is { } scene) {
            if (type == null || type.IsInterface) {
                type = await this._dialogService.OpenTypeSelectionDialog(this._systemService.AvailableTypes);
            }

            if (type != null && Activator.CreateInstance(type) is IUpdateableSystem system) {
                var originallySelected = this._systemService.Selected;
                this._undoService.Do(() => {
                    Dispatcher.UIThread.Post(() => {
                        scene.AddSystem(system);
                        this.SceneService.Selected = system;
                    });
                }, () => {
                    Dispatcher.UIThread.Post(() => {
                        scene.RemoveSystem(system);
                        this.SceneService.Selected = originallySelected;
                    });
                });
            }
        }
    }

    private bool CanMoveDown(object selected) {
        return selected switch {
            IEntity entity and not IScene when !Entity.IsNullOrEmpty(entity.Parent, out var parent) => parent.Children.IndexOf(entity) < parent.Children.Count - 1,
            IUpdateableSystem system => this.SceneService.CurrentScene.Systems.IndexOf(system) < this.SceneService.CurrentScene.Systems.Count - 1,
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
            IUpdateableSystem system => this.SceneService.CurrentScene.Systems.IndexOf(system) != 0,
            _ => false
        };
    }

    private void CloneEntity(IEntity entity) {
        if (entity is { Parent: { } parent } && entity.TryClone(out var clone)) {
            this._undoService.Do(() => { parent.AddChild(clone); }, () => { parent.RemoveChild(clone); });
        }
    }

    private void ConvertToInstance(IEntity entity) {
        if (entity.Parent is { } parent &&
            !Entity.IsNullOrEmpty(parent, out _) &&
            entity is PrefabContainer container &&
            container.PrefabReference.Asset?.Content is IEntity content &&
            content.TryClone(out var clone)) {
            clone.LocalPosition = entity.LocalPosition;
            clone.LocalScale = entity.LocalScale;

            this._undoService.Do(() => {
                parent.AddChild(clone);
                parent.RemoveChild(entity);
            }, () => {
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
                this._undoService.Do(() => {
                    this.MoveEntity(entity, parent, index + 1);
                }, () => {
                    this.MoveEntity(entity, parent, index);
                });
                break;
            }
            case IUpdateableSystem system: {
                var index = this.SceneService.CurrentScene.Systems.IndexOf(system);
                this._undoService.Do(() => {
                    this.MoveSystem(system, index + 1);
                }, () => {
                    this.MoveSystem(system, index);
                });
                break;
            }
        }
        
        
    }

    private void MoveUp(object selected) {
        switch (selected) {
            case IEntity entity and not IScene when !Entity.IsNullOrEmpty(entity.Parent, out var parent): {
                var index = parent.Children.IndexOf(entity);
                this._undoService.Do(() => {
                    this.MoveEntity(entity, parent, index - 1);
                }, () => {
                    this.MoveEntity(entity, parent, index);
                });
                break;
            }
            case IUpdateableSystem system: {
                var index = this.SceneService.CurrentScene.Systems.IndexOf(system);
                this._undoService.Do(() => {
                    this.MoveSystem(system, index - 1);
                }, () => {
                    this.MoveSystem(system, index);
                });
                break;
            }
        }
    }

    private void MoveEntity(IEntity entity, IEntity parent, int index) {
        parent.RemoveChild(entity);
        parent.InsertChild(index, entity);
        this.SceneService.RaiseSelectedChanged();
    }

    private void MoveSystem(IUpdateableSystem system, int index) {
        this.SceneService.CurrentScene.RemoveSystem(system);
        this.SceneService.CurrentScene.InsertSystem(index, system);
        this.SceneService.RaiseSelectedChanged();
    }

    private void RemoveChild(object child) {
        switch (child) {
            case IEntity entity and not IScene:
                this.RemoveEntity(entity);
                break;
            case IUpdateableSystem system:
                this.RemoveSystem(system);
                break;
        }
    }

    private void RemoveEntity(object selected) {
        if (selected is IEntity entity) {
            var parent = entity.Parent;
            this._undoService.Do(() => {
                Dispatcher.UIThread.Post(() => {
                    parent.RemoveChild(entity);
                    this.SceneService.Selected = null;
                });
            }, () => {
                Dispatcher.UIThread.Post(() => {
                    parent.AddChild(entity);
                    this.SceneService.Selected = entity;
                });
            });
        }
    }

    private void RemoveSystem(object selected) {
        if (selected is IUpdateableSystem system && this.SceneService.CurrentScene is { } scene) {
            this._undoService.Do(() => {
                Dispatcher.UIThread.Post(() => {
                    scene.RemoveSystem(system);
                    this.SceneService.Selected = null;
                });
            }, () => {
                Dispatcher.UIThread.Post(() => {
                    scene.AddSystem(system);
                    this.SceneService.Selected = system;
                });
            });
        }
    }

    private void RenameChild(string updatedName) {
        if (this.SceneService.ImpliedSelected is INameable nameable && nameable.Name != updatedName) {
            var originalName = nameable.Name;
            this._undoService.Do(
                () => { Dispatcher.UIThread.Post(() => { nameable.Name = updatedName; }); },
                () => { Dispatcher.UIThread.Post(() => { nameable.Name = originalName; }); });
        }
    }
}