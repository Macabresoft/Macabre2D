namespace Macabre2D.Framework.Rendering {

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

            internal set {
                if (this.Columns != value && value > 0) {
                    this.ResizeArray(value, this.Rows);
                }
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

            internal set {
                if (this.Rows != value && value > 0) {
                    this.ResizeArray(this.Columns, value);
                }
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
            var width = sprites.GetLength(0);
            var height = sprites.GetLength(1);

            if (width < 1 || height < 1) {
                throw new ArgumentOutOfRangeException(nameof(sprites));
            }

            this._sprites = sprites;
        }

        private void ResizeArray(int columns, int rows) {
            var originalColumns = this._sprites.GetLength(0);
            var originalRows = this._sprites.GetLength(1);
            var newArray = new Sprite[columns, rows];

            for (var x = 0; x < columns; x++) {
                for (var y = 0; y < rows; y++) {
                    if (x < originalColumns && y < originalRows) {
                        newArray[x, y] = this._sprites[x, y];
                    }
                    else {
                        newArray[x, y] = null;
                    }
                }
            }
        }
    }
}