namespace Macabre2D.Framework {

    /// <summary>
    /// An automatic tile set that takes in up to eight directions to determine a sprite.
    /// </summary>
    public sealed class EightWayAutoTileSet : BaseAutoTileSet {
        private const byte ArrayLength = 48;

        /// <inheritdoc/>
        public override byte Size {
            get {
                return EightWayAutoTileSet.ArrayLength;
            }
        }

        /// <inheritdoc/>
        public override bool UseIntermediateDirections {
            get {
                return true;
            }
        }
    }
}