namespace Macabre2D.Framework {

    /// <summary>
    /// Interface for a component that can be enabled.
    /// </summary>
    public interface IEnableableComponent {

        /// <summary>
        /// Gets a value indicating whether this <see cref="IUpdateableComponent"/> is enabled.
        /// </summary>
        /// <value><c>true</c> if enabled; otherwise, <c>false</c>.</value>
        bool IsEnabled { get; }
    }
}