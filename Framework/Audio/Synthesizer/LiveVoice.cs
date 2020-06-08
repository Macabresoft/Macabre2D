namespace Macabre2D.Framework {

    /// <summary>
    /// A live voice that isn't necesarrily tied to a real note instance.
    /// </summary>
    public sealed class LiveVoice : Voice {
        private bool _isPlaying = false;
        private float _millisecondsBeforeRelease = 0f;

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
            this._millisecondsBeforeRelease = 0;
        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        public void Stop() {
            this._isPlaying = false;
            this._millisecondsBeforeRelease = 0;
        }

        protected override float GetNoteLengthInMilliseconds() {
            return this._millisecondsBeforeRelease;
        }

        protected override bool IsNoteOver(float millisecondsIntoNote) {
            var result = !this._isPlaying;
            if (result && this._millisecondsBeforeRelease == 0f) {
                this._millisecondsBeforeRelease = this._millisecondsBeforeRelease > 0 ? this._millisecondsBeforeRelease : millisecondsIntoNote;
            }

            return result;
        }

        protected override bool IsNoteReleasing(float millisecondsIntoNote) {
            return millisecondsIntoNote - this._millisecondsBeforeRelease < this.Envelope.Release;
        }
    }
}