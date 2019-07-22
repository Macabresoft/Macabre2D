namespace Macabre2D.Framework {

    /// <summary>
    /// A tile set that can be auto-generated and edited at runtime.
    /// </summary>
    public interface IAutoTileSet : IIdentifiable {

        /// <summary>
        /// Gets the appropriate sprite for the current tile.
        /// </summary>
        /// <param name="directionsOfActiveSprites">
        /// The directions of active sprites relatively to the requesting tile.
        /// </param>
        /// <returns>The sprite.</returns>
        Sprite GetSprite(CardinalDirection directionsOfActiveSprites);
    }
}