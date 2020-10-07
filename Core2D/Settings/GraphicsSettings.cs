namespace Macabresoft.MonoGame.Core2D {

    using Macabresoft.Core;
    using Microsoft.Xna.Framework;
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// The display mode.
    /// </summary>
    public enum DisplayModes : byte {
        Borderless = 0,
        Fullscreen = 1,
        Windowed = 2
    }

    /// <summary>
    /// Graphics settings such as resolution and display mode.
    /// </summary>
    [DataContract]
    public sealed class GraphicsSettings : VersionedData {

        /// <summary>
        /// The settings file name.
        /// </summary>
        public const string SettingsFileName = "Graphics.m2d";

        [DataMember]
        private DisplayModes _displayMode = DisplayModes.Windowed;

        [DataMember]
        private Point _resolution = new Point(1920, 1080);

        /// <summary>
        /// Initializes a new instance of the <see cref="GraphicsSettings" /> class.
        /// </summary>
        public GraphicsSettings() {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GraphicsSettings" /> class.
        /// </summary>
        /// <param name="displayMode">The display mode.</param>
        /// <param name="resolution">The resolution.</param>
        public GraphicsSettings(DisplayModes displayMode, Point resolution) {
            this._displayMode = displayMode;
            this._resolution = resolution;
        }

        /// <summary>
        /// Occurs when either <see cref="DisplayMode" /> or <see cref="Resolution" /> has changed.
        /// </summary>
        public event EventHandler? SettingsChanged;

        /// <summary>
        /// Gets or sets the display mode.
        /// </summary>
        /// <value>The display mode.</value>
        public DisplayModes DisplayMode {
            get {
                return this._displayMode;
            }

            set {
                if (this._displayMode != value) {
                    this._displayMode = value;
                    this.SettingsChanged.SafeInvoke(this);
                }
            }
        }

        /// <summary>
        /// Gets or sets the resolution.
        /// </summary>
        /// <value>The resolution.</value>
        public Point Resolution {
            get {
                return this._resolution;
            }

            set {
                if (this._resolution != value) {
                    this._resolution = value;
                    this.SettingsChanged.SafeInvoke(this);
                }
            }
        }

        public GraphicsSettings Clone() {
            return new GraphicsSettings(this.DisplayMode, this.Resolution);
        }
    }
}