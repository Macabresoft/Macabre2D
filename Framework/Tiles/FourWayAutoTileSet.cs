namespace Macabre2D.Framework {

    /// <summary>
    /// An automatic tile set that takes in up to four directions to determine a sprite.
    /// </summary>
    public sealed class FourWayAutoTileSet : BaseAutoTileSet {
        private const byte ArrayLength = 16;

        /// <inheritdoc/>
        public override byte Size {
            get {
                return FourWayAutoTileSet.ArrayLength;
            }
        }

        /// <inheritdoc/>
        public override bool UseIntermediateDirections {
            get {
                return false;
            }
        }
    }
}