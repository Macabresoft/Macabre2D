namespace Macabre2D.Framework.Audio.Synthesizer {

    using System;

    /// <summary>
    /// A synthesizer voice. These are pooled and used to play notes to completion.
    /// </summary>
    public sealed class Voice {
        private Instrument _instrument;
        private SynthNote _note;

        /// <summary>
        /// Occurs when the note is finished.
        /// </summary>
        public event EventHandler OnFinished;

        /// <summary>
        /// Reinitializes the specified instrument.
        /// </summary>
        /// <param name="instrument">The instrument.</param>
        /// <param name="note">The note.</param>
        public void Reinitialize(Instrument instrument, SynthNote note) {
            this._instrument = instrument;
            this._note = note;
        }
    }
}