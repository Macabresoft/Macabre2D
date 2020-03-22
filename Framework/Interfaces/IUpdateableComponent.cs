namespace Macabre2D.Framework {

    /// <summary>
    /// An updateable component.
    /// </summary>
    public interface IUpdateableComponent : IBaseComponent, IEnableableComponent {

        /// <summary>
        /// Updates this instance.
        /// </summary>
        /// <param name="frameTime">The frame time.</param>
        void Update(FrameTime frameTime);
    }
}