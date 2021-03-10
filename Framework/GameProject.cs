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
        IAssetManager Assets { get; }

        /// <summary>
        /// Gets the game settings for this project.
        /// </summary>
        IGameSettings Settings { get; }
        
        /// <summary>
        /// Gets the name for this project.
        /// </summary>
        string Name { get; }
        
    }

    /// <summary>
    /// Defines a single project within the engine.
    /// </summary>
    [DataContract]
    public class GameProject : IGameProject {
        
        /// <summary>
        /// The project file extension.
        /// </summary>
        public const string ProjectFileName = ".m2dproj";

        
        /// <summary>
        /// Gets the asset manager for this project.
        /// </summary>
        [DataMember]
        public IAssetManager Assets { get; } = new AssetManager();

        /// <inheritdoc />
        [DataMember]
        public IGameSettings Settings { get; } = new GameSettings();

        /// <inheritdoc />
        [DataMember]
        public string Name { get; set; } = "Macabre2D Project";
    }
}