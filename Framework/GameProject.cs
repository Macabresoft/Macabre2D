namespace Macabresoft.Macabre2D.Framework {
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Interface for a single project in the engine.
    /// </summary>
    public interface IGameProject {
        /// <summary>
        /// Gets the asset manager for this project.
        /// </summary>
        IAssetManager Assets { get; }

        /// <summary>
        /// Gets the name for this project.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the game settings for this project.
        /// </summary>
        IGameSettings Settings { get; }

        /// <summary>
        /// Gets the content identifier of the scene loaded on startup.
        /// </summary>
        Guid StartupSceneContentId { get; }
    }

    /// <summary>
    /// Defines a single project within the engine.
    /// </summary>
    [DataContract]
    public class GameProject : IGameProject {
        /// <summary>
        /// The default project name.
        /// </summary>
        public const string DefaultProjectName = "Project";

        /// <summary>
        /// The project file extension.
        /// </summary>
        public const string ProjectFileExtension = ".m2dproj";

        /// <summary>
        /// The project file name.
        /// </summary>
        public const string ProjectFileName = DefaultProjectName + ProjectFileExtension;

        /// <summary>
        /// Initializes a new instance of the <see cref="GameProject" /> class.
        /// </summary>
        /// <param name="assets">The asset manager.</param>
        /// <param name="settings">The settings.</param>
        /// <param name="name">The name.</param>
        /// <param name="startupSceneContentId">The identifier for the scene which should run on startup.</param>
        public GameProject(IAssetManager assets, IGameSettings settings, string name, Guid startupSceneContentId) {
            this.Assets = assets;
            this.Settings = settings;
            this.Name = name;
            this.StartupSceneContentId = startupSceneContentId;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GameProject" /> class.
        /// </summary>
        public GameProject() : this(new AssetManager(), new GameSettings(), DefaultProjectName, Guid.Empty) {
        }

        /// <summary>
        /// Gets the asset manager for this project.
        /// </summary>
        [DataMember]
        public IAssetManager Assets { get; }

        /// <inheritdoc />
        [DataMember]
        public IGameSettings Settings { get; }

        /// <inheritdoc />
        [DataMember]
        public string Name { get; set; }

        /// <inheritdoc />
        [DataMember]
        public Guid StartupSceneContentId { get; set; }
    }
}