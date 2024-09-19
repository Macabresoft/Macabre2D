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
public interface IScene : IUpdateableGameObject, IGridContainer, IBoundable {
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
    /// Gets thje animatable entities.
    /// </summary>
    IReadOnlyCollection<IAnimatableEntity> AnimatableEntities => Array.Empty<IAnimatableEntity>();

    /// <summary>
    /// Gets the asset manager.
    /// </summary>
    IAssetManager Assets => AssetManager.Empty;

    /// <summary>
    /// Gets the cameras in the scene.
    /// </summary>
    IReadOnlyCollection<ICamera> Cameras => Array.Empty<ICamera>();

    /// <summary>
    /// Gets the fixed updateable entities.
    /// </summary>
    IReadOnlyCollection<IFixedUpdateableEntity> FixedUpdateableEntities => Array.Empty<IFixedUpdateableEntity>();

    /// <summary>
    /// Gets the game currently running this scene.
    /// </summary>
    IGame Game => BaseGame.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether or not this is active.
    /// </summary>
    bool IsActive { get; }

    /// <summary>
    /// Gets the named children.
    /// </summary>
    IReadOnlyCollection<INameableCollection> NamedChildren => Array.Empty<INameableCollection>();

    /// <summary>
    /// Gets the physics bodies.
    /// </summary>
    IReadOnlyCollection<IPhysicsBody> PhysicsBodies => Array.Empty<IPhysicsBody>();

    /// <summary>
    /// Gets the renderable entities in the scene.
    /// </summary>
    IReadOnlyCollection<IRenderableEntity> RenderableEntities => Array.Empty<IRenderableEntity>();

    /// <summary>
    /// Gets the scene state.
    /// </summary>
    SceneState State { get; }

    /// <summary>
    /// Gets the systems.
    /// </summary>
    IReadOnlyCollection<IGameSystem> Systems => Array.Empty<IGameSystem>();

    /// <summary>
    /// Gets the updateable entities.
    /// </summary>
    IReadOnlyCollection<IUpdateableEntity> UpdateableEntities => Array.Empty<IUpdateableEntity>();

    /// <summary>
    /// Gets or sets the color of the background.
    /// </summary>
    Color BackgroundColor { get; set; }

    /// <summary>
    /// Gets or sets the version of this scene.
    /// </summary>
    Version Version { get; set; }

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
    /// Resolves the dependency.
    /// </summary>
    /// <typeparam name="T">The type of the dependency.</typeparam>
    /// <returns>The dependency if it already exists or a newly created dependency.</returns>
    T ResolveDependency<T>() where T : new();

    /// <summary>
    /// Resolves the dependency.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="objectFactory">The object factory.</param>
    /// <returns>
    /// The dependency if it already exists or a dependency created from the provided factory.
    /// </returns>
    T ResolveDependency<T>(Func<T> objectFactory) where T : class;

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
        c => c.IsEnabled,
        nameof(IAnimatableEntity.IsEnabled));

    private readonly FilterSortCollection<ICamera> _cameras = new(
        c => c.IsEnabled,
        nameof(IEnableable.IsEnabled),
        (c1, c2) => Comparer<int>.Default.Compare(c1.RenderOrder, c2.RenderOrder),
        nameof(ICamera.RenderOrder));

    private readonly Dictionary<Type, object> _dependencies = new();

    private readonly FilterSortCollection<IFixedUpdateableEntity> _fixedUpdateableEntities = new(
        c => c.ShouldUpdate,
        nameof(IFixedUpdateableEntity.ShouldUpdate),
        (c1, c2) => Comparer<int>.Default.Compare(c1.UpdateOrder, c2.UpdateOrder),
        nameof(IFixedUpdateableEntity.UpdateOrder));

    private readonly List<INameableCollection> _namedChildren = new();
    private readonly List<Action> _pendingActions = new();

    private readonly FilterSortCollection<IPhysicsBody> _physicsBodies = new(
        r => r.IsEnabled,
        nameof(IEnableable.IsEnabled),
        (r1, r2) => Comparer<int>.Default.Compare(r1.UpdateOrder, r2.UpdateOrder),
        nameof(IPhysicsBody.UpdateOrder));

    private readonly HashSet<Guid> _registeredEntities = new();

    private readonly FilterCollection<IRenderableEntity> _renderableEntities = new(
        c => c.ShouldRender,
        nameof(IRenderableEntity.ShouldRender));

    [DataMember]
    private readonly SystemCollection _systems = new();

    private readonly FilterSortCollection<IUpdateableEntity> _updateableEntities = new(
        c => c.ShouldUpdate,
        nameof(IUpdateableEntity.ShouldUpdate),
        (c1, c2) => Comparer<int>.Default.Compare(c1.UpdateOrder, c2.UpdateOrder),
        nameof(IUpdateableEntity.UpdateOrder));

    private BoundingArea _boundingArea = BoundingArea.Empty;
    private IGame _game = BaseGame.Empty;
    private bool _isBusy;
    private bool _isInitialized;

    /// <inheritdoc />
    public event EventHandler? Activated;

    /// <inheritdoc />
    public event EventHandler? BoundingAreaChanged;

    /// <inheritdoc />
    public event EventHandler? Deactivated;

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
    public IReadOnlyCollection<ICamera> Cameras => this._cameras;

    /// <summary>
    /// Gets the default empty <see cref="IScene" /> that is present before initialization.
    /// </summary>
    public new static IScene Empty => EmptyObject.Instance;

    /// <inheritdoc />
    public IReadOnlyCollection<IFixedUpdateableEntity> FixedUpdateableEntities => this._fixedUpdateableEntities;

    /// <inheritdoc cref="IScene" />
    public override IGame Game => this._game;

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
    public IAssetManager Assets { get; private set; } = AssetManager.Empty;

    /// <inheritdoc />
    [DataMember]
    public Color BackgroundColor { get; set; } = Color.Black;

    /// <inheritdoc />
    [DataMember]
    public BoundingArea BoundingArea {
        get => this._boundingArea;
        set {
            this._boundingArea = value;
            if (this.IsInitialized) {
                this.BoundingAreaChanged.SafeInvoke(this);
            }
        }
    }

    /// <inheritdoc />
    public bool IsActive { get; private set; }

    /// <inheritdoc />
    [DataMember]
    public Version Version { get; set; } = new(0, 0, 0, 0);

    /// <inheritdoc />
    public T AddSystem<T>() where T : IGameSystem, new() {
        var system = new T();
        this.AddSystem(system);
        return system;
    }

    /// <inheritdoc />
    public void AddSystem(IGameSystem system) {
        this._systems.Add(system);

        if (this._isInitialized) {
            system.Initialize(this);
        }
    }

    /// <inheritdoc />
    public bool ContainsEntity(Guid id) => this._registeredEntities.Contains(id);

    public override void Deinitialize() {
        base.Deinitialize();

        foreach (var system in this._systems) {
            system.Deinitialize();
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

                foreach (var system in this.Systems) {
                    system.Initialize(this);
                }

                this.State.Initialize(this._game.State);
                this.LoadAssets(this.Assets, this.Game);
                this.Initialize(this, this);
                this.RebuildFilterCaches();
            }
            finally {
                this._isInitialized = true;
                this._isBusy = false;
            }

            this.InvokePendingActions();
        }
    }

    /// <inheritdoc />
    public void InsertSystem(int index, IGameSystem system) {
        this._systems.InsertOrAdd(index, system);

        if (this._isInitialized) {
            system.Initialize(this);
        }
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
        this._registeredEntities.Add(entity.Id);

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
                system.Deinitialize();
            });
            result = true;
        }

        return result;
    }

    /// <inheritdoc />
    public void Render(FrameTime frameTime, InputState inputState) {
        try {
            this._isBusy = true;

            foreach (var system in this.Systems.Where(x => x is { ShouldUpdate: true, Kind: GameSystemKind.Render })) {
                system.Update(frameTime, inputState);
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
    public T ResolveDependency<T>() where T : new() {
        if (this._dependencies.TryGetValue(typeof(T), out var dependency)) {
            return (T)dependency;
        }

        dependency = new T();
        this._dependencies.Add(typeof(T), dependency);
        return (T)dependency;
    }

    /// <inheritdoc />
    public T ResolveDependency<T>(Func<T> objectFactory) where T : class {
        if (this._dependencies.TryGetValue(typeof(T), out var found) && found is T dependency) {
            return dependency;
        }

        dependency = objectFactory.SafeInvoke();
        this._dependencies.Add(typeof(T), dependency);
        return dependency;
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
        this._registeredEntities.Remove(entity.Id);
    }

    /// <inheritdoc />
    public void Update(FrameTime frameTime, InputState inputState) {
        try {
            this._isBusy = true;
            this.RebuildFilterCaches();
            this.InvokePendingActions();

            foreach (var system in this.Systems.Where(x => x is { ShouldUpdate: true, Kind: GameSystemKind.PreUpdate })) {
                system.Update(frameTime, inputState);
            }

            foreach (var system in this.Systems.Where(x => x is { ShouldUpdate: true, Kind: GameSystemKind.Update })) {
                system.Update(frameTime, inputState);
            }

            foreach (var system in this.Systems.Where(x => x is { ShouldUpdate: true, Kind: GameSystemKind.PostUpdate })) {
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

    private void InvokePendingActions() {
        var actions = this._pendingActions.ToList();
        foreach (var action in actions) {
            action();
            this._pendingActions.Remove(action);
        }
    }

    private void RebuildFilterCaches() {
        this._animatableEntities.RebuildCache();
        this._cameras.RebuildCache();
        this._physicsBodies.RebuildCache();
        this._renderableEntities.RebuildCache();
        this._updateableEntities.RebuildCache();
        this._fixedUpdateableEntities.RebuildCache();
    }
}