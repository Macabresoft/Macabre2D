namespace Macabresoft.Macabre2D.UI.Editor;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Threading;
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
        this.SystemService = systemService;
        this._undoService = undoService;

        this.AddEntityCommand = ReactiveCommand.CreateFromTask<Type>(async x => await this.AddEntity(x));
        this.RemoveEntityCommand = ReactiveCommand.Create<IEntity>(this.RemoveEntity);
        this.AddSystemCommand = ReactiveCommand.CreateFromTask<Type>(async x => await this.AddSystem(x));
        this.RemoveSystemCommand = ReactiveCommand.Create<IUpdateableSystem>(this.RemoveSystem);
        this.RenameEntityCommand = ReactiveCommand.Create<string>(this.RenameEntity);
        this.RenameSystemCommand = ReactiveCommand.Create<string>(this.RenameSystem);
        this.CloneEntityCommand = ReactiveCommand.Create<IEntity>(this.CloneEntity);
        this.ConvertToInstanceCommand = ReactiveCommand.Create<IEntity>(this.ConvertToInstance);
        this.CreatePrefabCommand = ReactiveCommand.CreateFromTask<IEntity>(async x => await this.CreateFromPrefab(x));

        this.AddEntityModels = this.EntityService.AvailableTypes.OrderBy(x => x.Name)
            .Select(x => new MenuItemModel(x.Name, x.FullName, this.AddEntityCommand, x)).ToList();
        this.AddSystemModels = this.SystemService.AvailableTypes.OrderBy(x => x.Name)
            .Select(x => new MenuItemModel(x.Name, x.FullName, this.AddSystemCommand, x)).ToList();
    }

    /// <summary>
    /// Gets a command to add an entity.
    /// </summary>
    public ICommand AddEntityCommand { get; }

    /// <summary>
    /// Gets a collection of <see cref="MenuItemModel" /> for adding entities.
    /// </summary>
    public IReadOnlyCollection<MenuItemModel> AddEntityModels { get; }

    /// <summary>
    /// Gets a command to add a system.
    /// </summary>
    public ICommand AddSystemCommand { get; }

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
    /// Gets a command to remove an entity.
    /// </summary>
    public ICommand RemoveEntityCommand { get; }

    /// <summary>
    /// Gets a command to remove a system.
    /// </summary>
    public ICommand RemoveSystemCommand { get; }

    /// <summary>
    /// Gets a command for renaming an entity.
    /// </summary>
    public ICommand RenameEntityCommand { get; }

    /// <summary>
    /// Gets a command for renaming a system.
    /// </summary>
    public ICommand RenameSystemCommand { get; }

    /// <summary>
    /// Gets the scene service.
    /// </summary>
    public ISceneService SceneService { get; }

    /// <summary>
    /// Gets the system service.
    /// </summary>
    public ISystemService SystemService { get; }

    /// <summary>
    /// Moves the source entity to be a child of the target entity.
    /// </summary>
    /// <param name="sourceEntity">The source entity.</param>
    /// <param name="targetEntity">The target entity.</param>
    public void MoveEntity(IEntity sourceEntity, IEntity targetEntity) {
        if (CanMoveEntity(sourceEntity, targetEntity)) {
            var originalParent = sourceEntity.Parent;
            this._undoService.Do(() => { targetEntity.AddChild(sourceEntity); }, () => { originalParent.AddChild(sourceEntity); });
        }
    }

    private async Task AddEntity(Type type) {
        type ??= await this._dialogService.OpenTypeSelectionDialog(this.EntityService.AvailableTypes);

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
            type ??= await this._dialogService.OpenTypeSelectionDialog(this.SystemService.AvailableTypes);

            if (type != null && Activator.CreateInstance(type) is IUpdateableSystem system) {
                var originallySelected = this.SystemService.Selected;
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

    private static bool CanMoveEntity(IEntity sourceEntity, IEntity targetEntity) {
        return sourceEntity != null &&
               targetEntity != null &&
               sourceEntity != targetEntity &&
               !targetEntity.IsDescendentOf(sourceEntity);
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

    private void RenameEntity(string updatedName) {
        if (this.EntityService.Selected is { } entity && entity.Name != updatedName) {
            var originalName = entity.Name;
            this._undoService.Do(
                () => { Dispatcher.UIThread.Post(() => { entity.Name = updatedName; }); }, () => { Dispatcher.UIThread.Post(() => { entity.Name = originalName; }); });
        }
    }

    private void RenameSystem(string updatedName) {
        if (this.SystemService.Selected is { } system && system.Name != updatedName) {
            var originalName = system.Name;
            this._undoService.Do(
                () => { Dispatcher.UIThread.Post(() => { system.Name = updatedName; }); },
                () => { Dispatcher.UIThread.Post(() => { system.Name = originalName; }); });
        }
    }
}