namespace Macabresoft.Macabre2D.Framework {
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Runtime.Serialization;
    using Macabresoft.Core;
    using Microsoft.Xna.Framework;

    /// <summary>
    /// Interface for a combination of <see cref="IGameSystem" /> and <see cref="IGameEntity" />
    /// which runs on a <see cref="IGame" />.
    /// </summary>
    public interface IGameScene : IGameEntity, IGameUpdateable {
        
        /// <summary>
        /// Gets the asset manager.
        /// </summary>
        IAssetManager Assets => AssetManager.Empty;
        
        /// <summary>
        /// Gets the camera components.
        /// </summary>
        /// <value>The camera components.</value>
        IReadOnlyCollection<ICameraComponent> CameraComponents => Array.Empty<ICameraComponent>();

        /// <summary>
        /// Gets the game currently running this scene.
        /// </summary>
        /// <value>The game.</value>
        IGame Game => BaseGame.Empty;

        /// <summary>
        /// Gets the grid.
        /// </summary>
        /// <value>The grid.</value>
        TileGrid Grid => TileGrid.Empty;

        /// <summary>
        /// Gets the physics bodies.
        /// </summary>
        /// <value>The physics bodies.</value>
        IReadOnlyCollection<IPhysicsBodyComponent> PhysicsBodies => Array.Empty<IPhysicsBodyComponent>();

        /// <summary>
        /// Gets the renderable components.
        /// </summary>
        /// <value>The renderable components.</value>
        IReadOnlyCollection<IGameRenderableComponent> RenderableComponents => Array.Empty<IGameRenderableComponent>();

        /// <summary>
        /// Gets the systems.
        /// </summary>
        /// <value>The systems.</value>
        IReadOnlyCollection<IGameSystem> Systems => new IGameSystem[0];

        /// <summary>
        /// Gets the updateable components.
        /// </summary>
        /// <value>The updateable components.</value>
        IReadOnlyCollection<IGameUpdateableComponent> UpdateableComponents => new IGameUpdateableComponent[0];

        /// <summary>
        /// Gets or sets the color of the background.
        /// </summary>
        /// <value>The color of the background.</value>
        Color BackgroundColor { get; set; }

        /// <summary>
        /// Adds the service.
        /// </summary>
        /// <typeparam name="T">
        /// A type that implements <see cref="IGameSystem" /> and has an empty contructor.
        /// </typeparam>
        /// <returns>The added service.</returns>
        T AddSystem<T>() where T : IGameSystem, new();

        /// <summary>
        /// Adds the service.
        /// </summary>
        /// <param name="service">The service.</param>
        void AddSystem(IGameSystem service);

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        /// <param name="game">The game.</param>
        /// <param name="assetManager">The asset manager.</param>
        void Initialize(IGame game, IAssetManager assetManager);

        /// <summary>
        /// Invokes the specified action after the current update
        /// </summary>
        /// <param name="action">The action.</param>
        void Invoke(Action action);

        /// <summary>
        /// Registers the component with relevant services.
        /// </summary>
        /// <param name="component">The component.</param>
        void RegisterComponent(IGameComponent component);

        /// <summary>
        /// Removes the service.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <returns>A value indicating whether or not the service was removed.</returns>
        bool RemoveSystem(IGameSystem service);

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
        /// Unregisters the component from services.
        /// </summary>
        /// <param name="component">The component.</param>
        void UnregisterComponent(IGameComponent component);
    }

    /// <summary>
    /// A user-created combination of <see cref="IGameSystem" /> and <see cref="IGameEntity" />
    /// which runs on a <see cref="IGame" />.
    /// </summary>
    public sealed class GameScene : GameEntity, IGameScene {
        /// <summary>
        /// The default empty <see cref="IGameScene" /> that is present before initialization.
        /// </summary>
        public new static readonly IGameScene Empty = new EmptyGameScene();

        private readonly HashSet<IGameComponent> _allComponentsInScene = new();

        private readonly FilterSortCollection<ICameraComponent> _cameraComponents = new(
            c => c.IsEnabled,
            nameof(IGameComponent.IsEnabled),
            (c1, c2) => Comparer<int>.Default.Compare(c1.RenderOrder, c2.RenderOrder),
            nameof(ICameraComponent.RenderOrder));

        private readonly Dictionary<Type, object> _dependencies = new();
        private readonly List<Action> _pendingActions = new();

        private readonly FilterSortCollection<IPhysicsBodyComponent> _physicsBodies = new(
            r => r.IsEnabled,
            nameof(IGameComponent.IsEnabled),
            (r1, r2) => Comparer<int>.Default.Compare(r1.UpdateOrder, r2.UpdateOrder),
            nameof(IPhysicsBodyComponent.UpdateOrder));

        private readonly FilterSortCollection<IGameRenderableComponent> _renderableComponents = new(
            c => c.IsVisible,
            nameof(IGameRenderableComponent.IsVisible),
            (c1, c2) => Comparer<int>.Default.Compare(c1.RenderOrder, c2.RenderOrder),
            nameof(IGameRenderableComponent.RenderOrder));

        [DataMember]
        private readonly ObservableCollection<IGameSystem> _systems = new();

        private readonly FilterSortCollection<IGameUpdateableComponent> _updateableComponents = new(
            c => c.IsEnabled,
            nameof(IGameUpdateableComponent.IsEnabled),
            (c1, c2) => Comparer<int>.Default.Compare(c1.UpdateOrder, c2.UpdateOrder),
            nameof(IGameUpdateableComponent.UpdateOrder));

        private Color _backgroundColor = DefinedColors.MacabresoftBlack;
        private bool _isBusy;
        private bool _isInitialized;

        /// <inheritdoc />
        public IReadOnlyCollection<ICameraComponent> CameraComponents => this._cameraComponents;

        /// <inheritdoc />
        public IReadOnlyCollection<IPhysicsBodyComponent> PhysicsBodies => this._physicsBodies;

        public IReadOnlyCollection<IGameRenderableComponent> RenderableComponents => this._renderableComponents;

        /// <inheritdoc />
        public IReadOnlyCollection<IGameSystem> Systems => this._systems;

        /// <inheritdoc />
        public IReadOnlyCollection<IGameUpdateableComponent> UpdateableComponents => this._updateableComponents;

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
        public TileGrid Grid { get; set; } = new(Vector2.One);

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
        public void Invoke(Action action) {
            if (this._isBusy) {
                this._pendingActions.Add(action);
            }
            else {
                action();
            }
        }

        /// <summary>
        /// Determines whether the specified scene is null or <see cref="GameScene.Empty" />.
        /// </summary>
        /// <param name="scene">The scene.</param>
        /// <returns>
        /// <c>true</c> if the specified scene is null or <see cref="GameScene.Empty" />; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNullOrEmpty(IGameScene? scene) {
            return scene == null || scene == Empty;
        }

        /// <inheritdoc />
        public void RegisterComponent(IGameComponent component) {
            this._allComponentsInScene.Add(component);
            this._cameraComponents.Add(component);
            this._physicsBodies.Add(component);
            this._renderableComponents.Add(component);
            this._updateableComponents.Add(component);
        }

        /// <inheritdoc />
        public bool RemoveSystem(IGameSystem system) {
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
        public void UnregisterComponent(IGameComponent component) {
            this._allComponentsInScene.Remove(component);
            this._cameraComponents.Remove(component);
            this._physicsBodies.Remove(component);
            this._renderableComponents.Remove(component);
            this._updateableComponents.Remove(component);
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

        private void InvokePendingActions() {
            var actions = this._pendingActions.ToList();
            foreach (var action in actions) {
                action();
                this._pendingActions.Remove(action);
            }
        }

        private class EmptyGameScene : EmptyGameEntity, IGameScene {
            /// <inheritdoc />
            public Color BackgroundColor {
                get => Color.HotPink;
                set { }
            }

            /// <inheritdoc />
            public T AddSystem<T>() where T : IGameSystem, new() {
                return new();
            }

            /// <inheritdoc />
            public void AddSystem(IGameSystem service) {
            }

            /// <inheritdoc />
            public void Initialize(IGame gameLoop, IAssetManager assetManager) {
            }

            /// <inheritdoc />
            public void Invoke(Action action) {
            }

            /// <inheritdoc />
            public void RegisterComponent(IGameComponent component) {
            }

            /// <inheritdoc />
            public bool RemoveSystem(IGameSystem service) {
                return false;
            }

            /// <inheritdoc />
            public void Render(FrameTime frameTime, InputState inputState) {
            }

            /// <inheritdoc />
            public T ResolveDependency<T>() where T : new() {
                return new();
            }

            /// <inheritdoc />
            public T ResolveDependency<T>(Func<T> objectFactory) where T : class {
                return objectFactory.SafeInvoke();
            }

            /// <inheritdoc />
            public void UnregisterComponent(IGameComponent component) {
            }

            /// <inheritdoc />
            public void Update(FrameTime frameTime, InputState inputState) {
            }
        }
    }
}