namespace Macabresoft.MonoGame.Core {

    using Macabresoft.Core;
    using System.ComponentModel;
    using System.Runtime.Serialization;

    /// <summary>
    /// Interface for an system which runs operations for a <see cref="IGameScene" />.
    /// </summary>
    public interface IGameSystem : INotifyPropertyChanged {

        /// <summary>
        /// Initializes this service as a descendent of <paramref name="scene" />.
        /// </summary>
        /// <param name="scene">The scene.</param>
        void Initialize(IGameScene scene);
    }

    /// <summary>
    /// Base class for a system which runs operations for a <see cref="IGameScene" />.
    /// </summary>
    [DataContract]
    public abstract class GameSystem : PropertyChangedNotifier, IGameSystem {

        /// <summary>
        /// Gets the scene.
        /// </summary>
        /// <value>The scene.</value>
        protected IGameScene Scene { get; private set; } = GameScene.Empty;

        /// <inheritdoc />
        public virtual void Initialize(IGameScene scene) {
            this.Scene = scene;
        }
    }
}