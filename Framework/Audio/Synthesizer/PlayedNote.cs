namespace Macabre2D.Framework {

    using System.Runtime.Serialization;

    /// <summary>
    /// A standard note.
    /// </summary>
    [DataContract]
    public sealed class PlayedNote {
        private ushort _length = 1;

        /// <summary>
        /// Initializes a new instance of the <see cref="PlayedNote"/> class.
        /// </summary>
        /// <param name="beat">The beat.</param>
        /// <param name="length">The length.</param>
        /// <param name="note">The note.</param>
        public PlayedNote(ushort beat, ushort length, MusicalNote note) {
            this.Beat = beat;
            this.Length = length;
            this.Note = note;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PlayedNote"/> class.
        /// </summary>
        internal PlayedNote() {
        }

        /// <summary>
        /// Gets or sets the beat.
        /// </summary>
        /// <value>The beat.</value>
        [DataMember]
        public ushort Beat { get; set; }

        // <inheritdoc/>
        [DataMember]
        public ushort Length {
            get {
                return this._length;
            }

            private set {
                if (value == 0) {
                    value = 1;
                }

                this._length = value;
            }
        }

        /// <summary>
        /// Gets or sets the note.
        /// </summary>
        /// <value>The note.</value>
        [DataMember]
        public MusicalNote Note { get; set; }
    }
}