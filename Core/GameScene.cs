namespace Macabresoft.MonoGame.Core {

    using Macabresoft.Core;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Runtime.Serialization;

    /// <summary>
    /// Interface for a combination of <see cref="IGameSystem" /> and <see cref="IGameEntity" />
    /// which runs on a <see cref="IGame" />.
    /// </summary>
    public interface IGameScene : IGameEntity, IGameUpdateable {

        /// <summary>
        /// Occurs when a component is registered.
        /// </summary>
        event EventHandler<IGameComponent>? ComponentRegistered;

        /// <summary>
        /// Occurs when a component is unregistered.
        /// </summary>
        event EventHandler<IGameComponent>? ComponentUnregistered;

        /// <summary>
        /// Gets all components in this scene.
        /// </summary>
        /// <value>All components in this scene.</value>
        IReadOnlyCollection<IGameComponent> AllComponentsInScene { get => new IGameComponent[0]; }

        /// <summary>
        /// Gets or sets the color of the background.
        /// </summary>
        /// <value>The color of the background.</value>
        Color BackgroundColor { get; set; }

        /// <summary>
        /// Gets the camera components.
        /// </summary>
        /// <value>The camera components.</value>
        IReadOnlyCollection<IGameCameraComponent> CameraComponents { get => new IGameCameraComponent[0]; }

        /// <summary>
        /// Gets the game currently running this scene.
        /// </summary>
        /// <value>The game.</value>
        IGame Game { get => DefaultGame.Empty; }

        /// <summary>
        /// Gets the grid.
        /// </summary>
        /// <value>The grid.</value>
        TileGrid Grid { get => TileGrid.Empty; }

        /// <summary>
        /// Gets the physics bodies.
        /// </summary>
        /// <value>The physics bodies.</value>
        IReadOnlyCollection<IPhysicsBody> PhysicsBodies { get => new IPhysicsBody[0]; }

        /// <summary>
        /// Gets the renderable components.
        /// </summary>
        /// <value>The renderable components.</value>
        IReadOnlyCollection<IGameRenderableComponent> RenderableComponents { get => new IGameRenderableComponent[0]; }

        /// <summary>
        /// Gets the systems.
        /// </summary>
        /// <value>The systems.</value>
        IReadOnlyCollection<IGameSystem> Systems { get => new IGameSystem[0]; }

        /// <summary>
        /// Gets the updateable components.
        /// </summary>
        /// <value>The updateable components.</value>
        IReadOnlyCollection<IGameUpdateableComponent> UpdateableComponents { get => new IGameUpdateableComponent[0]; }

        /// <summary>
        /// Adds the service.
        /// </summary>
        /// <typeparam name="T">
        /// A type that implements <see cref="IGameSystem" /> and has an empty contructor.
        /// </typeparam>
        /// <returns>The added service.</returns>
        T AddService<T>() where T : IGameSystem, new();

        /// <summary>
        /// Adds the service.
        /// </summary>
        /// <param name="service">The service.</param>
        void AddService(IGameSystem service);

        /// <summary>
        /// Initializes this instance with the specified game.
        /// </summary>
        /// <param name="game">The game.</param>
        void Initialize(IGame game);

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
        bool RemoveService(IGameSystem service);

        /// <summary>
        /// Renders the scene.
        /// </summary>
        /// <param name="frameTime">The frame time.</param>
        public void Render(FrameTime frameTime);

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
        public static readonly new IGameScene Empty = new EmptyGameScene();

        private readonly List<Action> _actionsToInvoke = new List<Action>();
        private readonly HashSet<IGameComponent> _allComponentsInScene = new HashSet<IGameComponent>();

        private readonly FilterSortCollection<IGameCameraComponent> _cameraComponents = new FilterSortCollection<IGameCameraComponent>(
            c => c.IsEnabled,
            nameof(IGameComponent.IsEnabled),
            (c1, c2) => Comparer<int>.Default.Compare(c1.RenderOrder, c2.RenderOrder),
            nameof(IGameCameraComponent.RenderOrder));

        private readonly Dictionary<Type, object> _dependencies = new Dictionary<Type, object>();

        private readonly FilterSortCollection<IPhysicsBody> _physicsBodies = new FilterSortCollection<IPhysicsBody>(
            r => r.IsEnabled,
            nameof(IGameComponent.IsEnabled),
            (r1, r2) => Comparer<int>.Default.Compare(r1.UpdateOrder, r2.UpdateOrder),
            nameof(IPhysicsBody.UpdateOrder));

        private readonly FilterSortCollection<IGameRenderableComponent> _renderableComponents = new FilterSortCollection<IGameRenderableComponent>(
            c => c.IsVisible,
            nameof(IGameRenderableComponent.IsVisible),
            (c1, c2) => Comparer<int>.Default.Compare(c1.RenderOrder, c2.RenderOrder),
            nameof(IGameRenderableComponent.RenderOrder));

        private readonly QuadTree<IGameRenderableComponent> _renderTree = new QuadTree<IGameRenderableComponent>(0, float.MinValue * 0.5f, float.MinValue * 0.5f, float.MaxValue, float.MaxValue);

        [DataMember]
        private readonly ObservableCollection<IGameSystem> _services = new ObservableCollection<IGameSystem>();

        private readonly FilterSortCollection<IGameUpdateableComponent> _updateableComponents = new FilterSortCollection<IGameUpdateableComponent>(
            c => c.IsEnabled,
            nameof(IGameComponent.IsEnabled),
            (c1, c2) => Comparer<int>.Default.Compare(c1.UpdateOrder, c2.UpdateOrder),
            nameof(IGameUpdateableComponent.UpdateOrder));

        private bool _isInitialized = false;

        /// <inheritdoc />
        public event EventHandler<IGameComponent>? ComponentRegistered;

        /// <inheritdoc />
        public event EventHandler<IGameComponent>? ComponentUnregistered;

        /// <inheritdoc />
        public IReadOnlyCollection<IGameComponent> AllComponentsInScene => this._allComponentsInScene;

        /// <inheritdoc />
        [DataMember]
        public Color BackgroundColor { get; set; } = new Color(30, 15, 15);

        /// <inheritdoc />
        public IReadOnlyCollection<IGameCameraComponent> CameraComponents => this._cameraComponents;

        /// <inheritdoc />
        public IGame Game { get; private set; } = DefaultGame.Empty;

        /// <inheritdoc />
        public TileGrid Grid { get; set; } = new TileGrid(Vector2.One);

        /// <inheritdoc />
        public IReadOnlyCollection<IPhysicsBody> PhysicsBodies => this._physicsBodies;

        public IReadOnlyCollection<IGameRenderableComponent> RenderableComponents => this._renderableComponents;

        /// <inheritdoc />
        public IReadOnlyCollection<IGameSystem> Systems => this._services;

        /// <inheritdoc />
        public override Transform Transform => Transform.Origin;

        /// <inheritdoc />
        public override Matrix TransformMatrix => Matrix.Identity;

        /// <inheritdoc />
        public IReadOnlyCollection<IGameUpdateableComponent> UpdateableComponents => this._updateableComponents;

        /// <inheritdoc />
        public T AddService<T>() where T : IGameSystem, new() {
            var service = new T();
            this.AddService(service);
            return service;
        }

        /// <inheritdoc />
        public void AddService(IGameSystem service) {
            if (service != null) {
                this._services.Add(service);

                if (this._isInitialized) {
                    service.Initialize(this);
                }
            }
        }

        /// <inheritdoc />
        public void Initialize(IGame game) {
            if (!this._isInitialized) {
                try {
                    this.Game = game;
                    this.Initialize(this, this);

                    foreach (var service in this.Systems) {
                        service.Initialize(this);
                    }
                }
                finally {
                    this._isInitialized = true;
                }

                var actions = this._actionsToInvoke.ToList();
                foreach (var action in actions) {
                    action();
                    this._actionsToInvoke.Remove(action);
                }
            }
        }

        /// <inheritdoc />
        public void Invoke(Action action) {
            this._actionsToInvoke.Add(action);
        }

        /// <inheritdoc />
        public void RegisterComponent(IGameComponent component) {
            this._allComponentsInScene.Add(component);
            this._cameraComponents.Add(component);
            this._physicsBodies.Add(component);
            this._renderableComponents.Add(component);
            this._updateableComponents.Add(component);
            this.ComponentRegistered.SafeInvoke(this, component);
        }

        /// <inheritdoc />
        public bool RemoveService(IGameSystem service) {
            var result = false;
            if (service != null && this._services.Contains(service)) {
                this.Invoke(() => this._services.Remove(service));
                result = true;
            }

            return result;
        }

        /// <inheritdoc />
        public void Render(FrameTime frameTime) {
            if (this.Game.SpriteBatch != null && this.Game.GraphicsDevice != null) {
                this._renderTree.Clear();

                foreach (var component in this.RenderableComponents) {
                    this._renderTree.Insert(component);
                }

                this.Game.GraphicsDevice.Clear(Color.Black);

                foreach (var camera in this.CameraComponents) {
                    var potentialRenderables = this._renderTree.RetrievePotentialCollisions(camera.BoundingArea);

                    if (potentialRenderables.Any()) {
                        this.Game.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, camera.SamplerState, null, RasterizerState.CullNone, camera.Shader?.Effect, camera.ViewMatrix);

                        foreach (var component in potentialRenderables) {
                            // As long as it doesn't equal Layers.None, at least one of the layers
                            // defined on the component are also to be rendered by LayersToRender.
                            if ((component.Entity.Layers & camera.LayersToRender) != Layers.None) {
                                component.Render(frameTime, camera.BoundingArea);
                            }
                        }

                        this.Game.SpriteBatch.End();
                    }
                }
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
            this.ComponentUnregistered.SafeInvoke(this, component);
        }

        /// <inheritdoc />
        public void Update(FrameTime frameTime, InputState inputState) {
            foreach (var service in this.Systems) {
                service.Update(frameTime, inputState);
            }
        }

        internal class EmptyGameScene : EmptyGameEntity, IGameScene {

            /// <inheritdoc />
            public event EventHandler<IGameComponent>? ComponentRegistered;

            /// <inheritdoc />
            public event EventHandler<IGameComponent>? ComponentUnregistered;

            /// <inheritdoc />
            public Color BackgroundColor { get => Color.HotPink; set { return; } }

            /// <inheritdoc />
            public T AddService<T>() where T : IGameSystem, new() {
                return new T();
            }

            /// <inheritdoc />
            public void AddService(IGameSystem service) {
                return;
            }

            /// <inheritdoc />
            public void Initialize(IGame gameLoop) {
                return;
            }

            /// <inheritdoc />
            public void Invoke(Action action) {
                return;
            }

            /// <inheritdoc />
            public void RegisterComponent(IGameComponent component) {
                return;
            }

            /// <inheritdoc />
            public bool RemoveService(IGameSystem service) {
                return false;
            }

            /// <inheritdoc />
            public void Render(FrameTime frameTime) {
                return;
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
            public void UnregisterComponent(IGameComponent component) {
                return;
            }

            /// <inheritdoc />
            public void Update(FrameTime frameTime, InputState inputState) {
                return;
            }
        }
    }
}