namespace Macabresoft.MonoGame.Core {

    using Macabresoft.Core;
    using Macabresoft.MonoGame.Core.Framework;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Runtime.Serialization;

    /// <summary>
    /// Interface for a combination of <see cref="IGameService" /> and <see cref="IGameEntity" />
    /// which runs on a <see cref="IGameLoop" />.
    /// </summary>
    public interface IGameScene : IGameEntityParent, IGameUpdateable {

        /// <summary>
        /// Gets the game loop.
        /// </summary>
        /// <value>The game loop.</value>
        IGameLoop GameLoop { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is enabled.
        /// </summary>
        /// <value><c>true</c> if this instance is enabled; otherwise, <c>false</c>.</value>
        bool IsEnabled { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        string Name { get; set; }

        /// <summary>
        /// Gets the services.
        /// </summary>
        /// <value>The services.</value>
        IReadOnlyCollection<IGameService> Services { get; }

        /// <summary>
        /// Initializes the specified game loop.
        /// </summary>
        /// <param name="gameLoop">The game loop.</param>
        void Initialize(IGameLoop gameLoop);

        /// <summary>
        /// Registers the component with relevant services.
        /// </summary>
        /// <param name="component">The component.</param>
        void RegisterComponent(IGameComponent component);
    }

    /// <summary>
    /// A user-created combination of <see cref="IGameService" /> and <see cref="IGameEntity" />
    /// which runs on a <see cref="IGameLoop" />.
    /// </summary>
    public sealed class GameScene : GameEntityParent, IGameScene {
        private readonly EmptyEntity _rootEntity;

        [DataMember]
        private readonly ObservableCollection<IGameService> _services = new ObservableCollection<IGameService>();

        private bool _isEnabled;

        [DataMember]
        private string _name;

        /// <summary>
        /// Initializes a new instance of the <see cref="GameScene" /> class.
        /// </summary>
        public GameScene() {
            this._rootEntity = new EmptyEntity(this);
        }

        /// <inheritdoc />
        public IGameLoop GameLoop { get; private set; }

        /// <inheritdoc />
        public bool IsEnabled {
            get {
                return this._isEnabled;
            }

            set {
                this.Set(ref this._isEnabled, value);
            }
        }

        /// <inheritdoc />
        public string Name {
            get {
                return this._name;
            }

            set {
                this.Set(ref this._name, value);
            }
        }

        /// <inheritdoc />
        public IReadOnlyCollection<IGameService> Services => this._services;

        /// <inheritdoc />
        public void Initialize(IGameLoop gameLoop) {
            if (this.GameLoop != null) {
                this.GameLoop = gameLoop;
                this._rootEntity.Initialize(this, null);
            }
        }

        /// <inheritdoc />
        public void RegisterComponent(IGameComponent component) {
            foreach (var service in this.Services) {
                service.RegisterComponent(component);
            }
        }

        /// <inheritdoc />
        public void Update(FrameTime frameTime, InputState inputState) {
            foreach (var service in this.Services) {
                service.Update(frameTime, inputState);
            }
        }

        /// <inheritdoc />
        protected override void OnAddChild(IGameEntity entity) {
            if (this.GameLoop != null) {
                entity.Initialize(this, this._rootEntity);
            }
        }

        private class EmptyEntity : PropertyChangedNotifier, IGameEntity {
            private bool _isInitialized;

            /// <summary>
            /// Initializes a new instance of the <see cref="EmptyEntity" /> class.
            /// </summary>
            /// <param name="scene">The scene.</param>
            public EmptyEntity(IGameScene scene) {
                this.Scene = scene;
            }

            /// <inheritdoc />
            public IReadOnlyCollection<IGameEntity> Children => this.Scene.Children;

            /// <inheritdoc />
            public IReadOnlyCollection<IGameComponent> Components { get; } = new IGameComponent[0];

            /// <inheritdoc />
            public bool IsEnabled {
                get {
                    return this.Scene.IsEnabled;
                }

                set {
                    this.Scene.IsEnabled = value;
                    this.RaisePropertyChanged();
                }
            }

            /// <inheritdoc />
            public string Name {
                get {
                    return this.Scene.Name;
                }

                set {
                    this.Scene.Name = value;
                    this.RaisePropertyChanged();
                }
            }

            /// <inheritdoc />
            public IGameEntity Parent {
                get {
                    return null;
                }
            }

            /// <inheritdoc />
            public IGameScene Scene { get; }

            /// <inheritdoc />
            public Transform Transform {
                get {
                    return Transform.Origin;
                }
            }

            /// <inheritdoc />
            public T AddChild<T>() where T : IGameEntity, new() {
                return this.Scene.AddChild<T>();
            }

            /// <inheritdoc />
            public IGameEntity AddChild() {
                return this.Scene.AddChild();
            }

            /// <inheritdoc />
            public void AddChild(IGameEntity entity) {
                this.Scene.AddChild(entity);
            }

            /// <inheritdoc />
            public T AddComponent<T>() where T : IGameComponent, new() {
                return default;
            }

            /// <inheritdoc />
            public void AddComponent(IGameComponent component) {
                return;
            }

            /// <inheritdoc />
            public void Initialize(IGameScene scene, IGameEntity parent) {
                if (!this._isInitialized) {
                    try {
                        foreach (var child in this.Children) {
                            child.Initialize(scene, this);
                        }
                    }
                    finally {
                        this._isInitialized = true;
                    }
                }
            }

            /// <inheritdoc />
            public bool IsDescendentOf(IGameEntity entity) {
                return false;
            }

            /// <inheritdoc />
            public bool RemoveChild(IGameEntity entity) {
                return this.Scene.RemoveChild(entity);
            }

            /// <inheritdoc />
            public bool RemoveComponent(IGameComponent component) {
                return false;
            }
        }
    }
}