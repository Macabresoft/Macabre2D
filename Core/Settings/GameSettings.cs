namespace Macabresoft.MonoGame.Core {

    using Microsoft.Xna.Framework;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// An interface to read common game settings.
    /// </summary>
    public interface IGameSettings {

        /// <summary>
        /// Gets the default graphics settings.
        /// </summary>
        /// <value>The default graphics settings.</value>
        GraphicsSettings DefaultGraphicsSettings { get; }

        /// <summary>
        /// Gets the default resolution.
        /// </summary>
        /// <value>The default resolution.</value>
        Point DefaultResolution { get; }

        /// <summary>
        /// Gets or sets the color that sprites will be filled in with if their content cannot be loaded.
        /// </summary>
        Color ErrorSpritesColor { get; }

        /// <summary>
        /// Gets the color of the game background when there is no scene opened.
        /// </summary>
        /// <value>The fallback background color.</value>
        Color FallbackBackgroundColor { get; }

        /// <summary>
        /// Gets the inverse of <see cref="PixelsPerUnit" />.
        /// </summary>
        /// <value>
        /// <remarks>This will be calculated when <see cref="PixelsPerUnit" /> is set.
        /// Multiplication is a quicker operation than division, so if you find yourself dividing by
        /// <see cref="PixelsPerUnit" /> regularly, consider multiplying by this instead as it will
        /// produce the same value, but quicker.</remarks> The inverse pixels per unit.
        /// </value>
        float InversePixelsPerUnit { get; }

        /// <summary>
        /// Gets the layer settings.
        /// </summary>
        /// <value>The layer settings.</value>
        LayerSettings Layers { get; }

        /// <summary>
        /// Getsthe pixels per unit. This value is the number of pixels per abritrary game units.
        /// </summary>
        /// <value>The pixel density.</value>
        int PixelsPerUnit { get; }

        /// <summary>
        /// Gets the name of the project.
        /// </summary>
        /// <value>The name of the project.</value>
        string ProjectName { get; }

        /// <summary>
        /// Gets the startup scene asset identifier.
        /// </summary>
        /// <value>The startup scene asset identifier.</value>
        Guid StartupSceneAssetId { get; }

        /// <summary>
        /// Gets a pixel agnostic ratio. This can be used to make something appear the same size on
        /// screen regardless of the current view size.
        /// </summary>
        /// <param name="unitViewHeight">Height of the unit view.</param>
        /// <param name="pixelViewHeight">Height of the pixel view.</param>
        /// <returns>A pixel agnostic ratio.</returns>
        float GetPixelAgnosticRatio(float unitViewHeight, int pixelViewHeight);

        /// <summary>
        /// Tries to get a custom setting.
        /// </summary>
        /// <param name="settingName">Name of the setting.</param>
        /// <param name="settingValue">The setting value.</param>
        /// <returns>A value indicating whether or not the custom setting was found.</returns>
        bool TryGetCustomSetting(string settingName, out string settingValue);
    }

    /// <summary>
    /// A place for common game settings to be serialized across scenes.
    /// </summary>
    [DataContract]
    public sealed class GameSettings : IGameSettings {

        /// <summary>
        /// The content file name for <see cref="GameSettings" />.
        /// </summary>
        public const string ContentFileName = "Settings";

        private static IGameSettings _instance = new GameSettings();

        [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Replace)]
        private readonly Dictionary<string, string> _customSettings = new Dictionary<string, string>();

        private int _pixelsPerUnit = 32;

        /// <summary>
        /// Initializes the <see cref="GameSettings" /> class.
        /// </summary>
        static GameSettings() {
        }

        /// <summary>
        /// Gets the singleton instance of game settings.
        /// </summary>
        /// <remarks>
        /// Honestly, I'm not a huge fan of singletons. They feel like bad design to me. But there
        /// are objects in a tight loop, like <see cref="Sprite" /> that need access to some
        /// settings, but <see cref="Sprite" /> was written to not know a whole lot about anything
        /// else. I think it should stay that way. I created a readonly interface of <see
        /// cref="GameSettings" /> called <see cref="IGameSettings" /> to alleviate some of the
        /// grossness of this. The setter is internal, so only <see cref="MacabreGame" /> should
        /// ever set it.
        /// </remarks>
        public static IGameSettings Instance {
            get {
                return GameSettings._instance;
            }

            set {
                if (value != null) {
                    GameSettings._instance = value;
                }
            }
        }

        /// <inheritdoc />
        [DataMember]
        public GraphicsSettings DefaultGraphicsSettings { get; } = new GraphicsSettings();

        /// <inheritdoc />
        [DataMember]
        public Point DefaultResolution { get; set; }

        /// <inheritdoc />
        [DataMember]
        public Color ErrorSpritesColor { get; set; } = Color.HotPink;

        /// <inheritdoc />
        [DataMember]
        public Color FallbackBackgroundColor { get; set; } = Color.Black;

        /// <inheritdoc />
        public float InversePixelsPerUnit { get; private set; } = 1f / 32f;

        /// <inheritdoc />
        [DataMember]
        public LayerSettings Layers { get; } = new LayerSettings();

        /// <inheritdoc />
        [DataMember]
        public int PixelsPerUnit {
            get {
                return this._pixelsPerUnit;
            }

            set {
                if (value < 1) {
                    throw new ArgumentOutOfRangeException($"{nameof(this.PixelsPerUnit)} must be greater than 0!");
                }

                this._pixelsPerUnit = value;
                this.InversePixelsPerUnit = 1f / this._pixelsPerUnit;
            }
        }

        /// <inheritdoc />
        [DataMember]
        public string ProjectName { get; set; } = "Project Name";

        /// <inheritdoc />
        [DataMember]
        public Guid StartupSceneAssetId { get; set; }

        /// <summary>
        /// Adds the custom setting.
        /// </summary>
        /// <param name="settingName">Name of the setting.</param>
        /// <param name="settingValue">The setting value.</param>
        public void AddCustomSetting(string settingName, string settingValue) {
            this._customSettings.Add(settingName, settingValue);
        }

        /// <inheritdoc />
        public float GetPixelAgnosticRatio(float unitViewHeight, int pixelViewHeight) {
            return unitViewHeight * ((float)this.PixelsPerUnit / pixelViewHeight);
        }

        /// <inheritdoc />
        public bool TryGetCustomSetting(string settingName, out string settingValue) {
            return this._customSettings.TryGetValue(settingName, out settingValue);
        }
    }
}