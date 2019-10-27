namespace Macabre2D.Framework {

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;

    /// <summary>
    /// A track that is played by the synthesizer.
    /// </summary>
    [DataContract]
    public sealed class Track {
        private readonly List<SynthNote> _activeNotes = new List<SynthNote>();
        private readonly List<SynthNote> _notes = new List<SynthNote>();

        /// <summary>
        /// Gets the instrument.
        /// </summary>
        /// <value>The instrument.</value>
        [DataMember]
        public Instrument Instrument { get; }

        /// <summary>
        /// Gets the length.
        /// </summary>
        /// <value>The length.</value>
        public ushort Length { get; private set; }

        /// <summary>
        /// Adds the note.
        /// </summary>
        /// <param name="beat">The beat.</param>
        /// <param name="frequency">The frequency.</param>
        /// <returns>The added note.</returns>
        public SynthNote AddNote(ushort beat, Frequency frequency) {
            var note = new SynthNote(beat, frequency);
            this._notes.Add(note);
            return note;
        }

        public float[] GetSignals(ushort beat) {
            this._activeNotes.AddRange(this._notes.Where(x => x.Beat == beat));

            if (this._activeNotes.Any()) {
                var notesToRemove = new List<SynthNote>();
                foreach (var note in this._activeNotes) {
                    if (beat - note.Beat >= note.Length) {
                        notesToRemove.Add(note);
                    }
                }

                foreach (var note in notesToRemove) {
                    this._activeNotes.Remove(note);
                }
            }

            return Array.Empty<float>();
        }

        // some sort of chain of notes. Maybe it's just a queue? Get frequency from current note and
        // then pass it into the instrument to do its thing.
    }
}