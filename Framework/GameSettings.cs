namespace Macabre2D.Framework {

    using Microsoft.Xna.Framework;
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// A place for common game settings to be serialized across scenes.
    /// </summary>
    [DataContract]
    public sealed class GameSettings {

        /// <summary>
        /// The name of the content file for <see cref="GameSettings"/>. This is the same for every
        /// project and there is only one.
        /// </summary>
        internal const string ContentFileName = "Settings";

        [DataMember]
        private readonly Dictionary<string, string> _customSettings = new Dictionary<string, string>();

        private int _pixelsPerUnit = 32;

        /// <summary>
        /// Gets or sets the color of the game background when there is no scene opened.
        /// </summary>
        /// <value>The fallback background color.</value>
        [DataMember]
        public Color FallbackBackgroundColor { get; set; } = Color.HotPink;

        /// <summary>
        /// Gets or sets the pixels per unit. This value is the number of pixels per abritrary
        /// Macabre2D units.
        /// </summary>
        /// <value>The pixel density.</value>
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

        /// <summary>
        /// Gets the startup scene path.
        /// </summary>
        /// <value>The startup scene path.</value>
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

        /// <summary>
        /// Tries to get custom setting.
        /// </summary>
        /// <param name="settingName">Name of the setting.</param>
        /// <param name="settingValue">The setting value.</param>
        /// <returns>A value indicating whether or not the custom setting was found.</returns>
        public bool TryGetCustomSetting(string settingName, out string settingValue) {
            return this._customSettings.TryGetValue(settingName, out settingValue);
        }
    }
}