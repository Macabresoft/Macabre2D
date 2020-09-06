namespace Macabresoft.MonoGame.Core {

    using System.Threading.Tasks;

    /// <summary>
    /// An interface for a game component that can update on a background thread.
    /// </summary>
    public interface IUpdateableComponentAsync : IBaseComponent, IEnableable {

        /// <summary>
        /// Updates this instance asynchronously.
        /// </summary>
        /// <param name="frameTime">The frame time.</param>
        /// <param name="inputState">State of the input.</param>
        /// <returns>The task.</returns>
        Task UpdateAsync(FrameTime frameTime, InputState inputState);
    }
}