namespace Macabresoft.Macabre2D.Framework {

    using System.ComponentModel;

    /// <summary>
    /// Interface for an updateable object in the game loop.
    /// </summary>
    public interface IGameUpdateable : INotifyPropertyChanged, IEnableable {

        /// <summary>
        /// Updates this instance.
        /// </summary>
        /// <param name="frameTime">The frame time.</param>
        /// <param name="inputState">State of the input.</param>
        void Update(FrameTime frameTime, InputState inputState);
    }
}