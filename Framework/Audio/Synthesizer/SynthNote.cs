namespace Macabre2D.Framework {

    using System.Runtime.Serialization;

    /// <summary>
    /// A standard note.
    /// </summary>
    [DataContract]
    public sealed class SynthNote {
        private ushort _length = 1;

        /// <summary>
        /// Initializes a new instance of the <see cref="SynthNote"/> class.
        /// </summary>
        /// <param name="beat">The beat.</param>
        /// <param name="frequency">The frequency.</param>
        public SynthNote(ushort beat, Frequency frequency) {
            this.Beat = beat;
            this.Frequency = frequency;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SynthNote"/> class.
        /// </summary>
        internal SynthNote() {
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
    }
}