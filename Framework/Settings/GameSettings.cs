namespace Macabresoft.Macabre2D.Framework {
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.Serialization;
    using Microsoft.Xna.Framework;
    using Newtonsoft.Json;

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
        /// Gets the inverse of <see cref="PixelsPerUnit" />.
        /// </summary>
        /// <value>
        /// <remarks>
        /// This will be calculated when <see cref="PixelsPerUnit" /> is set.
        /// Multiplication is a quicker operation than division, so if you find yourself dividing by
        /// <see cref="PixelsPerUnit" /> regularly, consider multiplying by this instead as it will
        /// produce the same value, but quicker.
        /// </remarks>
        /// The inverse pixels per unit.
        /// </value>
        float InversePixelsPerUnit { get; }

        /// <summary>
        /// Gets the layer settings.
        /// </summary>
        /// <value>The layer settings.</value>
        LayerSettings Layers { get; }

        /// <summary>
        /// Gets or sets the color that sprites will be filled in with if their content cannot be loaded.
        /// </summary>
        Color ErrorSpritesColor { get; set; }

        /// <summary>
        /// Gets or sets the color of the game background when there is no scene opened.
        /// </summary>
        /// <value>The fallback background color.</value>
        Color FallbackBackgroundColor { get; set; }

        /// <summary>
        /// Gets or sets the pixels per unit. This value is the number of pixels per abritrary game units.
        /// </summary>
        /// <value>The pixel density.</value>
        ushort PixelsPerUnit { get; set; }

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
        bool TryGetCustomSetting(string settingName, out string? settingValue);
    }

    /// <summary>
    /// A place for common game settings to be serialized across scenes.
    /// </summary>
    [DataContract]
    [Category(CommonCategories.Settings)]
    public sealed class GameSettings : IGameSettings {
        /// <summary>
        /// The content file name for <see cref="GameSettings" />.
        /// </summary>
        public const string ContentFileName = "Settings";

        [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Replace)]
        private readonly Dictionary<string, string> _customSettings = new();

        private ushort _pixelsPerUnit = 32;

        /// <inheritdoc />
        [DataMember]
        public GraphicsSettings DefaultGraphicsSettings { get; } = new();

        /// <inheritdoc />
        [DataMember]
        public LayerSettings Layers { get; } = new();

        /// <inheritdoc />
        [DataMember]
        public Point DefaultResolution { get; set; }

        /// <inheritdoc />
        [DataMember]
        [Category(CommonCategories.Fallback)]
        public Color ErrorSpritesColor { get; set; } = Color.HotPink;

        /// <inheritdoc />
        [DataMember]
        [Category(CommonCategories.Fallback)]
        public Color FallbackBackgroundColor { get; set; } = Color.Black;

        /// <inheritdoc />
        public float InversePixelsPerUnit { get; private set; } = 1f / 32f;

        /// <inheritdoc />
        [DataMember]
        public ushort PixelsPerUnit {
            get => this._pixelsPerUnit;

            set {
                if (value < 1) {
                    throw new ArgumentOutOfRangeException($"{nameof(this.PixelsPerUnit)} must be greater than 0!");
                }

                this._pixelsPerUnit = value;
                this.InversePixelsPerUnit = 1f / this._pixelsPerUnit;
            }
        }

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
        public bool TryGetCustomSetting(string settingName, out string? settingValue) {
            return this._customSettings.TryGetValue(settingName, out settingValue);
        }
    }
}