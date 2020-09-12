namespace Macabresoft.MonoGame.Core {

    /// <summary>
    /// A dynamic <see cref="IGameUpdateable" /> that also implements <see cref="IEnableable" /> and
    /// can be sorted.
    /// </summary>
    public interface IDynamicGameUpdateable : IGameUpdateable, IEnableable {

        /// <summary>
        /// Gets the update order.
        /// </summary>
        /// <value>The update order.</value>
        int UpdateOrder { get => 0; }
    }
}