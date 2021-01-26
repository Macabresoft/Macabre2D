namespace Macabresoft.Macabre2D.Framework {
    using System.Runtime.Serialization;
    using Microsoft.Xna.Framework.Content;

    /// <summary>
    /// Interface for a single project in the engine.
    /// </summary>
    public interface IGameProject {
        /// <summary>
        /// Gets the asset manager for this project.
        /// </summary>
        [DataMember]
        IAssetManager Assets { get; }

        /// <summary>
        /// Gets the game settings for this instance.
        /// </summary>
        [DataMember]
        IGameSettings Settings { get; }
        
        /// <summary>
        /// Initializes this instance.
        /// </summary>
        /// <param name="contentManager">The content manager.</param>
        void Initialize(ContentManager contentManager);
    }

    /// <summary>
    /// Defines a single project within the engine.
    /// </summary>
    [DataContract]
    public class GameProject : IGameProject {
        /// <summary>
        /// The content file name for <see cref="GameProject" />.
        /// </summary>
        public const string ContentFileName = "GameProject";

        /// <summary>
        /// Gets the singleton instance of <see cref="IGameProject"/>.
        /// </summary>
        public static IGameProject Instance { get; private set; } = new GameProject();
        
        /// <summary>
        /// Gets the asset manager for this project.
        /// </summary>
        [DataMember]
        public IAssetManager Assets { get; } = new AssetManager();

        /// <inheritdoc />
        [DataMember]
        public IGameSettings Settings { get; } = new GameSettings();

        /// <inheritdoc />
        public void Initialize(ContentManager contentManager) {
            Instance = this;
            this.Settings.Initialize();
            this.Assets.Initialize(contentManager);
        }
    }
}