namespace Macabresoft.MonoGame.Core {

    using Macabresoft.Core;
    using System.ComponentModel;
    using System.Runtime.Serialization;

    /// <summary>
    /// Interface for an service which runs operations for a <see cref="IGameScene" />.
    /// </summary>
    public interface IGameService : INotifyPropertyChanged {

        /// <summary>
        /// Initializes this service as a descendent of <paramref name="scene" />.
        /// </summary>
        /// <param name="scene">The scene.</param>
        void Initialize(IGameScene scene);

        /// <summary>
        /// Registers a component with this service.
        /// </summary>
        /// <param name="component">The component.</param>
        void RegisterComponent(IGameComponent component);

        /// <summary>
        /// Updates this instance.
        /// </summary>
        /// <param name="frameTime">The frame time.</param>
        /// <param name="inputState">State of the input.</param>
        void Update(FrameTime frameTime, InputState inputState);
    }

    /// <summary>
    /// Base class for a service which runs operations for a <see cref="IGameScene" />.
    /// </summary>
    [DataContract]
    public abstract class GameService : PropertyChangedNotifier, IGameService {

        /// <inheritdoc />
        public abstract void Initialize(IGameScene scene);

        /// <inheritdoc />
        public abstract void RegisterComponent(IGameComponent component);

        /// <inheritdoc />
        public abstract void Update(FrameTime frameTime, InputState inputState);
    }
}