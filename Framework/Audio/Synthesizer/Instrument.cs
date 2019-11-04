namespace Macabre2D.Framework {

    using System.Runtime.Serialization;

    /// <summary>
    /// An instrument that can be played by a synthesizer within Macabre2D.
    /// </summary>
    [DataContract]
    public sealed class Instrument {
        private IOscillator _oscillator;

        /// <summary>
        /// Gets or sets the note envelope.
        /// </summary>
        /// <value>The note envelope.</value>
        [DataMember]
        public Envelope NoteEnvelope { get; } = new Envelope();

        /// <summary>
        /// Gets or sets the oscillator.
        /// </summary>
        /// <value>The oscillator.</value>
        [DataMember]
        public IOscillator Oscillator {
            get {
                return this._oscillator;
            }

            set {
                if (value != null) {
                    this._oscillator = value;
                }
                else {
                    this._oscillator = new SineWaveOscillator();
                }
            }
        }

        /// <summary>
        /// Gets or sets the pitch.
        /// </summary>
        /// <value>The pitch.</value>
        [DataMember]
        public MusicalPitch Pitch { get; set; } = MusicalPitch.Normal;
    }
}