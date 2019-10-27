namespace Macabre2D.Framework {

    using System.Runtime.Serialization;

    /// <summary>
    /// An instrument that can be played by a synthesizer within Macabre2D.
    /// </summary>
    [DataContract]
    public sealed class Instrument {

        /// <summary>
        /// Gets or sets the note envelope.
        /// </summary>
        /// <value>The note envelope.</value>
        [DataMember]
        public Envelope NoteEnvelope { get; set; }

        /// <summary>
        /// Gets or sets the oscillator.
        /// </summary>
        /// <value>The oscillator.</value>
        [DataMember]
        public IOscillator Oscillator { get; set; }
    }
}