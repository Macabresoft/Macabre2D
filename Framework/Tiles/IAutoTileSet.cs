namespace Macabre2D.Framework {

    using System;

    /// <summary>
    /// A tile set that can be auto-generated and edited at runtime.
    /// </summary>
    public interface IAutoTileSet : IIdentifiable {

        /// <summary>
        /// Occurs when the sprite changes for a set of relative <see cref="CardinalDirection"/>.
        /// </summary>
        event EventHandler<CardinalDirection> SpriteChanged;

        /// <summary>
        /// Gets the appropriate sprite for the current tile.
        /// </summary>
        /// <param name="relativeDirectionOfActiveSprites">The relative direction of active sprites.</param>
        /// <returns>The sprite.</returns>
        Sprite GetSprite(CardinalDirection relativeDirectionOfActiveSprites);

        /// <summary>
        /// Sets the sprite.
        /// </summary>
        /// <param name="sprite">The sprite.</param>
        /// <param name="relativeDirectionOfActiveSprites">The relative direction of active sprites.</param>
        void SetSprite(Sprite sprite, CardinalDirection relativeDirectionOfActiveSprites);
    }
}