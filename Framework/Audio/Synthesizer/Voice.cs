namespace Macabre2D.Framework {

    using System;

    /// <summary>
    /// A synthesizer voice. These are pooled and used to play notes to completion.
    /// </summary>
    public class Voice : IDisposable, IVoice {
        private ushort _beatsPlayed;
        private float _inverseSampleRate;
        private bool _isActive;
        private bool _isDisposed = false;
        private NoteInstance _note;
        private int _noteLengthInSamples;
        private float _peakAmplitude;
        private float _preReleaseVolume = 0f;
        private Song _song;
        private Track _track;

        /// <inheritdoc/>
        public event EventHandler OnFinished;

        protected Envelope Envelope {
            get {
                return this.Instrument.NoteEnvelope;
            }
        }

        protected Instrument Instrument {
            get {
                return this._track.Instrument;
            }
        }

        protected NoteInstance Note {
            get {
                return this._note;
            }
        }

        /// <inheritdoc/>
        public void Dispose() {
            this.Dispose(true);
        }

        /// <inheritdoc/>
        public AudioSample[] GetNextSamples() {
            var samplesPerBeat = this.GetSamplesPerBuffer();
            var samples = new AudioSample[samplesPerBeat];
            var sampleModifier = samplesPerBeat * this._beatsPlayed;

            for (var i = 0; i < samples.Length; i++) {
                if (this._isActive) {
                    var sampleNumber = sampleModifier + i;
                    var frequency = this._note.GetFrequency((sampleNumber / (float)this._noteLengthInSamples).Clamp(0f, 1f));
                    var volume = this.GetSampleAmplitude(sampleNumber);
                    var time = sampleNumber * this._inverseSampleRate;
                    var leftSample = this.Instrument.Oscillator.GetSignal(time, frequency, volume * this._track.LeftChannelVolume);
                    var rightSample = this.Instrument.Oscillator.GetSignal(time, frequency, volume * this._track.RightChannelVolume);

                    foreach (var effect in this.Instrument.Effects) {
                        leftSample = effect.ApplyEffect(leftSample, time);
                        rightSample = effect.ApplyEffect(rightSample, time);
                    }

                    samples[i] = new AudioSample(leftSample, rightSample);
                }
                else {
                    samples[i] = new AudioSample(0f, 0f);
                }
            }

            this._beatsPlayed++;

            if (!this._isActive) {
                this.OnFinished?.Invoke(this, EventArgs.Empty);
            }

            return samples;
        }

        /// <inheritdoc/>
        public virtual void Reinitialize(Song song, Track track, NoteInstance note) {
            this._isActive = true;
            this._song = song;
            this._track = track;
            this._note = note;
            this._beatsPlayed = 0;
            this._noteLengthInSamples = this._note.Length * this._song.SamplesPerBeat;
            this._preReleaseVolume = 0f;
            this._peakAmplitude = this.Envelope.Decay > 0 ? this.Envelope.PeakAmplitude : this.Envelope.SustainAmplitude;
            this._inverseSampleRate = 1f / this._song.SampleRate;
        }

        // To detect redundant calls
        protected virtual void Dispose(bool disposing) {
            if (!this._isDisposed) {
                this.OnFinished = null;
                this._isDisposed = true;
            }
        }

        protected virtual int GetNoteLengthInSamples() {
            return this._noteLengthInSamples;
        }

        protected virtual ushort GetSamplesPerBuffer() {
            return this._song.SamplesPerBeat;
        }

        protected virtual bool IsNoteOver(int sampleNumber) {
            return sampleNumber >= this._noteLengthInSamples;
        }

        protected virtual bool IsNoteReleasing(int sampleNumber) {
            return (sampleNumber - this._noteLengthInSamples) < this.Envelope.Release;
        }

        private float GetAttackAmplitude(int sampleNumber) {
            return this._peakAmplitude * (sampleNumber / (float)this.Envelope.Attack);
        }

        private float GetDecayAmplitude(int sampleNumber) {
            return this.Envelope.SustainAmplitude + (this._peakAmplitude - this.Envelope.SustainAmplitude) * (1f - ((sampleNumber - this.Envelope.Attack) / (float)this.Envelope.Decay));
        }

        private float GetReleaseAmplitude(int sampleNumber) {
            return this._preReleaseVolume * (1f - ((sampleNumber - this.GetNoteLengthInSamples()) / (float)this.Envelope.Release));
        }

        private float GetSampleAmplitude(int sampleNumber) {
            var result = 0f;
            if (!this.IsNoteOver(sampleNumber)) {
                if (sampleNumber < this.Envelope.Attack) {
                    result = this.GetAttackAmplitude(sampleNumber);
                }
                else if ((sampleNumber - this.Envelope.Attack) < this.Envelope.Decay) {
                    result = this.GetDecayAmplitude(sampleNumber);
                }
                else {
                    result = this._track.Instrument.NoteEnvelope.SustainAmplitude;
                }

                this._preReleaseVolume = result;
            }
            else if (this.IsNoteReleasing(sampleNumber)) {
                result = this.GetReleaseAmplitude(sampleNumber);
            }
            else {
                this._isActive = false;
            }

            return result * this._note.Velocity;
        }
    }
}