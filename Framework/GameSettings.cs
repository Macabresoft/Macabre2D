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
        /// Gets the color of the game background when there is no scene opened.
        /// </summary>
        /// <value>The fallback background color.</value>
        Color FallbackBackgroundColor { get; }

        /// <summary>
        /// Getsthe pixels per unit. This value is the number of pixels per abritrary Macabre2D units.
        /// </summary>
        /// <value>The pixel density.</value>
        int PixelsPerUnit { get; }

        /// <summary>
        /// Adds the custom setting.
        /// </summary>
        /// <param name="settingName">Name of the setting.</param>
        /// <param name="settingValue">The setting value.</param>
        string StartupScenePath { get; }

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
        /// The name of the content file for <see cref="GameSettings"/>. This is the same for every
        /// project and there is only one.
        /// </summary>
        internal const string ContentFileName = "Settings";

        private static IGameSettings _settings = new GameSettings();

        [DataMember]
        private readonly Dictionary<string, string> _customSettings = new Dictionary<string, string>();

        private int _pixelsPerUnit = 32;

        /// <summary>
        /// The singleton instance of game settings.
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
                return GameSettings._settings;
            }
            set {
                if (value != null) {
                    GameSettings._settings = value;
                }
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public Color FallbackBackgroundColor { get; set; } = Color.HotPink;

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
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public string StartupScenePath { get; internal set; }

        /// <summary>
        /// Adds the custom setting.
        /// </summary>
        /// <param name="settingName">Name of the setting.</param>
        /// <param name="settingValue">The setting value.</param>
        public void AddCustomSetting(string settingName, string settingValue) {
            this._customSettings.Add(settingName, settingValue);
        }

        /// <inheritdoc/>
        public float GetPixelAgnosticRatio(float unitViewHeight, int pixelViewHeight) {
            return unitViewHeight * ((float)this.PixelsPerUnit / pixelViewHeight);
        }

        /// <inheritdoc/>
        public bool TryGetCustomSetting(string settingName, out string settingValue) {
            return this._customSettings.TryGetValue(settingName, out settingValue);
        }
    }
}