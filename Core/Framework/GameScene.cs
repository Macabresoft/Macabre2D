namespace Macabresoft.MonoGame.Core {

    using Macabresoft.Core;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Runtime.Serialization;

    /// <summary>
    /// Interface for a combination of <see cref="IGameService" /> and <see cref="IGameEntity" />
    /// which runs on a <see cref="IGameLoop" />.
    /// </summary>
    public interface IGameScene : IGameEntity, IGameUpdateable {

        /// <summary>
        /// Occurs when a component is registered.
        /// </summary>
        event EventHandler<IGameComponent> ComponentRegistered;

        /// <summary>
        /// Occurs when a component is unregistered.
        /// </summary>
        event EventHandler<IGameComponent> ComponentUnregistered;

        /// <summary>
        /// Gets all components in this scene.
        /// </summary>
        /// <value>All components in this scene.</value>
        IReadOnlyCollection<IGameComponent> AllComponentsInScene { get; }

        /// <summary>
        /// Gets the camera components.
        /// </summary>
        /// <value>The camera components.</value>
        IReadOnlyCollection<IGameCameraComponent> CameraComponents { get; }

        /// <summary>
        /// Gets the game loop.
        /// </summary>
        /// <value>The game loop.</value>
        IGameLoop GameLoop { get; }

        /// <summary>
        /// Gets the renderable components.
        /// </summary>
        /// <value>The renderable components.</value>
        IReadOnlyCollection<IGameRenderableComponent> RenderableComponents { get; }

        /// <summary>
        /// Gets the services.
        /// </summary>
        /// <value>The services.</value>
        IReadOnlyCollection<IGameService> Services { get; }

        /// <summary>
        /// Gets the updateable components.
        /// </summary>
        /// <value>The updateable components.</value>
        IReadOnlyCollection<IGameUpdateableComponent> UpdateableComponents { get; }

        /// <summary>
        /// Adds the service.
        /// </summary>
        /// <typeparam name="T">
        /// A type that implements <see cref="IGameService" /> and has an empty contructor.
        /// </typeparam>
        /// <returns>The added service.</returns>
        T AddService<T>() where T : IGameService, new();

        /// <summary>
        /// Adds the service.
        /// </summary>
        /// <param name="service">The service.</param>
        void AddService(IGameService service);

        /// <summary>
        /// Initializes the specified game loop.
        /// </summary>
        /// <param name="gameLoop">The game loop.</param>
        void Initialize(IGameLoop gameLoop);

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
        bool RemoveService(IGameService service);

        /// <summary>
        /// Unregisters the component from services.
        /// </summary>
        /// <param name="component">The component.</param>
        void UnregisterComponent(IGameComponent component);
    }

    /// <summary>
    /// A user-created combination of <see cref="IGameService" /> and <see cref="IGameEntity" />
    /// which runs on a <see cref="IGameLoop" />.
    /// </summary>
    public sealed class GameScene : GameEntity, IGameScene {
        private readonly Queue<Action> _actionsToInvoke = new Queue<Action>();

        private readonly HashSet<IGameComponent> _allComponentsInScene = new HashSet<IGameComponent>();

        private readonly FilterSortCollection<IGameCameraComponent> _cameraComponents = new FilterSortCollection<IGameCameraComponent>(
            c => c.IsEnabled,
            nameof(IGameCameraComponent.IsEnabled),
            (c1, c2) => Comparer<int>.Default.Compare(c1.RenderOrder, c2.RenderOrder),
            nameof(IGameCameraComponent.RenderOrder));

        private readonly FilterSortCollection<IGameRenderableComponent> _renderableComponents = new FilterSortCollection<IGameRenderableComponent>(
            c => c.IsVisible,
            nameof(IGameRenderableComponent.IsVisible),
            (c1, c2) => Comparer<int>.Default.Compare(c1.RenderOrder, c2.RenderOrder),
            nameof(IGameRenderableComponent.RenderOrder));

        [DataMember]
        private readonly ObservableCollection<IGameService> _services = new ObservableCollection<IGameService>();

        private readonly FilterSortCollection<IGameUpdateableComponent> _updateableComponents = new FilterSortCollection<IGameUpdateableComponent>(
            c => c.IsEnabled,
            nameof(IGameUpdateableComponent.IsEnabled),
            (c1, c2) => Comparer<int>.Default.Compare(c1.UpdateOrder, c2.UpdateOrder),
            nameof(IGameUpdateableComponent.UpdateOrder));

        private bool _isInitialized = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="GameScene" /> class.
        /// </summary>
        public GameScene() {
        }

        /// <inheritdoc />
        public event EventHandler<IGameComponent> ComponentRegistered;

        /// <inheritdoc />
        public event EventHandler<IGameComponent> ComponentUnregistered;

        /// <inheritdoc />
        public IReadOnlyCollection<IGameComponent> AllComponentsInScene => this._allComponentsInScene;

        /// <inheritdoc />
        public IReadOnlyCollection<IGameCameraComponent> CameraComponents => this._cameraComponents;

        /// <inheritdoc />
        public IGameLoop GameLoop { get; private set; }

        public IReadOnlyCollection<IGameRenderableComponent> RenderableComponents => this._renderableComponents;

        /// <inheritdoc />
        public IReadOnlyCollection<IGameService> Services => this._services;

        /// <inheritdoc />
        public override Transform Transform => Transform.Origin;

        /// <inheritdoc />
        public IReadOnlyCollection<IGameUpdateableComponent> UpdateableComponents => this._updateableComponents;

        /// <inheritdoc />
        public T AddService<T>() where T : IGameService, new() {
            var service = new T();
            this.AddService(service);
            return service;
        }

        /// <inheritdoc />
        public void AddService(IGameService service) {
            if (service != null) {
                this._services.Add(service);

                if (this._isInitialized) {
                    service.Initialize(this);
                }
            }
        }

        /// <inheritdoc />
        public void Initialize(IGameLoop gameLoop) {
            if (!this._isInitialized) {
                try {
                    this.GameLoop = gameLoop;
                    this.Initialize(this, this);

                    foreach (var service in this.Services) {
                        service.Initialize(this);
                    }
                }
                finally {
                    this._isInitialized = true;
                }
            }
        }

        /// <inheritdoc />
        public void Invoke(Action action) {
            this._actionsToInvoke.Enqueue(action);
        }

        /// <inheritdoc />
        public void RegisterComponent(IGameComponent component) {
            this._allComponentsInScene.Add(component);
            this._cameraComponents.Add(component);
            this._renderableComponents.Add(component);
            this._updateableComponents.Add(component);
            this.ComponentUnregistered.SafeInvoke(this, component);
        }

        /// <inheritdoc />
        public bool RemoveService(IGameService service) {
            var result = false;
            if (service != null && this._services.Contains(service)) {
                this.Invoke(() => this._services.Remove(service));
                result = true;
            }

            return result;
        }

        /// <inheritdoc />
        public void UnregisterComponent(IGameComponent component) {
            this._allComponentsInScene.Add(component);
            this._cameraComponents.Remove(component);
            this._renderableComponents.Remove(component);
            this._updateableComponents.Remove(component);
            this.ComponentUnregistered.SafeInvoke(this, component);
        }

        /// <inheritdoc />
        public void Update(FrameTime frameTime, InputState inputState) {
            foreach (var service in this.Services) {
                service.Update(frameTime, inputState);
            }
        }
    }
}