namespace Macabre2D.Framework {

    using Microsoft.Xna.Framework;
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// An interface to read common game settings.
    /// </summary>
    public interface IGameSettings {

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
        /// Gets the inverse of <see cref="PixelsPerUnit"/>.
        /// </summary>
        /// <value>
        /// <remarks>This will be calculated when <see cref="PixelsPerUnit"/> is set. Multiplication
        /// is a quicker operation than division, so if you find yourself dividing by <see
        /// cref="PixelsPerUnit"/> regularly, consider multiplying by this instead as it will produce
        /// the same value, but quicker.</remarks> The inverse pixels per unit.
        /// </value>
        float InversePixelsPerUnit { get; }

        /// <summary>
        /// Getsthe pixels per unit. This value is the number of pixels per abritrary Macabre2D units.
        /// </summary>
        /// <value>The pixel density.</value>
        int PixelsPerUnit { get; }

        /// <summary>
        /// Gets the startup scene asset identifier.
        /// </summary>
        /// <value>The startup scene asset identifier.</value>
        Guid StartupSceneAssetId { get; }

        /// <summary>
        /// Gets the name of the layer.
        /// </summary>
        /// <param name="layer">The layer.</param>
        /// <returns></returns>
        string GetLayerName(Layers layer);

        /// <summary>
        /// Gets a pixel agnostic ratio. This can be used to make something appear the same size on
        /// screen regardless of the current view size.
        /// </summary>
        /// <param name="unitViewHeight">Height of the unit view.</param>
        /// <param name="pixelViewHeight">Height of the pixel view.</param>
        /// <returns>A pixel agnostic ratio.</returns>
        float GetPixelAgnosticRatio(float unitViewHeight, int pixelViewHeight);

        /// <summary>
        /// Sets the name of the layer.
        /// </summary>
        /// <param name="layer">The layer.</param>
        /// <param name="name">The name.</param>
        void SetLayerName(Layers layer, string name);

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
        internal const string ContentFileName = "Settings";

        private static IGameSettings _instance = new GameSettings();

        [DataMember]
        private readonly Dictionary<string, string> _customSettings = new Dictionary<string, string>();

        [DataMember]
        private readonly Dictionary<Layers, string> _layersToName = new Dictionary<Layers, string>();

        private int _pixelsPerUnit = 32;

        /// <summary>
        /// Initializes the <see cref="GameSettings"/> class.
        /// </summary>
        static GameSettings() {
        }

        /// <summary>
        /// Gets the singleton instance of game settings.
        /// </summary>
        /// <remarks>
        /// Honestly, I'm not a huge fan of singletons. They feel like bad design to me. But there
        /// are objects in a tight loop, like <see cref="Sprite"/> that need access to some settings,
        /// but <see cref="Sprite"/> was written to not know a whole lot about anything else. I think
        /// it should stay that way. I created a readonly interface of <see cref="GameSettings"/>
        /// called <see cref="IGameSettings"/> to alleviate some of the grossness of this. The setter
        /// is internal, so only <see cref="MacabreGame"/> should ever set it.
        /// </remarks>
        public static IGameSettings Instance {
            get {
                return GameSettings._instance;
            }

            internal set {
                if (value != null) {
                    GameSettings._instance = value;
                }
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public Color ErrorSpritesColor { get; set; } = Color.HotPink;

        /// <inheritdoc/>
        [DataMember]
        public Color FallbackBackgroundColor { get; set; } = Color.Black;

        /// <inheritdoc/>
        public float InversePixelsPerUnit { get; private set; } = 1f / 32f;

        /// <inheritdoc/>
        [DataMember]
        public int PixelsPerUnit {
            get {
                return this._pixelsPerUnit;
            }

            internal set {
                if (value < 1) {
                    throw new ArgumentOutOfRangeException($"{nameof(this.PixelsPerUnit)} must be greater than 0!");
                }

                this._pixelsPerUnit = value;
                this.InversePixelsPerUnit = 1f / this._pixelsPerUnit;
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public Guid StartupSceneAssetId { get; internal set; }

        /// <summary>
        /// Adds the custom setting.
        /// </summary>
        /// <param name="settingName">Name of the setting.</param>
        /// <param name="settingValue">The setting value.</param>
        public void AddCustomSetting(string settingName, string settingValue) {
            this._customSettings.Add(settingName, settingValue);
        }

        /// <inheritdoc/>
        public string GetLayerName(Layers layer) {
            string name;
            if (!this._layersToName.TryGetValue(layer, out name)) {
                name = layer.ToString();
            }

            return name;
        }

        /// <inheritdoc/>
        public float GetPixelAgnosticRatio(float unitViewHeight, int pixelViewHeight) {
            return unitViewHeight * ((float)this.PixelsPerUnit / pixelViewHeight);
        }

        /// <inheritdoc/>
        public void SetLayerName(Layers layer, string name) {
            this._layersToName[layer] = name;
        }

        /// <inheritdoc/>
        public bool TryGetCustomSetting(string settingName, out string settingValue) {
            return this._customSettings.TryGetValue(settingName, out settingValue);
        }
    }
}