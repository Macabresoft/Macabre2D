namespace Macabresoft.MonoGame.Core {

    /// <summary>
    /// Interface for an initializable descendent of <see cref="IGameScene" />.
    /// </summary>
    public interface IGameInitializable {

        /// <summary>
        /// Initializes this instance as a descendent of <paramref name="scene" />.
        /// </summary>
        /// <param name="scene">The scene.</param>
        void Initialize(IGameScene scene);
    }
}