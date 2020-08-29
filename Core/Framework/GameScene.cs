namespace Macabresoft.MonoGame.Core {

    using System;

    /// <summary>
    /// Interface for a combination of <see cref="IGameService" /> and <see cref="IGameEntity" />
    /// which runs on a <see cref="IGameLoop" />.
    /// </summary>
    public interface IGameScene : IGameUpdateable, ITransformable {
        IGameLoop GameLoop { get; }

        void Initialize(IGameLoop gameLoop);
    }

    /// <summary>
    /// A user-created combination of <see cref="IGameService" /> and <see cref="IGameEntity" />
    /// which runs on a <see cref="IGameLoop" />.
    /// </summary>
    public sealed class GameScene : IGameScene {

        /// <inheritdoc />
        public IGameLoop GameLoop => throw new NotImplementedException();

        /// <inheritdoc />
        public Transform Transform => throw new NotImplementedException();

        /// <inheritdoc />
        public void Initialize(IGameLoop gameLoop) {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public void Update(FrameTime frameTime, InputState inputState) {
            throw new NotImplementedException();
        }
    }
}