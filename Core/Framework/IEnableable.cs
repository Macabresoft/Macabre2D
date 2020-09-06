namespace Macabresoft.MonoGame.Core {

    /// <summary>
    /// Interface for a component that can be enabled.
    /// </summary>
    public interface IEnableable {

        /// <summary>
        /// Gets a value indicating whether this instance is enabled.
        /// </summary>
        /// <value><c>true</c> if enabled; otherwise, <c>false</c>.</value>
        bool IsEnabled { get; }
    }
}