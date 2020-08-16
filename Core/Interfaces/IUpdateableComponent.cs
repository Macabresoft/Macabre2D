namespace Macabresoft.MonoGame.Core {

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
        /// <param name="inputState">State of the input.</param>
        void Update(FrameTime frameTime, InputState inputState);
    }
}