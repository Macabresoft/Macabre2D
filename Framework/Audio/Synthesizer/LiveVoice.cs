namespace Macabre2D.Framework {

    /// <summary>
    /// A live voice that isn't necesarrily tied to a real note instance.
    /// </summary>
    public sealed class LiveVoice : Voice {
        private bool _isPlaying = false;
        private ulong _samplesBeforeRelease = 0;

        /// <summary>
        /// Gets the frequency.
        /// </summary>
        /// <value>The frequency.</value>
        public Frequency Frequency {
            get {
                return this.Note.StartFrequency;
            }
        }

        /// <inheritdoc/>
        public override void Reinitialize(Song song, Track track, NoteInstance note, float startingBeat = 0f) {
            base.Reinitialize(song, track, note, startingBeat);

            this._isPlaying = true;
            this._samplesBeforeRelease = 0;
        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        public void Stop() {
            this._isPlaying = false;
            this._samplesBeforeRelease = 0;
        }

        protected override ulong GetNoteLengthInSamples() {
            return this._samplesBeforeRelease;
        }

        protected override bool IsNoteOver(ulong sampleNumber) {
            var result = !this._isPlaying;
            if (result && this._samplesBeforeRelease == 0) {
                this._samplesBeforeRelease = this._samplesBeforeRelease > 0 ? this._samplesBeforeRelease : sampleNumber;
            }

            return result;
        }

        protected override bool IsNoteReleasing(ulong sampleNumber) {
            return sampleNumber - this._samplesBeforeRelease < this.Envelope.Release;
        }
    }
}