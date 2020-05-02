namespace Macabre2D.Framework {

    /// <summary>
    /// An updateable component.
    /// </summary>
    public interface IUpdateableComponent : IBaseComponent, IEnableableComponent {

        /// <summary>
        /// Gets the update order.
        /// </summary>
        /// <value>The update order.</value>
        int UpdateOrder { get; }

        /// <summary>
        /// Updates this instance.
        /// </summary>
        /// <param name="frameTime">The frame time.</param>
        void Update(FrameTime frameTime);
    }
}