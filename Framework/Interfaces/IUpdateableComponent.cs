namespace Macabre2D.Framework {

    using Microsoft.Xna.Framework;

    /// <summary>
    /// An updateable component.
    /// </summary>
    public interface IUpdateableComponent : IBaseComponent, IEnableableComponent {

        /// <summary>
        /// Updates this instance.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        void Update(GameTime gameTime);
    }
}