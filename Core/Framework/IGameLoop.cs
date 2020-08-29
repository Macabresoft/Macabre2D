namespace Macabresoft.MonoGame.Core {

    using System.Collections.Generic;

    /// <summary>
    /// Interface for the base object running the update loop for a game.
    /// </summary>
    public interface IGameLoop {

        /// <summary>
        /// Gets the scene.
        /// </summary>
        /// <value>The scene.</value>
        IGameScene Scene { get; }

        /// <summary>
        /// Gets the services.
        /// </summary>
        /// <value>The services.</value>
        IReadOnlyCollection<IGameService> Services { get; }

        /// <summary>
        /// Loads the scene.
        /// </summary>
        /// <param name="sceneName">Name of the scene to load.</param>
        void LoadScene(string sceneName);

        /// <summary>
        /// Loads the scene.
        /// </summary>
        /// <param name="scene">The scene.</param>
        void LoadScene(IGameScene scene);
    }
}