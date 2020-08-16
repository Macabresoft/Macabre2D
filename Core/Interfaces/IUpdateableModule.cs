namespace Macabresoft.MonoGame.Core {

    using System.ComponentModel;

    /// <summary>
    /// Represents a module that can be updated before and after all components update.
    /// </summary>
    public interface IUpdateableModule : INotifyPropertyChanged {

        /// <summary>
        /// Gets or sets a value indicating whether this instance is enabled.
        /// </summary>
        /// <value><c>true</c> if this instance is enabled; otherwise, <c>false</c>.</value>
        bool IsEnabled { get; }

        /// <summary>
        /// Gets the update order.
        /// </summary>
        /// <value>The update order.</value>
        int UpdateOrder { get; }

        /// <summary>
        /// Updates after the normal update occurs for a scene.
        /// </summary>
        /// <param name="frameTime">The frame time.</param>
        /// <param name="inputState">State of the input.</param>
        void PostUpdate(FrameTime frameTime, InputState inputState);

        /// <summary>
        /// Updates before the normal update for a scene.
        /// </summary>
        /// <param name="frameTime">The frame time.</param>
        /// <param name="inputState">State of the input.</param>
        void PreUpdate(FrameTime frameTime, InputState inputState);
    }
}