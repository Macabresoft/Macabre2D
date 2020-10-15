namespace Macabresoft.Macabre2D.Framework {

    using Macabresoft.Core;
    using System.ComponentModel;
    using System.Runtime.Serialization;

    /// <summary>
    /// Interface for an system which runs operations for a <see cref="IGameScene" />.
    /// </summary>
    public interface IGameSystem : INotifyPropertyChanged, IGameUpdateable, IEnableable {

        /// <summary>
        /// Gets the loop.
        /// </summary>
        /// <value>The loop.</value>
        SystemLoop Loop { get; }

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
        private bool _isEnabled = true;

        /// <inheritdoc />
        [DataMember]
        public bool IsEnabled {
            get {
                return this._isEnabled;
            }

            set {
                this.Set(ref this._isEnabled, value);
            }
        }

        /// <inheritdoc />
        public abstract SystemLoop Loop { get; }

        /// <summary>
        /// Gets the scene.
        /// </summary>
        /// <value>The scene.</value>
        protected IGameScene Scene { get; private set; } = GameScene.Empty;

        /// <inheritdoc />
        public virtual void Initialize(IGameScene scene) {
            this.Scene = scene;
        }

        /// <inheritdoc />
        public abstract void Update(FrameTime frameTime, InputState inputState);
    }
}