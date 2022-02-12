namespace Macabresoft.Macabre2D.Framework;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Macabresoft.Core;
using Microsoft.Xna.Framework;

/// <summary>
/// Interface for a combination of <see cref="IUpdateableSystem" /> and <see cref="IEntity" />
/// which runs on a <see cref="IGame" />.
/// </summary>
public interface IScene : IUpdateableGameObject, IGridContainer {
    /// <summary>
    /// Gets the asset manager.
    /// </summary>
    IAssetManager Assets => AssetManager.Empty;

    /// <summary>
    /// Gets the cameras in the scene.
    /// </summary>
    /// <value>The cameras.</value>
    IReadOnlyCollection<ICamera> Cameras => Array.Empty<ICamera>();

    /// <summary>
    /// Gets the game currently running this scene.
    /// </summary>
    /// <value>The game.</value>
    IGame Game => BaseGame.Empty;

    /// <summary>
    /// Gets the named children.
    /// </summary>
    IReadOnlyCollection<INameableCollection> NamedChildren => Array.Empty<INameableCollection>();

    /// <summary>
    /// Gets the physics bodies.
    /// </summary>
    /// <value>The physics bodies.</value>
    IReadOnlyCollection<IPhysicsBody> PhysicsBodies => Array.Empty<IPhysicsBody>();

    /// <summary>
    /// Gets the renderable entities in the scene.
    /// </summary>
    /// <value>The renderable entities.</value>
    IReadOnlyCollection<IRenderableEntity> RenderableEntities => Array.Empty<IRenderableEntity>();

    /// <summary>
    /// Gets the systems.
    /// </summary>
    /// <value>The systems.</value>
    IReadOnlyCollection<IUpdateableSystem> Systems => Array.Empty<IUpdateableSystem>();

    /// <summary>
    /// Gets the updateable entities.
    /// </summary>
    /// <value>The updateable entities.</value>
    IReadOnlyCollection<IUpdateableEntity> UpdateableEntities => Array.Empty<IUpdateableEntity>();

    /// <summary>
    /// Gets or sets the color of the background.
    /// </summary>
    /// <value>The color of the background.</value>
    Color BackgroundColor { get; set; }

    /// <summary>
    /// Gets or sets the version of this scene.
    /// </summary>
    Version Version { get; set; }

    /// <summary>
    /// Adds the system.
    /// </summary>
    /// <typeparam name="T">
    /// A type that implements <see cref="IUpdateableSystem" /> and has an empty constructor.
    /// </typeparam>
    /// <returns>The added system.</returns>
    T AddSystem<T>() where T : IUpdateableSystem, new();

    /// <summary>
    /// Adds the system.
    /// </summary>
    /// <param name="system">The system.</param>
    void AddSystem(IUpdateableSystem system);

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
    void InsertSystem(int index, IUpdateableSystem system);

    /// <summary>
    /// Invokes the specified action after the current update
    /// </summary>
    /// <param name="action">The action.</param>
    void Invoke(Action action);

    /// <summary>
    /// Registers the entity with relevant services.
    /// </summary>
    /// <param name="entity">The entity.</param>
    void RegisterEntity(IEntity entity);

    /// <summary>
    /// Removes the system.
    /// </summary>
    /// <param name="system">The system.</param>
    /// <returns>A value indicating whether or not the system was removed.</returns>
    bool RemoveSystem(IUpdateableSystem system);

    /// <summary>
    /// Renders the scene.
    /// </summary>
    /// <param name="frameTime">The frame time.</param>
    /// <param name="inputState">The input state.</param>
    public void Render(FrameTime frameTime, InputState inputState);

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
/// A user-created combination of <see cref="IUpdateableSystem" /> and <see cref="IEntity" />
/// which runs on a <see cref="IGame" />.
/// </summary>
public sealed class Scene : GridContainer, IScene {
    /// <summary>
    /// The default empty <see cref="IScene" /> that is present before initialization.
    /// </summary>
    public new static readonly IScene Empty = new EmptyScene();

    private readonly FilterSortCollection<ICamera> _cameras = new(
        c => c.IsEnabled,
        nameof(IEnableable.IsEnabled),
        (c1, c2) => Comparer<int>.Default.Compare(c1.RenderOrder, c2.RenderOrder),
        nameof(ICamera.RenderOrder));

    private readonly Dictionary<Type, object> _dependencies = new();
    private readonly List<INameableCollection> _namedChildren = new();
    private readonly List<Action> _pendingActions = new();

    private readonly FilterSortCollection<IPhysicsBody> _physicsBodies = new(
        r => r.IsEnabled,
        nameof(IEnableable.IsEnabled),
        (r1, r2) => Comparer<int>.Default.Compare(r1.UpdateOrder, r2.UpdateOrder),
        nameof(IPhysicsBody.UpdateOrder));

    private readonly FilterSortCollection<IRenderableEntity> _renderableEntities = new(
        c => c.IsVisible,
        nameof(IRenderableEntity.IsVisible),
        (c1, c2) => Comparer<int>.Default.Compare(c1.RenderOrder, c2.RenderOrder),
        nameof(IRenderableEntity.RenderOrder));

    [DataMember]
    private readonly SystemCollection _systems = new();

    private readonly FilterSortCollection<IUpdateableEntity> _updateableEntities = new(
        c => c.IsEnabled,
        nameof(IUpdateableEntity.IsEnabled),
        (c1, c2) => Comparer<int>.Default.Compare(c1.UpdateOrder, c2.UpdateOrder),
        nameof(IUpdateableEntity.UpdateOrder));

    private Color _backgroundColor = DefinedColors.MacabresoftBlack;
    private bool _isBusy;
    private bool _isInitialized;
    private Version _version = new(0, 0, 0, 0);

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
    public IReadOnlyCollection<ICamera> Cameras => this._cameras;

    /// <inheritdoc />
    public IReadOnlyCollection<INameableCollection> NamedChildren => this._namedChildren;

    /// <inheritdoc />
    public IReadOnlyCollection<IPhysicsBody> PhysicsBodies => this._physicsBodies;

    /// <inheritdoc />
    public IReadOnlyCollection<IRenderableEntity> RenderableEntities => this._renderableEntities;

    /// <inheritdoc />
    public IReadOnlyCollection<IUpdateableSystem> Systems => this._systems;

    /// <inheritdoc />
    public IReadOnlyCollection<IUpdateableEntity> UpdateableEntities => this._updateableEntities;

    /// <inheritdoc />
    public IAssetManager Assets { get; private set; } = AssetManager.Empty;

    /// <inheritdoc />
    [DataMember]
    public Color BackgroundColor {
        get => this._backgroundColor;
        set => this.Set(ref this._backgroundColor, value);
    }

    /// <inheritdoc />
    public IGame Game { get; private set; } = BaseGame.Empty;

    /// <inheritdoc />
    [DataMember]
    public Version Version {
        get => this._version;
        set => this.Set(ref this._version, value);
    }

    /// <inheritdoc />
    public T AddSystem<T>() where T : IUpdateableSystem, new() {
        var system = new T();
        this.AddSystem(system);
        return system;
    }

    /// <inheritdoc />
    public void AddSystem(IUpdateableSystem system) {
        this._systems.Add(system);

        if (this._isInitialized) {
            system.Initialize(this);
        }
    }

    /// <inheritdoc />
    public void Initialize(IGame game, IAssetManager assetManager) {
        if (!this._isInitialized) {
            try {
                this._isBusy = true;
                this.Assets = assetManager;
                this.Game = game;
                this.Initialize(this, this);

                foreach (var system in this.Systems) {
                    system.Initialize(this);
                }
            }
            finally {
                this._isInitialized = true;
                this._isBusy = false;
            }

            this.InvokePendingActions();
        }
    }

    /// <inheritdoc />
    public void InsertSystem(int index, IUpdateableSystem system) {
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
    public static bool IsNullOrEmpty(IScene? scene) {
        return scene == null || scene == Empty;
    }

    /// <inheritdoc />
    public void RegisterEntity(IEntity entity) {
        this._cameras.Add(entity);
        this._physicsBodies.Add(entity);
        this._renderableEntities.Add(entity);
        this._updateableEntities.Add(entity);
    }

    /// <inheritdoc />
    public bool RemoveSystem(IUpdateableSystem system) {
        var result = false;
        if (this._systems.Contains(system)) {
            this.Invoke(() => this._systems.Remove(system));
            result = true;
        }

        return result;
    }

    /// <inheritdoc />
    public void Render(FrameTime frameTime, InputState inputState) {
        try {
            this._isBusy = true;

            foreach (var system in this.Systems.Where(x => x.IsEnabled && x.Loop == SystemLoop.Render)) {
                system.Update(frameTime, inputState);
            }
        }
        finally {
            this._isBusy = false;
        }
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
    public override bool TryGetParentEntity<T>(out T? entity) where T : class {
        entity = null;
        return false;
    }

    /// <inheritdoc />
    public void UnregisterEntity(IEntity entity) {
        this._cameras.Remove(entity);
        this._physicsBodies.Remove(entity);
        this._renderableEntities.Remove(entity);
        this._updateableEntities.Remove(entity);
    }

    /// <inheritdoc />
    public void Update(FrameTime frameTime, InputState inputState) {
        try {
            this._isBusy = true;

            this.InvokePendingActions();

            foreach (var system in this.Systems.Where(x => x.IsEnabled && x.Loop == SystemLoop.Update)) {
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

    private class EmptyScene : EmptyGridContainer, IScene {
        /// <inheritdoc />
        public Color BackgroundColor {
            get => Color.HotPink;
            set { }
        }

        /// <inheritdoc />
        public Version Version { get; set; } = new();

        /// <inheritdoc />
        public T AddSystem<T>() where T : IUpdateableSystem, new() {
            return new T();
        }

        /// <inheritdoc />
        public void AddSystem(IUpdateableSystem service) {
        }

        /// <inheritdoc />
        public void Initialize(IGame gameLoop, IAssetManager assetManager) {
        }

        /// <inheritdoc />
        public void InsertSystem(int index, IUpdateableSystem system) {
        }

        /// <inheritdoc />
        public void Invoke(Action action) {
        }

        /// <inheritdoc />
        public void RegisterEntity(IEntity entity) {
        }

        /// <inheritdoc />
        public bool RemoveSystem(IUpdateableSystem service) {
            return false;
        }

        /// <inheritdoc />
        public void Render(FrameTime frameTime, InputState inputState) {
        }

        /// <inheritdoc />
        public T ResolveDependency<T>() where T : new() {
            return new T();
        }

        /// <inheritdoc />
        public T ResolveDependency<T>(Func<T> objectFactory) where T : class {
            return objectFactory.SafeInvoke();
        }

        /// <inheritdoc />
        public void UnregisterEntity(IEntity entity) {
        }

        /// <inheritdoc />
        public void Update(FrameTime frameTime, InputState inputState) {
        }
    }
}