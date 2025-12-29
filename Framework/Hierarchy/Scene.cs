namespace Macabresoft.Macabre2D.Framework;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.Serialization;
using Macabresoft.Core;
using Macabresoft.Macabre2D.Project.Common;
using Microsoft.Xna.Framework;

/// <summary>
/// Interface for a combination of <see cref="IGameSystem" /> and <see cref="IEntity" />
/// which runs on a <see cref="IGame" />.
/// </summary>
public interface IScene : IUpdateableGameObject, IGridContainer, IBoundableEntity {
    /// <summary>
    /// Occurs when the scene is activated and made the current scene on <see cref="IGame" />.
    /// </summary>
    /// <remarks>
    /// This is necessary, because we do not want initialization to happen when a scene is popped,
    /// exposing another scene as the current scene; however, some entities still need to know that
    /// they are back in focus.
    /// </remarks>
    event EventHandler Activated;

    /// <summary>
    /// Occurs when the scene is deactivated and made to no longer be the current scene on <see cref="IGame" />.
    /// </summary>
    event EventHandler Deactivated;

    /// <summary>
    /// Gets the animatable entities.
    /// </summary>
    IReadOnlyCollection<IAnimatableEntity> AnimatableEntities => [];

    /// <summary>
    /// Gets the asset manager.
    /// </summary>
    IAssetManager Assets => AssetManager.Empty;

    /// <summary>
    /// Gets or sets the color of the background.
    /// </summary>
    Color BackgroundColor { get; set; }

    /// <summary>
    /// Gets the cameras in the scene.
    /// </summary>
    IReadOnlyCollection<ICamera> Cameras => [];

    /// <summary>
    /// Gets the fixed updateable entities.
    /// </summary>
    IReadOnlyCollection<IFixedUpdateableEntity> FixedUpdateableEntities => [];

    /// <summary>
    /// Gets or sets a value indicating whether this is active.
    /// </summary>
    bool IsActive { get; }

    /// <summary>
    /// Gets the named children.
    /// </summary>
    IReadOnlyCollection<INameableCollection> NamedChildren => [];

    /// <summary>
    /// Gets the physics bodies.
    /// </summary>
    IReadOnlyCollection<IPhysicsBody> PhysicsBodies => [];

    /// <summary>
    /// Gets the renderable entities in the scene.
    /// </summary>
    IReadOnlyCollection<IRenderableEntity> RenderableEntities => [];

    /// <summary>
    /// Gets the scene state.
    /// </summary>
    SceneState State { get; }

    /// <summary>
    /// Gets the systems.
    /// </summary>
    IReadOnlyCollection<IGameSystem> Systems => [];

    /// <summary>
    /// Gets the updateable entities.
    /// </summary>
    IReadOnlyCollection<IUpdateableEntity> UpdateableEntities => [];

    /// <summary>
    /// Adds the system.
    /// </summary>
    /// <typeparam name="T">
    /// A type that implements <see cref="IGameSystem" /> and has an empty constructor.
    /// </typeparam>
    /// <returns>The added system.</returns>
    T AddSystem<T>() where T : IGameSystem, new();

    /// <summary>
    /// Adds the system.
    /// </summary>
    /// <param name="system">The system.</param>
    void AddSystem(IGameSystem system);

    /// <summary>
    /// Gets a value indicating whether this scene contains an entity with the specified identifier.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns>A value indicating whether this scene contains an entity with the specified identifier.</returns>
    bool ContainsEntity(Guid id);

    /// <summary>
    /// Finds an entity by its identifier.
    /// </summary>
    /// <param name="id">The entity identifier.</param>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <returns>The entity if found.</returns>
    TEntity? FindEntity<TEntity>(Guid id) where TEntity : class, IEntity;

    /// <summary>
    /// Finds a system by its identifier.
    /// </summary>
    /// <param name="id">The system identifier.</param>
    /// <typeparam name="TSystem">The system type.</typeparam>
    /// <returns>The system if found.</returns>
    TSystem? FindSystem<TSystem>(Guid id) where TSystem : class, IGameSystem;

    /// <summary>
    /// Gets the specified system if it exists, otherwise it adds it to the scene.
    /// </summary>
    /// <typeparam name="T">The type of system to get or add.</typeparam>
    /// <returns>The system.</returns>
    T GetOrAddSystem<T>() where T : class, IGameSystem, new();

    /// <summary>
    /// Gets the first found system of the specified type.
    /// </summary>
    /// <typeparam name="T">The type of system.</typeparam>
    /// <returns>The system.</returns>
    T? GetSystem<T>() where T : class, IGameSystem;

    /// <summary>
    /// Initializes this instance.
    /// </summary>
    /// <param name="game">The game.</param>
    /// <param name="assetManager">The asset manager.</param>
    void Initialize(IGame game, IAssetManager assetManager);

    /// <summary>
    /// Inserts a system at the specified index.
    /// </summary>
    /// <param name="index">The index.</param>
    /// <param name="system">The system.</param>
    void InsertSystem(int index, IGameSystem system);

    /// <summary>
    /// Invokes the specified action after the current update
    /// </summary>
    /// <param name="action">The action.</param>
    void Invoke(Action action);

    /// <summary>
    /// Raises the <see cref="Activated" /> event.
    /// </summary>
    void RaiseActivated();

    /// <summary>
    /// Raises the <see cref="Deactivated" /> event.
    /// </summary>
    void RaiseDeactivated();

    /// <summary>
    /// Registers the entity with relevant services.
    /// </summary>
    /// <param name="entity">The entity.</param>
    void RegisterEntity(IEntity entity);

    /// <summary>
    /// Removes the system.
    /// </summary>
    /// <param name="system">The system.</param>
    /// <returns>A value indicating whether the system was removed.</returns>
    bool RemoveSystem(IGameSystem system);

    /// <summary>
    /// Renders the scene.
    /// </summary>
    /// <param name="frameTime">The frame time.</param>
    /// <param name="inputState">The input state.</param>
    public void Render(FrameTime frameTime, InputState inputState);

    /// <summary>
    /// Reorders systems so the specified system is moved to the specified index.
    /// </summary>
    /// <param name="system">The system.</param>
    /// <param name="newIndex">The new index.</param>
    void ReorderSystem(IGameSystem system, int newIndex);

    /// <summary>
    /// Tries to find the specified entity.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="entity">The entity if found.</param>
    /// <returns>A value indicating whether the entity was found.</returns>
    bool TryFindEntity(Guid id, [NotNullWhen(true)] out IEntity? entity);

    /// <summary>
    /// Tries to find the specified entity.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="entity">The entity if found.</param>
    /// <returns>A value indicating whether the entity was found.</returns>
    bool TryFindEntity<TEntity>(Guid id, [NotNullWhen(true)] out TEntity? entity) where TEntity : class, IEntity;

    /// <summary>
    /// Unregisters the entity from services.
    /// </summary>
    /// <param name="entity">The entity.</param>
    void UnregisterEntity(IEntity entity);
}

/// <summary>
/// A user-created combination of <see cref="IGameSystem" /> and <see cref="IEntity" />
/// which runs on a <see cref="IGame" />.
/// </summary>
public sealed class Scene : GridContainer, IScene {

    private readonly FilterCollection<IAnimatableEntity> _animatableEntities = new(
        a => a.ShouldAnimate,
        (a, handler) => a.ShouldAnimateChanged += handler,
        (a, handler) => a.ShouldAnimateChanged -= handler);

    private readonly FilterSortCollection<ICamera> _cameras = new(
        c => c.IsEnabled,
        (c, handler) => c.IsEnabledChanged += handler,
        (c, handler) => c.IsEnabledChanged -= handler,
        (c1, c2) => Comparer<int>.Default.Compare(c1.RenderOrder, c2.RenderOrder),
        (c, handler) => c.RenderOrderChanged += handler,
        (c, handler) => c.RenderOrderChanged -= handler);

    private readonly FilterSortCollection<IFixedUpdateableEntity> _fixedUpdateableEntities = new(
        u => u.ShouldUpdate,
        (u, handler) => u.ShouldUpdateChanged += handler,
        (u, handler) => u.ShouldUpdateChanged -= handler,
        (p1, p2) => Comparer<int>.Default.Compare(p1.UpdateOrder, p2.UpdateOrder),
        (u, handler) => u.UpdateOrderChanged += handler,
        (u, handler) => u.UpdateOrderChanged -= handler);

    private readonly Dictionary<Guid, IEntity> _idToEntitiesInScene = [];

    private readonly List<INameableCollection> _namedChildren = [];
    private readonly List<Action> _pendingActions = [];

    private readonly FilterSortCollection<IPhysicsBody> _physicsBodies = new(
        p => p.IsEnabled,
        (p, handler) => p.IsEnabledChanged += handler,
        (p, handler) => p.IsEnabledChanged -= handler,
        (p1, p2) => Comparer<int>.Default.Compare(p1.UpdateOrder, p2.UpdateOrder),
        (p, handler) => p.UpdateOrderChanged += handler,
        (p, handler) => p.UpdateOrderChanged -= handler);

    private readonly FilterCollection<IUpdateSystem> _postUpdateSystems = new(
        a => a.ShouldUpdate,
        (a, handler) => a.ShouldUpdateChanged += handler,
        (a, handler) => a.ShouldUpdateChanged -= handler);

    private readonly FilterCollection<IUpdateSystem> _preUpdateSystems = new(
        a => a.ShouldUpdate,
        (a, handler) => a.ShouldUpdateChanged += handler,
        (a, handler) => a.ShouldUpdateChanged -= handler);

    private readonly FilterCollection<IRenderableEntity> _renderableEntities = new(
        r => r.ShouldRender,
        (r, handler) => r.ShouldRenderChanged += handler,
        (r, handler) => r.ShouldRenderChanged -= handler);

    private readonly FilterCollection<IRenderSystem> _renderSystems = new(
        a => a.ShouldRender,
        (a, handler) => a.ShouldRenderChanged += handler,
        (a, handler) => a.ShouldRenderChanged -= handler);

    [DataMember]
    private readonly SystemCollection _systems = [];

    private readonly FilterSortCollection<IUpdateableEntity> _updateableEntities = new(
        u => u.ShouldUpdate,
        (u, handler) => u.ShouldUpdateChanged += handler,
        (u, handler) => u.ShouldUpdateChanged -= handler,
        (p1, p2) => Comparer<int>.Default.Compare(p1.UpdateOrder, p2.UpdateOrder),
        (u, handler) => u.UpdateOrderChanged += handler,
        (u, handler) => u.UpdateOrderChanged -= handler);

    private readonly FilterCollection<IUpdateSystem> _updateSystems = new(
        a => a.ShouldUpdate,
        (a, handler) => a.ShouldUpdateChanged += handler,
        (a, handler) => a.ShouldUpdateChanged -= handler);

    private IGame _game = BaseGame.Empty;
    private bool _isBusy;
    private bool _isInitialized;

    /// <inheritdoc />
    public event EventHandler? Activated;

    /// <inheritdoc />
    public event EventHandler? BoundingAreaChanged;

    /// <inheritdoc />
    public event EventHandler? Deactivated;

    /// <inheritdoc />
    public event EventHandler? ShouldUpdateChanged;

    /// <summary>
    /// Initializes a new instance of the <see cref="Scene" /> class.
    /// </summary>
    public Scene() : base() {
        this._namedChildren.Add(this._systems);
        if (this.Children is INameableCollection nameableCollection) {
            this._namedChildren.Add(nameableCollection);
        }
    }

    /// <inheritdoc />
    public IReadOnlyCollection<IAnimatableEntity> AnimatableEntities => this._animatableEntities;

    /// <inheritdoc />
    public IAssetManager Assets { get; private set; } = AssetManager.Empty;

    /// <inheritdoc />
    [DataMember]
    public Color BackgroundColor { get; set; } = Color.Black;

    /// <inheritdoc />
    [DataMember]
    public BoundingArea BoundingArea {
        get;
        set {
            field = value;
            if (this.IsInitialized) {
                this.BoundingAreaChanged.SafeInvoke(this);
            }
        }
    } = BoundingArea.Empty;

    /// <inheritdoc />
    public IReadOnlyCollection<ICamera> Cameras => this._cameras;

    /// <inheritdoc />
    public IReadOnlyCollection<IFixedUpdateableEntity> FixedUpdateableEntities => this._fixedUpdateableEntities;

    /// <inheritdoc cref="IScene" />
    public override IGame Game => this._game;

    /// <inheritdoc />
    public bool IsActive { get; private set; }

    /// <inheritdoc />
    public IReadOnlyCollection<INameableCollection> NamedChildren => this._namedChildren;

    /// <inheritdoc />
    public IReadOnlyCollection<IPhysicsBody> PhysicsBodies => this._physicsBodies;

    /// <inheritdoc />
    public IReadOnlyCollection<IRenderableEntity> RenderableEntities => this._renderableEntities;

    /// <inheritdoc />
    public bool ShouldUpdate => true;

    /// <inheritdoc />
    [DataMember]
    public SceneState State { get; } = new();

    /// <inheritdoc />
    public IReadOnlyCollection<IGameSystem> Systems => this._systems;

    /// <inheritdoc />
    public IReadOnlyCollection<IUpdateableEntity> UpdateableEntities => this._updateableEntities;

    /// <inheritdoc />
    public T AddSystem<T>() where T : IGameSystem, new() {
        var system = new T();
        this.AddSystem(system);
        return system;
    }

    /// <inheritdoc />
    public void AddSystem(IGameSystem system) {
        this._systems.Add(system);
        this.OnSystemAdded(system);
    }

    /// <inheritdoc />
    public bool ContainsEntity(Guid id) => this._idToEntitiesInScene.ContainsKey(id);

    /// <inheritdoc />
    public override void Deinitialize() {
        if (this._isInitialized) {
            try {
                base.Deinitialize();

                foreach (var system in this._systems) {
                    system.Deinitialize();
                }

                this.ClearFilterCaches();
            }
            finally {
                this._isInitialized = false;
            }
        }
    }

    /// <inheritdoc />
    public TEntity? FindEntity<TEntity>(Guid id) where TEntity : class, IEntity {
        if (this is TEntity entity && entity.Id == id) {
            return entity;
        }

        return this.FindChild(id) as TEntity;
    }

    /// <inheritdoc />
    public TSystem? FindSystem<TSystem>(Guid id) where TSystem : class, IGameSystem {
        return this.Systems.OfType<TSystem>().FirstOrDefault(x => x.Id == id);
    }

    /// <inheritdoc />
    public T GetOrAddSystem<T>() where T : class, IGameSystem, new() => this.GetSystem<T>() ?? this.AddSystem<T>();

    /// <inheritdoc />
    public T? GetSystem<T>() where T : class, IGameSystem => this.Systems.OfType<T>().FirstOrDefault();

    /// <inheritdoc />
    public void Initialize(IGame game, IAssetManager assetManager) {
        if (!this._isInitialized) {
            try {
                this._isBusy = true;
                this.IsActive = true;
                this.Assets = assetManager;
                this._game = game;

                this.Project.Initialize(this.Assets, this.Game);

                this._idToEntitiesInScene.Clear();
                this._idToEntitiesInScene[this.Id] = this;
                foreach (var child in this.GetAllDescendants()) {
                    this._idToEntitiesInScene[child.Id] = child;
                }

                foreach (var system in this.Systems) {
                    system.Initialize(this);
                    this.RegisterSystem(system);
                }

                this.State.Initialize(this._game.State);
                this.LoadAssets(this.Assets, this.Game);
                this.Initialize(this, this);
                this.RebuildFilterCaches();
                this.OnSceneTreeLoaded();
            }
            finally {
                this._isInitialized = true;
                this._isBusy = false;
            }

            this.InvokePendingActions();
        }
        else {
            foreach (var system in this.Systems) {
                this.OnSystemAdded(system);
            }
        }
    }

    /// <inheritdoc />
    public void InsertSystem(int index, IGameSystem system) {
        this._systems.InsertOrAdd(index, system);
        this.OnSystemAdded(system);
    }

    /// <inheritdoc />
    public void Invoke(Action action) {
        if (this._isBusy) {
            this._pendingActions.Add(action);
        }
        else {
            action();
        }
    }

    /// <summary>
    /// Determines whether the specified scene is null or <see cref="Scene.Empty" />.
    /// </summary>
    /// <param name="scene">The scene.</param>
    /// <returns>
    /// <c>true</c> if the specified scene is null or <see cref="Scene.Empty" />; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsNullOrEmpty(IScene? scene) => scene == null || scene == Empty;

    /// <summary>
    /// Determines whether the specified scene is null or <see cref="Scene.Empty" />.
    /// </summary>
    /// <param name="scene">The scene.</param>
    /// <param name="nonNullScene">The non-null scene.</param>
    /// <returns>
    /// <c>true</c> if the specified scene is null or <see cref="Scene.Empty" />; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsNullOrEmpty(IScene? scene, [NotNullWhen(false)] out IScene? nonNullScene) {
        if (scene == null || scene == Empty) {
            nonNullScene = null;
            return true;
        }

        nonNullScene = scene;
        return false;
    }

    /// <inheritdoc />
    public override void OnSceneTreeLoaded() {
        foreach (var system in this.Systems) {
            system.OnSceneTreeLoaded();
        }

        base.OnSceneTreeLoaded();
    }

    /// <inheritdoc />
    public void RaiseActivated() {
        this.IsActive = true;
        this.Activated?.SafeInvoke(this);
    }

    /// <inheritdoc />
    public void RaiseDeactivated() {
        this.IsActive = false;
        this.Deactivated?.SafeInvoke(this);
    }

    /// <inheritdoc />
    public void RegisterEntity(IEntity entity) {
        this._animatableEntities.Add(entity);
        this._cameras.Add(entity);
        this._physicsBodies.Add(entity);
        this._renderableEntities.Add(entity);
        this._updateableEntities.Add(entity);
        this._fixedUpdateableEntities.Add(entity);
        this._idToEntitiesInScene[entity.Id] = entity;

        if (this._isInitialized) {
            if (BaseGame.IsDesignMode) {
                this.RebuildFilterCaches();
            }
            else {
                this.Scene.Invoke(this.RebuildFilterCaches);
            }
        }
    }

    /// <inheritdoc />
    public bool RemoveSystem(IGameSystem system) {
        var result = false;
        if (this._systems.Contains(system)) {
            this.Invoke(() =>
            {
                this._systems.Remove(system);
                this.OnSystemRemoved(system);
            });
            result = true;
        }

        return result;
    }

    /// <inheritdoc />
    public void Render(FrameTime frameTime, InputState inputState) {
        try {
            this._isBusy = true;
            this._renderSystems.RebuildCache();

            foreach (var system in this._renderSystems) {
                system.Render(frameTime);
            }
        }
        finally {
            this._isBusy = false;
        }
    }

    /// <inheritdoc />
    public void ReorderSystem(IGameSystem system, int newIndex) {
        this._systems.Reorder(system, newIndex);
    }

    /// <inheritdoc />
    public bool TryFindEntity(Guid id, [NotNullWhen(true)] out IEntity? entity) => this._idToEntitiesInScene.TryGetValue(id, out entity);

    /// <inheritdoc />
    public bool TryFindEntity<TEntity>(Guid id, [NotNullWhen(true)] out TEntity? entity) where TEntity : class, IEntity {
        if (this._idToEntitiesInScene.TryGetValue(id, out var untypedEntity)) {
            entity = untypedEntity as TEntity;
        }
        else {
            entity = null;
        }

        return entity != null;
    }

    /// <inheritdoc />
    public override bool TryGetAncestor<T>([NotNullWhen(true)] out T? entity) where T : default {
        entity = default;
        return false;
    }

    /// <inheritdoc />
    public void UnregisterEntity(IEntity entity) {
        this._animatableEntities.Remove(entity);
        this._cameras.Remove(entity);
        this._physicsBodies.Remove(entity);
        this._renderableEntities.Remove(entity);
        this._updateableEntities.Remove(entity);
        this._fixedUpdateableEntities.Remove(entity);
        this._idToEntitiesInScene.Remove(entity.Id);
    }

    /// <inheritdoc />
    public void Update(FrameTime frameTime, InputState inputState) {
        try {
            this._isBusy = true;
            this.RebuildFilterCaches();
            this.InvokePendingActions();

            foreach (var system in this._preUpdateSystems) {
                system.Update(frameTime, inputState);
            }

            foreach (var system in this._updateSystems) {
                system.Update(frameTime, inputState);
            }

            foreach (var system in this._postUpdateSystems) {
                system.Update(frameTime, inputState);
            }
        }
        finally {
            this._isBusy = false;
        }
    }

    /// <inheritdoc />
    protected override void OnDisposing() {
        base.OnDisposing();
        this.Assets.Dispose();
    }

    private void ClearFilterCaches() {
        this._animatableEntities.Clear();
        this._cameras.Clear();
        this._physicsBodies.Clear();
        this._renderableEntities.Clear();
        this._updateableEntities.Clear();
        this._fixedUpdateableEntities.Clear();
        this._idToEntitiesInScene.Clear();

        this._renderSystems.Clear();
        this._updateSystems.Clear();
        this._preUpdateSystems.Clear();
        this._postUpdateSystems.Clear();
    }

    private void InvokePendingActions() {
        var actions = this._pendingActions.ToList();
        foreach (var action in actions) {
            action();
            this._pendingActions.Remove(action);
        }
    }

    private void OnSystemAdded(IGameSystem system) {
        if (this._isInitialized) {
            system.Initialize(this);
            system.OnSceneTreeLoaded();
        }
    }

    private void OnSystemRemoved(IGameSystem system) {
        if (this._isInitialized) {
            this.UnregisterSystem(system);
            system.Deinitialize();
        }
    }

    private void RebuildFilterCaches() {
        this._animatableEntities.RebuildCache();
        this._cameras.RebuildCache();
        this._physicsBodies.RebuildCache();
        this._renderableEntities.RebuildCache();
        this._updateableEntities.RebuildCache();
        this._fixedUpdateableEntities.RebuildCache();

        this._updateSystems.RebuildCache();
        this._preUpdateSystems.RebuildCache();
        this._postUpdateSystems.RebuildCache();
    }

    private void RegisterSystem(IGameSystem system) {
        this._renderSystems.Add(system);

        if (system is IUpdateSystem updateSystem) {
            switch (updateSystem.Kind) {
                case UpdateSystemKind.Update:
                    this._updateSystems.Add(updateSystem);
                    break;
                case UpdateSystemKind.PreUpdate:
                    this._preUpdateSystems.Add(updateSystem);
                    break;
                case UpdateSystemKind.PostUpdate:
                    this._postUpdateSystems.Add(updateSystem);
                    break;
                case UpdateSystemKind.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    private void UnregisterSystem(IGameSystem system) {
        this._renderSystems.Remove(system);
        this._updateSystems.Remove(system);
        this._preUpdateSystems.Remove(system);
        this._postUpdateSystems.Remove(system);
    }
}