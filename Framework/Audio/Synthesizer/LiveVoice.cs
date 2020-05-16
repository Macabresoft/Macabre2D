namespace Macabre2D.Framework {

    public sealed class LiveVoice : Voice {
        private bool _isPlaying = false;
        private int _releaseSamplesPlayed = -1;

        public ushort SamplesPerBuffer { get; set; } = 1000;

        public override void Reinitialize(Song song, Track track, NoteInstance note) {
            base.Reinitialize(song, track, note);

            this._isPlaying = true;
            this._releaseSamplesPlayed = -1;
        }

        public void Stop() {
            this._isPlaying = false;
            this._releaseSamplesPlayed = -1;
        }

        protected override ushort GetSamplesPerBuffer() {
            return this.SamplesPerBuffer;
        }

        protected override bool IsNoteOver(int sampleNumber) {
            return !this._isPlaying;
        }

        protected override bool IsNoteReleasing(int sampleNumber) {
            this._releaseSamplesPlayed++;
            return this._releaseSamplesPlayed < this.Envelope.Release;
        }
    }
}