namespace Macabre2D.Framework {

    using System.Threading.Tasks;

    /// <summary>
    /// An interface for a game component that can update on a background thread.
    /// </summary>
    public interface IUpdateableComponentAsync : IBaseComponent, IEnableableComponent {

        /// <summary>
        /// Updates this instance asynchronously.
        /// </summary>
        /// <param name="frameTime">The frame time.</param>
        /// <returns>The task.</returns>
        Task UpdateAsync(FrameTime frameTime);
    }
}