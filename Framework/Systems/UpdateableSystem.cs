namespace Macabresoft.Macabre2D.Framework {
    using System.ComponentModel;
    using System.Runtime.Serialization;
    using Macabresoft.Core;

    /// <summary>
    /// Interface for an system which runs operations for a <see cref="IScene" />.
    /// </summary>
    public interface IUpdateableSystem : INotifyPropertyChanged, IUpdateableGameObject, IEnableable {
        /// <summary>
        /// Gets the loop.
        /// </summary>
        /// <value>The loop.</value>
        SystemLoop Loop { get; }

        /// <summary>
        /// Initializes this service as a descendent of <paramref name="scene" />.
        /// </summary>
        /// <param name="scene">The scene.</param>
        void Initialize(IScene scene);
    }

    /// <summary>
    /// Base class for a system which runs operations for a <see cref="IScene" />.
    /// </summary>
    [DataContract]
    public abstract class UpdateableSystem : PropertyChangedNotifier, IUpdateableSystem {
        private bool _isEnabled = true;

        /// <inheritdoc />
        public abstract SystemLoop Loop { get; }

        /// <inheritdoc />
        [DataMember]
        public bool IsEnabled {
            get => this._isEnabled;
            set => this.Set(ref this._isEnabled, value);
        }

        /// <summary>
        /// Gets the scene.
        /// </summary>
        /// <value>The scene.</value>
        protected IScene Scene { get; private set; } = Framework.Scene.Empty;

        /// <inheritdoc />
        public virtual void Initialize(IScene scene) {
            this.Scene = scene;
        }

        /// <inheritdoc />
        public abstract void Update(FrameTime frameTime, InputState inputState);
    }
}