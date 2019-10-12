namespace Macabre2D.Framework {

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
        internal const string SettingsFileName = "Graphics.m2d";

        [DataMember]
        private DisplayModes _displayMode = DisplayModes.Windowed;

        [DataMember]
        private Point _resolution = new Point(1920, 1080);

        /// <summary>
        /// Occurs when either <see cref="DisplayMode"/> or <see cref="Resolution"/> has changed.
        /// </summary>
        public event EventHandler SettingsChanged;

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
    }
}