namespace Macabre2D.Framework {

    /// <summary>
    /// An instrument that can be played by a synthesizer within Macabre2D.
    /// </summary>
    public sealed class Instrument {

        /// <summary>
        /// Gets or sets the oscillator.
        /// </summary>
        /// <value>The oscillator.</value>
        public IOscillator Oscillator { get; set; }

        // Attack, Decay Sustain, Release, etc
    }
}