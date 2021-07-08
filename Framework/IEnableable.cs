namespace Macabresoft.Macabre2D.Framework {

    /// <summary>
    /// Interface for an entity that can be enabled.
    /// </summary>
    public interface IEnableable {

        /// <summary>
        /// Gets a value indicating whether this instance is enabled.
        /// </summary>
        /// <value><c>true</c> if enabled; otherwise, <c>false</c>.</value>
        bool IsEnabled { get; set; }
    }
}