namespace Macabresoft.MonoGame.Core {

    using System.ComponentModel;

    /// <summary>
    /// Interface for an updateable descendent of <see cref="IGameLoop" />.
    /// </summary>
    public interface IGameUpdateable : INotifyPropertyChanged {

        /// <summary>
        /// Updates this instance.
        /// </summary>
        /// <param name="frameTime">The frame time.</param>
        /// <param name="inputState">State of the input.</param>
        void Update(FrameTime frameTime, InputState inputState);
    }
}