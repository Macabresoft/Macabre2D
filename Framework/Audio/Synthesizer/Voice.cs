namespace Macabre2D.Framework {

    using System;

    /// <summary>
    /// A synthesizer voice. These are pooled and used to play notes to completion.
    /// </summary>
    public class Voice : IDisposable, IVoice {
        private bool _isActive;
        private bool _isDisposed = false;
        private float _noteLengthInMilliseconds;
        private ulong _noteLengthInSamples;
        private ulong _offset;
        private float _peakAmplitude;
        private float _preReleaseVolume = 0f;
        private ulong _samplesPlayed;
        private Song _song;
        private float _startingBeat;
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

        protected NoteInstance Note { get; private set; }

        /// <inheritdoc/>
        public void Dispose() {
            this.Dispose(true);
        }

        /// <inheritdoc/>
        public AudioSample[] GetBuffer(ushort numberOfSamples) {
            var samples = new AudioSample[numberOfSamples];

            for (var i = 0; i < samples.Length; i++) {
                var sampleNumber = (int)this._samplesPlayed + i - (int)this._offset;
                if (this._isActive && sampleNumber >= 0) {
                    var frequency = this.Note.GetFrequency((sampleNumber / (float)this._noteLengthInSamples).Clamp(0f, 1f));
                    var time = sampleNumber * this._song.InverseSampleRate;
                    var volume = this.GetSampleAmplitude(time * 1000f);
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

            this._samplesPlayed += numberOfSamples;

            if (!this._isActive) {
                this.OnFinished?.Invoke(this, EventArgs.Empty);
            }

            return samples;
        }

        /// <inheritdoc/>
        public virtual void Reinitialize(Song song, Track track, NoteInstance note, float startingBeat) {
            this._isActive = true;
            this._song = song;
            this._track = track;
            this.Note = note;
            this._samplesPlayed = 0;
            this._startingBeat = startingBeat;
            this._offset = this._song.ConvertBeatsToSamples(this.Note.Beat - this._startingBeat);
            this._noteLengthInSamples = this._song.ConvertBeatsToSamples(this.Note.Length);
            this._noteLengthInMilliseconds = 1000f * (this.Note.Length / this._song.BeatsPerSecond);
            this._preReleaseVolume = 0f;
            this._peakAmplitude = this.Envelope.Decay > 0 ? this.Envelope.PeakAmplitude : this.Envelope.SustainAmplitude;
        }

        protected virtual void Dispose(bool disposing) {
            if (!this._isDisposed) {
                this.OnFinished = null;
                this._isDisposed = true;
            }
        }

        protected virtual float GetNoteLengthInMilliseconds() {
            return this._noteLengthInMilliseconds;
        }

        protected virtual bool IsNoteOver(float millisecondsIntoNote) {
            return millisecondsIntoNote >= this._noteLengthInMilliseconds;
        }

        protected virtual bool IsNoteReleasing(float millisecondsIntoNote) {
            return (millisecondsIntoNote - this._noteLengthInMilliseconds) < this.Envelope.Release;
        }

        private float GetAttackAmplitude(float millisecondsIntoNote) {
            return this._peakAmplitude * (millisecondsIntoNote / this.Envelope.Attack);
        }

        private float GetDecayAmplitude(float millisecondsIntoNote) {
            return this.Envelope.SustainAmplitude + (this._peakAmplitude - this.Envelope.SustainAmplitude) * (1f - ((millisecondsIntoNote - this.Envelope.Attack) / this.Envelope.Decay));
        }

        private float GetReleaseAmplitude(float millisecondsIntoNote) {
            return this._preReleaseVolume * (1f - ((millisecondsIntoNote - this.GetNoteLengthInMilliseconds()) / this.Envelope.Release));
        }

        private float GetSampleAmplitude(float millisecondsIntoNote) {
            var result = 0f;
            if (!this.IsNoteOver(millisecondsIntoNote)) {
                if (millisecondsIntoNote < this.Envelope.Attack) {
                    result = this.GetAttackAmplitude(millisecondsIntoNote);
                }
                else if ((millisecondsIntoNote - this.Envelope.Attack) < this.Envelope.Decay) {
                    result = this.GetDecayAmplitude(millisecondsIntoNote);
                }
                else {
                    result = this._track.Instrument.NoteEnvelope.SustainAmplitude;
                }

                this._preReleaseVolume = result;
            }
            else if (this.IsNoteReleasing(millisecondsIntoNote)) {
                result = this.GetReleaseAmplitude(millisecondsIntoNote);
            }
            else {
                this._isActive = false;
            }

            return result * this.Note.Velocity;
        }
    }
}