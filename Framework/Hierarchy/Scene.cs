namespace Macabre2D.Framework;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.Serialization;
using Macabre2D.Project.Common;
using Macabresoft.Core;
using Microsoft.Xna.Framework;

/// <summary>
/// Interface for a combination of <see cref="ISceneSystem" /> and <see cref="IEntity" />
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
    /// Gets the scene initialization step.
    /// </summary>
    public SceneInitializationStep InitializationStep { get; }

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
    /// Gets the screen space renderers.
    /// </summary>
    IReadOnlyCollection<IScreenSpaceRenderer> ScreenSpaceRenderers => [];

    /// <summary>
    /// Gets the scene state.
    /// </summary>
    SceneState State { get; }

    /// <summary>
    /// Gets the systems.
    /// </summary>
    IReadOnlyCollection<ISceneSystem> Systems => [];

    /// <summary>
    /// Gets the updateable entities.
    /// </summary>
    IReadOnlyCollection<IUpdateableEntity> UpdateableEntities => [];

    /// <summary>
    /// Adds the system.
    /// </summary>
    /// <typeparam name="T">
    /// A type that implements <see cref="ISceneSystem" /> and has an empty constructor.
    /// </typeparam>
    /// <returns>The added system.</returns>
    T AddSystem<T>() where T : ISceneSystem, new();

    /// <summary>
    /// Adds the system.
    /// </summary>
    /// <param name="system">The system.</param>
    void AddSystem(ISceneSystem system);

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
    TSystem? FindSystem<TSystem>(Guid id) where TSystem : class, ISceneSystem;

    /// <summary>
    /// Gets the specified system if it exists, otherwise it adds it to the scene.
    /// </summary>
    /// <typeparam name="T">The type of system to get or add.</typeparam>
    /// <returns>The system.</returns>
    T GetOrAddSystem<T>() where T : class, ISceneSystem, new();

    /// <summary>
    /// Gets the first found system of the specified type.
    /// </summary>
    /// <typeparam name="T">The type of system.</typeparam>
    /// <returns>The system.</returns>
    T? GetSystem<T>() where T : class, ISceneSystem;

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
    void InsertSystem(int index, ISceneSystem system);

    /// <summary>
    /// Inserts the <see cref="systemToInsert" /> after the <see cref="precedingSystem" />.
    /// </summary>
    /// <param name="systemToInsert">The system to insert.</param>
    /// <param name="precedingSystem">The preceding system.</param>
    void InsertSystemAfter(ISceneSystem systemToInsert, ISceneSystem precedingSystem);

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
    bool RemoveSystem(ISceneSystem system);

    /// <summary>
    /// Renders the scene.
    /// </summary>
    /// <param name="frameTime">The frame time.</param>
    /// <param name="inputState">The input state.</param>
    public void Render(FrameTime frameTime, InputState inputState);

    /// <summary>
    /// Renders the scene in screen space.
    /// </summary>
    /// <param name="frameTime">The frame time.</param>
    /// <param name="inputState">The input state.</param>
    public void RenderInScreenSpace(FrameTime frameTime, InputState inputState);

    /// <summary>
    /// Reorders systems so the specified system is moved to the specified index.
    /// </summary>
    /// <param name="system">The system.</param>
    /// <param name="newIndex">The new index.</param>
    void ReorderSystem(ISceneSystem system, int newIndex);

    /// <summary>
    /// Begins updating again.
    /// </summary>
    void StartUpdates();

    /// <summary>
    /// Stops updates on this scene until turned back on.
    /// </summary>
    void StopUpdates();

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
/// A user-created combination of <see cref="ISceneSystem" /> and <see cref="IEntity" />
/// which runs on a <see cref="IGame" />.
/// </summary>
public sealed class Scene : GridContainer, IScene {

    // ReSharper disable once CollectionNeverUpdated.Local
    private readonly FilterCollection<IAnimatableEntity> _animatableEntities = new(
        a => a.ShouldAnimate,
        (a, handler) => a.ShouldAnimateChanged += handler,
        (a, handler) => a.ShouldAnimateChanged -= handler);

    // ReSharper disable once CollectionNeverUpdated.Local
    private readonly FilterSortCollection<ICamera> _cameras = new(
        c => c.IsEnabled,
        (c, handler) => c.IsEnabledChanged += handler,
        (c, handler) => c.IsEnabledChanged -= handler,
        (c1, c2) => Comparer<int>.Default.Compare(c1.RenderOrder, c2.RenderOrder),
        (c, handler) => c.RenderOrderChanged += handler,
        (c, handler) => c.RenderOrderChanged -= handler);

    // ReSharper disable once CollectionNeverUpdated.Local
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

    // ReSharper disable once CollectionNeverUpdated.Local
    private readonly FilterSortCollection<IPhysicsBody> _physicsBodies = new(
        p => p.IsEnabled,
        (p, handler) => p.IsEnabledChanged += handler,
        (p, handler) => p.IsEnabledChanged -= handler,
        (p1, p2) => Comparer<int>.Default.Compare(p1.UpdateOrder, p2.UpdateOrder),
        (p, handler) => p.UpdateOrderChanged += handler,
        (p, handler) => p.UpdateOrderChanged -= handler);

    private readonly FilterCollection<ISceneUpdateSystem> _postUpdateSystems = new(
        a => a.ShouldUpdate,
        (a, handler) => a.ShouldUpdateChanged += handler,
        (a, handler) => a.ShouldUpdateChanged -= handler);

    private readonly FilterCollection<ISceneUpdateSystem> _preUpdateSystems = new(
        a => a.ShouldUpdate,
        (a, handler) => a.ShouldUpdateChanged += handler,
        (a, handler) => a.ShouldUpdateChanged -= handler);

    // ReSharper disable once CollectionNeverUpdated.Local
    private readonly FilterCollection<IRenderableEntity> _renderableEntities = new(
        r => r.ShouldRender,
        (r, handler) => r.ShouldRenderChanged += handler,
        (r, handler) => r.ShouldRenderChanged -= handler);

    private readonly FilterCollection<IRenderSystem> _renderSystems = new(
        a => a.ShouldRender,
        (a, handler) => a.ShouldRenderChanged += handler,
        (a, handler) => a.ShouldRenderChanged -= handler);

    // ReSharper disable once CollectionNeverUpdated.Local
    private readonly FilterCollection<IScreenSpaceRenderer> _screenSpaceRenderers = new(
        r => r.ShouldRenderInScreenSpace,
        (r, handler) => r.ShouldRenderInScreenSpaceChanged += handler,
        (r, handler) => r.ShouldRenderInScreenSpaceChanged -= handler);


    private readonly FilterCollection<IScreenSpaceRenderSystem> _screenSpaceRenderSystems = new(
        a => a.ShouldRenderInScreenSpace,
        (a, handler) => a.ShouldRenderInScreenSpaceChanged += handler,
        (a, handler) => a.ShouldRenderInScreenSpaceChanged -= handler);

    private readonly HashSet<SceneSystemContainer> _systemContainers = [];

    [DataMember]
    private readonly SceneSystemCollection _systems = [];

    // ReSharper disable once CollectionNeverUpdated.Local
    private readonly FilterSortCollection<IUpdateableEntity> _updateableEntities = new(
        u => u.ShouldUpdate,
        (u, handler) => u.ShouldUpdateChanged += handler,
        (u, handler) => u.ShouldUpdateChanged -= handler,
        (p1, p2) => Comparer<int>.Default.Compare(p1.UpdateOrder, p2.UpdateOrder),
        (u, handler) => u.UpdateOrderChanged += handler,
        (u, handler) => u.UpdateOrderChanged -= handler);

    private readonly FilterCollection<ISceneUpdateSystem> _updateSystems = new(
        a => a.ShouldUpdate,
        (a, handler) => a.ShouldUpdateChanged += handler,
        (a, handler) => a.ShouldUpdateChanged -= handler);

    private IGame _game = BaseGame.Empty;
    private bool _isBusy;

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
    public SceneInitializationStep InitializationStep { get; private set; }

    /// <inheritdoc />
    public bool IsActive { get; private set; }

    /// <inheritdoc />
    public IReadOnlyCollection<INameableCollection> NamedChildren => this._namedChildren;

    /// <inheritdoc />
    public IReadOnlyCollection<IPhysicsBody> PhysicsBodies => this._physicsBodies;

    /// <inheritdoc />
    public IReadOnlyCollection<IRenderableEntity> RenderableEntities => this._renderableEntities;

    /// <inheritdoc />
    public IReadOnlyCollection<IScreenSpaceRenderer> ScreenSpaceRenderers => this._screenSpaceRenderers;

    /// <inheritdoc />
    public bool ShouldUpdate {
        get;
        private set {
            if (value != field) {
                field = value;
                this.ShouldUpdateChanged.SafeInvoke(this);
            }
        }
    } = true;

    /// <inheritdoc />
    [DataMember]
    public SceneState State { get; } = new();

    /// <inheritdoc />
    public IReadOnlyCollection<ISceneSystem> Systems => this._systems;

    /// <inheritdoc />
    public IReadOnlyCollection<IUpdateableEntity> UpdateableEntities => this._updateableEntities;

    /// <inheritdoc />
    public T AddSystem<T>() where T : ISceneSystem, new() {
        var system = new T();
        this.AddSystem(system);
        return system;
    }

    /// <inheritdoc />
    public void AddSystem(ISceneSystem system) {
        this._systems.Add(system);
        this.OnSystemAdded(system);
    }

    /// <inheritdoc />
    public bool ContainsEntity(Guid id) => this._idToEntitiesInScene.ContainsKey(id);

    /// <inheritdoc />
    public override void Deinitialize() {
        if (this.InitializationStep != SceneInitializationStep.NotStarted) {
            try {
                base.Deinitialize();

                foreach (var system in this._systems) {
                    system.Deinitialize();
                }

                this.ClearFilterCaches();
                this.State.Deinitialize();
            }
            finally {
                this.InitializationStep = SceneInitializationStep.NotStarted;
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
    public TSystem? FindSystem<TSystem>(Guid id) where TSystem : class, ISceneSystem {
        return this.Systems.OfType<TSystem>().FirstOrDefault(x => x.Id == id);
    }

    /// <inheritdoc />
    public T GetOrAddSystem<T>() where T : class, ISceneSystem, new() => this.GetSystem<T>() ?? this.AddSystem<T>();

    /// <inheritdoc />
    public T? GetSystem<T>() where T : class, ISceneSystem {
        if (this.Systems.OfType<T>().FirstOrDefault() is { } foundSystem) {
            return foundSystem;
        }

        return this._systemContainers.Select(x => x.System).OfType<T>().FirstOrDefault();
    }

    /// <inheritdoc />
    public void Initialize(IGame game, IAssetManager assetManager) {
        if (this.InitializationStep == SceneInitializationStep.NotStarted) {
            try {
                this._isBusy = true;
                this.IsActive = true;
                this.Assets = assetManager;
                this._game = game;

                this.InitializationStep = SceneInitializationStep.InitializeProject;
                this.Project.Initialize(this.Assets, this.Game);

                this._idToEntitiesInScene.Clear();
                this._idToEntitiesInScene[this.Id] = this;
                foreach (var child in this.GetAllDescendants()) {
                    this._idToEntitiesInScene[child.Id] = child;
                }

                // Reason for two loops: Load ALL assets before beginning initialization.
                this.InitializationStep = SceneInitializationStep.LoadSystemAssets;
                foreach (var system in this.Systems.ToList()) {
                    system.LoadAssets(this.Assets, this.Game);
                }

                this.InitializationStep = SceneInitializationStep.InitializeSystems;
                foreach (var system in this.Systems.ToList()) {
                    system.Initialize(this);
                    this.RegisterSystem(system);
                }

                this.InitializationStep = SceneInitializationStep.InitializeState;
                this.State.Initialize(this._game.State);

                this.InitializationStep = SceneInitializationStep.LoadEntityAssets;
                this.LoadAssets(this.Assets, this.Game);

                this.InitializationStep = SceneInitializationStep.InitializeEntities;
                this.Initialize(this, this);
                this.RebuildFilterCaches();

                this.InitializationStep = SceneInitializationStep.LoadSceneTree;
                this.OnSceneTreeLoaded();
                this.InvokePendingActions();
            }
            finally {
                this.InitializationStep = SceneInitializationStep.Done;
                this._isBusy = false;
            }
        }
        else {
            foreach (var system in this.Systems) {
                this.OnSystemAdded(system);
            }
        }
    }

    /// <inheritdoc />
    public void InsertSystem(int index, ISceneSystem system) {
        this._systems.InsertOrAdd(index, system);
        this.OnSystemAdded(system);
    }

    /// <inheritdoc />
    public void InsertSystemAfter(ISceneSystem systemToInsert, ISceneSystem precedingSystem) {
        var index = this._systems.IndexOf(precedingSystem) + 1;
        this.InsertSystem(index, systemToInsert);
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
        foreach (var system in this.Systems.ToList()) {
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
        foreach (var cache in this.GetEntityCaches()) {
            cache.Add(entity);
        }

        this._idToEntitiesInScene[entity.Id] = entity;

        if (this.InitializationStep == SceneInitializationStep.Done) {
            if (BaseGame.IsDesignMode) {
                this.RebuildEntityCaches();
            }
            else {
                this.Scene.Invoke(this.RebuildEntityCaches);
            }
        }
    }

    /// <inheritdoc />
    public bool RemoveSystem(ISceneSystem system) {
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
    public void RenderInScreenSpace(FrameTime frameTime, InputState inputState) {
        try {
            this._isBusy = true;
            this._screenSpaceRenderers.RebuildCache();
            this._screenSpaceRenderSystems.RebuildCache();

            foreach (var system in this._screenSpaceRenderSystems) {
                system.RenderInScreenSpace(frameTime);
            }
        }
        finally {
            this._isBusy = false;
        }
    }

    /// <inheritdoc />
    public void ReorderSystem(ISceneSystem system, int newIndex) {
        this._systems.Reorder(system, newIndex);
    }

    /// <inheritdoc />
    public void StartUpdates() {
        this.ShouldUpdate = true;
    }

    /// <inheritdoc />
    public void StopUpdates() {
        this.ShouldUpdate = false;
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
        foreach (var cache in this.GetEntityCaches()) {
            cache.Remove(entity);
        }

        this._idToEntitiesInScene.Remove(entity.Id);
    }

    /// <inheritdoc />
    public void Update(FrameTime frameTime, InputState inputState) {
        try {
            this._isBusy = true;
            this.RebuildFilterCaches();
            this.InvokePendingActions();

            if (this.ShouldUpdate) {
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
        foreach (var cache in this.GetEntityCaches()) {
            cache.Clear();
        }

        this._idToEntitiesInScene.Clear();

        foreach (var cache in this.GetSystemCaches()) {
            cache.Clear();
        }
    }

    private IEnumerable<IFilterCollection> GetEntityCaches() {
        yield return this._animatableEntities;
        yield return this._cameras;
        yield return this._screenSpaceRenderers;
        yield return this._physicsBodies;
        yield return this._renderableEntities;
        yield return this._updateableEntities;
        yield return this._fixedUpdateableEntities;
    }

    private IEnumerable<IFilterCollection> GetSystemCaches() {
        yield return this._screenSpaceRenderSystems;
        yield return this._renderSystems;
        yield return this._updateSystems;
        yield return this._preUpdateSystems;
        yield return this._postUpdateSystems;
    }

    private void InvokePendingActions() {
        var actions = this._pendingActions.ToList();
        foreach (var action in actions) {
            action();
            this._pendingActions.Remove(action);
        }
    }

    private void OnSystemAdded(ISceneSystem system) {
        if (this.InitializationStep >= SceneInitializationStep.LoadSystemAssets) {
            system.LoadAssets(this.Assets, this.Game);
        }

        if (this.InitializationStep >= SceneInitializationStep.InitializeSystems) {
            system.Initialize(this);
            this.RegisterSystem(system);
        }

        if (this.InitializationStep >= SceneInitializationStep.LoadSceneTree) {
            system.OnSceneTreeLoaded();
        }
    }

    private void OnSystemRemoved(ISceneSystem system) {
        if (this.InitializationStep >= SceneInitializationStep.InitializeSystems) {
            this.UnregisterSystem(system);
            system.Deinitialize();
        }
    }

    private void RebuildEntityCaches() {
        foreach (var cache in this.GetEntityCaches()) {
            cache.RebuildCache();
        }
    }

    private void RebuildFilterCaches() {
        this.RebuildEntityCaches();

        foreach (var system in this.GetSystemCaches()) {
            system.RebuildCache();
        }
    }

    private void RegisterSystem(ISceneSystem system) {
        this._screenSpaceRenderSystems.Add(system);
        this._renderSystems.Add(system);

        if (system is SceneSystemContainer container) {
            this._systemContainers.Add(container);
        }

        if (system is ISceneUpdateSystem updateSystem) {
            switch (updateSystem.Kind) {
                case SceneUpdateSystemKind.Update:
                    this._updateSystems.Add(updateSystem);
                    break;
                case SceneUpdateSystemKind.PreUpdate:
                    this._preUpdateSystems.Add(updateSystem);
                    break;
                case SceneUpdateSystemKind.PostUpdate:
                    this._postUpdateSystems.Add(updateSystem);
                    break;
                case SceneUpdateSystemKind.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    private void UnregisterSystem(ISceneSystem system) {
        foreach (var cache in this.GetSystemCaches()) {
            cache.Remove(system);
        }

        if (system is SceneSystemContainer container) {
            this._systemContainers.Remove(container);
        }
    }
}