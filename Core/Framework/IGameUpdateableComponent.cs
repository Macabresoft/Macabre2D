namespace Macabresoft.MonoGame.Core {

    /// <summary>
    /// A <see cref="IGameComponent" /> that also implements <see cref="IGameUpdateable" />.
    /// </summary>
    public interface IGameUpdateableComponent : IGameUpdateable, IGameComponent, IEnableable {

        /// <summary>
        /// Gets the update order.
        /// </summary>
        /// <value>The update order.</value>
        int UpdateOrder { get; }
    }
}