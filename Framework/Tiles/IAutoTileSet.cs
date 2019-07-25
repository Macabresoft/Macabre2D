namespace Macabre2D.Framework {

    using System;

    /// <summary>
    /// An interface for automatic tile sets.
    /// </summary>
    public interface IAutoTileSet {

        /// <summary>
        /// Occers when the sprite has changed for a specific index on this tileset.
        /// </summary>
        event EventHandler<AutoTileSetSpriteChangedEventArgs> SpriteChanged;

        /// <summary>
        /// Gets the size of the set.
        /// </summary>
        /// <value>The size.</value>
        byte Size { get; }

        /// <summary>
        /// Gets the sprite at the specific index of the auto tile set's sprite collection.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        Sprite GetSprite(byte index);

        /// <summary>
        /// Sets the sprite at the specific index of the auto tile set's sprite collection.
        /// </summary>
        /// <param name="sprite">The sprite.</param>
        /// <param name="index">The index.</param>
        void SetSprite(Sprite sprite, byte index);
    }
}