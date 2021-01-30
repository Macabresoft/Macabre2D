namespace Macabresoft.Macabre2D.Framework {

    /// <summary>
    /// A <see cref="IGameComponent" /> which implements <see cref="IGameUpdateable" />, <see
    /// cref="IEnableable" />, and can be sorted.
    /// </summary>
    public interface IGameUpdateableComponent : IGameComponent, IGameUpdateable, IEnableable {

        /// <summary>
        /// Gets the update order.
        /// </summary>
        /// <value>The update order.</value>
        int UpdateOrder { get => 0; }
    }
}