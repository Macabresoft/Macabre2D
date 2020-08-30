namespace Macabresoft.MonoGame.Core {

    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Runtime.Serialization;

    /// <summary>
    /// Interface for a combination of <see cref="IGameService" /> and <see cref="IGameEntity" />
    /// which runs on a <see cref="IGameLoop" />.
    /// </summary>
    public interface IGameScene : IGameEntity, IGameUpdateable {

        /// <summary>
        /// Gets the game loop.
        /// </summary>
        /// <value>The game loop.</value>
        IGameLoop GameLoop { get; }

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
    public sealed class GameScene : GameEntity, IGameScene {

        [DataMember]
        private readonly ObservableCollection<IGameService> _services = new ObservableCollection<IGameService>();

        /// <summary>
        /// Initializes a new instance of the <see cref="GameScene" /> class.
        /// </summary>
        public GameScene() {
        }

        /// <inheritdoc />
        public IGameLoop GameLoop { get; private set; }

        /// <inheritdoc />
        public IReadOnlyCollection<IGameService> Services => this._services;

        /// <inheritdoc />
        public override Transform Transform => Transform.Origin;

        /// <inheritdoc />
        public void Initialize(IGameLoop gameLoop) {
            if (this.GameLoop != null) {
                this.GameLoop = gameLoop;
                this.Initialize(this, this);
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
    }
}