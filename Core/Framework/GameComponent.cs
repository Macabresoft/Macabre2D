namespace Macabresoft.MonoGame.Core {

    /// <summary>
    /// Interface for a descendent of <see cref="IGameEntity" />.
    /// </summary>
    public interface IGameComponent : IGameInitializable {
    }

    /// <summary>
    /// A descendent of <see cref="IGameEntity" />.
    /// </summary>
    public abstract class GameComponent : IGameComponent {

        /// <inheritdoc />
        public abstract void Initialize(IGameScene scene);
    }
}