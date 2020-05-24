namespace Macabre2D.Framework {

    using Microsoft.Xna.Framework;
    using System.ComponentModel;

    /// <summary>
    /// A clickable component that is a child of a <see cref="PianoComponent"/>.
    /// </summary>
    public interface IClickablePianoComponent : INotifyPropertyChanged {

        /// <summary>
        /// Gets a value indicating whether this instance is clickable.
        /// </summary>
        /// <value><c>true</c> if this instance is clickable; otherwise, <c>false</c>.</value>
        bool IsClickable { get; }

        /// <summary>
        /// Gets the priority.
        /// </summary>
        /// <value>The priority.</value>
        int Priority { get; }

        /// <summary>
        /// Ends the click.
        /// </summary>
        void EndClick();

        /// <summary>
        /// Tries the click.
        /// </summary>
        /// <param name="mouseWorldPosition">The mouse world position.</param>
        /// <returns>A value indicating whether or not the click was successful.</returns>
        bool TryClick(Vector2 mouseWorldPosition);

        /// <summary>
        /// Tries the hold click.
        /// </summary>
        /// <param name="mouseWorldPosition">The mouse world position.</param>
        /// <returns>A value indicating whether or not the click was held.</returns>
        bool TryHoldClick(Vector2 mouseWorldPosition);
    }
}