namespace Macabre2D.Framework {

    using Microsoft.Xna.Framework;
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
    public sealed class GraphicsSettings {

        /// <summary>
        /// Gets or sets the display mode.
        /// </summary>
        /// <value>The display mode.</value>
        [DataMember]
        public DisplayModes DisplayMode { get; set; } = DisplayModes.Borderless;

        /// <summary>
        /// Gets or sets the resolution.
        /// </summary>
        /// <value>The resolution.</value>
        [DataMember]
        public Point Resolution { get; set; } = new Point(1920, 1080);
    }
}