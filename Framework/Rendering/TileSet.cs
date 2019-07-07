namespace Macabre2D.Framework {

    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// A tile set created from sprites.
    /// </summary>
    /// <seealso cref="Macabre2D.Framework.IIdentifiable"/>
    [DataContract]
    public class TileSet : IIdentifiable {

        [DataMember]
        private Guid _id = Guid.NewGuid();

        [DataMember]
        private Sprite[,] _sprites = new Sprite[3, 3];

        /// <summary>
        /// Gets the columns.
        /// </summary>
        /// <value>The columns.</value>
        public int Columns {
            get {
                return this._sprites.GetLength(0);
            }
        }

        /// <inheritdoc/>
        public Guid Id {
            get {
                return this._id;
            }
        }

        /// <summary>
        /// Gets the rows.
        /// </summary>
        /// <value>The rows.</value>
        public int Rows {
            get {
                return this._sprites.GetLength(1);
            }
        }

        internal Sprite[,] Sprites {
            get {
                return _sprites;
            }
        }

        /// <summary>
        /// Sets the sprites.
        /// </summary>
        /// <param name="sprites">The sprites.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The array for sprites should have at least one column and one row.
        /// </exception>
        public void SetSprites(Sprite[,] sprites) {
            if (sprites == null) {
                throw new ArgumentNullException(nameof(sprites));
            }

            var width = sprites.GetLength(0);
            var height = sprites.GetLength(1);

            if (width < 1 || height < 1) {
                throw new ArgumentOutOfRangeException(nameof(sprites));
            }

            this._sprites = sprites;
        }
    }
}