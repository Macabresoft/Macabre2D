namespace Macabre2D.Framework {

    using System;

    /// <summary>
    /// Event args for when an automatic tileset has changed a sprite at a specific index.
    /// </summary>
    /// <seealso cref="System.EventArgs"/>
    public sealed class AutoTileSetSpriteChangedEventArgs : EventArgs {

        /// <summary>
        /// Initializes a new instance of the <see cref="AutoTileSetSpriteChangedEventArgs"/> class.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="sprite">The sprite.</param>
        public AutoTileSetSpriteChangedEventArgs(byte index, Sprite sprite) {
            this.Index = index;
            this.Sprite = sprite;
        }

        /// <summary>
        /// Gets the index.
        /// </summary>
        /// <value>The index.</value>
        public byte Index { get; }

        /// <summary>
        /// Gets the sprite.
        /// </summary>
        /// <value>The sprite.</value>
        public Sprite Sprite { get; }
    }
}