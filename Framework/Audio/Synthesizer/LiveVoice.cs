namespace Macabre2D.Framework {

    public sealed class LiveVoice : Voice {
        private bool _isPlaying = false;
        private int _samplesBeforeRelease = -1;

        public Frequency Frequency {
            get {
                return this.Note.StartFrequency;
            }
        }

        public ushort SamplesPerBuffer { get; set; } = 1000;

        public override void Reinitialize(Song song, Track track, NoteInstance note) {
            base.Reinitialize(song, track, note);

            this._isPlaying = true;
            this._samplesBeforeRelease = -1;
        }

        public void Stop() {
            this._isPlaying = false;
            this._samplesBeforeRelease = -1;
        }

        protected override int GetNoteLengthInSamples() {
            return this._samplesBeforeRelease;
        }

        protected override ushort GetSamplesPerBuffer() {
            return this.SamplesPerBuffer;
        }

        protected override bool IsNoteOver(int sampleNumber) {
            var result = !this._isPlaying;
            if (result && this._samplesBeforeRelease < 0) {
                this._samplesBeforeRelease = this._samplesBeforeRelease > 0 ? this._samplesBeforeRelease : sampleNumber;
            }

            return result;
        }

        protected override bool IsNoteReleasing(int sampleNumber) {
            return sampleNumber - this._samplesBeforeRelease < this.Envelope.Release;
        }
    }
}