namespace Macabresoft.MonoGame.Core {

    /// <summary>
    /// A <see cref="IGameComponent" /> that also implements <see cref="IGameUpdateable" />.
    /// </summary>
    public interface IGameUpdateableComponent : IGameUpdateable, IGameComponent {

        /// <summary>
        /// Gets a value indicating whether this instance is enabled.
        /// </summary>
        /// <value><c>true</c> if this instance is enabled; otherwise, <c>false</c>.</value>
        bool IsEnabled { get; }

        /// <summary>
        /// Gets the update order.
        /// </summary>
        /// <value>The update order.</value>
        int UpdateOrder { get; }
    }
}