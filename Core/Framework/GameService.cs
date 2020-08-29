namespace Macabresoft.MonoGame.Core {

    /// <summary>
    /// Interface for an service which runs operations for a <see cref="IGameScene" />.
    /// </summary>
    public interface IGameService : IGameInitializable {

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
    public abstract class GameService : IGameService {

        /// <inheritdoc />
        public abstract void Initialize(IGameScene scene);

        /// <inheritdoc />
        public abstract void Update(FrameTime frameTime, InputState inputState);
    }
}