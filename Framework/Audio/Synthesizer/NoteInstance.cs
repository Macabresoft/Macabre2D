namespace Macabre2D.Framework {

    using System.Runtime.Serialization;

    /// <summary>
    /// An instance of a note on the piano roll.
    /// </summary>
    [DataContract]
    public sealed class NoteInstance {
        private ushort _length = 1;

        /// <summary>
        /// Initializes a new instance of the <see cref="NoteInstance"/> class.
        /// </summary>
        /// <param name="beat">The beat.</param>
        /// <param name="length">The length.</param>
        /// <param name="frequency">The frequency.</param>
        public NoteInstance(ushort beat, ushort length, Frequency frequency) {
            this.Beat = beat;
            this.Length = length;
            this.Frequency = frequency;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NoteInstance"/> class.
        /// </summary>
        internal NoteInstance() {
        }

        /// <summary>
        /// Gets or sets the beat.
        /// </summary>
        /// <value>The beat.</value>
        [DataMember]
        public ushort Beat { get; set; }

        /// <summary>
        /// Gets or sets the frequency.
        /// </summary>
        /// <value>The frequency.</value>
        [DataMember]
        public Frequency Frequency { get; set; }

        /// <summary>
        /// Gets the length.
        /// </summary>
        /// <value>The length.</value>
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
    }
}