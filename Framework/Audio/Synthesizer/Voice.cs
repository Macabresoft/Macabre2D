namespace Macabre2D.Framework {

    using System;

    /// <summary>
    /// A synthesizer voice. These are pooled and used to play notes to completion.
    /// </summary>
    public sealed class Voice {
        private ushort _beatsPlayed;
        private Frequency _frequency;
        private float _inverseSampleRate;
        private bool _isActive;
        private PlayedNote _note;
        private int _noteLengthInSamples;
        private float _peakAmplitude;
        private float _preReleaseVolume = 0f;
        private Song _song;
        private Track _track;

        /// <summary>
        /// Occurs when the note is finished.
        /// </summary>
        public event EventHandler OnFinished;

        private Envelope Envelope {
            get {
                return this.Instrument.NoteEnvelope;
            }
        }

        private Instrument Instrument {
            get {
                return this._track.Instrument;
            }
        }

        /// <summary>
        /// Gets the next samples.
        /// </summary>
        /// <returns>The next samples.</returns>
        public AudioSample[] GetNextSamples() {
            var samples = new AudioSample[this._song.SamplesPerBeat];
            var sampleModifier = this._song.SamplesPerBeat * this._beatsPlayed;

            for (var i = 0; i < samples.Length; i++) {
                if (this._isActive) {
                    var time = (sampleModifier + i) * this._inverseSampleRate;
                    var volume = this.GetSampleAmplitude(sampleModifier + i);
                    var leftSample = this.Instrument.Oscillator.GetSignal(time, this._frequency.Value, volume * this._track.LeftChannelVolume);
                    var rightSample = this.Instrument.Oscillator.GetSignal(time, this._frequency.Value, volume * this._track.RightChannelVolume);
                    samples[i] = new AudioSample(leftSample, rightSample);
                }
                else {
                    samples[i] = new AudioSample(0f, 0f);
                }
            }

            this._beatsPlayed++;

            if (!this._isActive) {
                this.OnFinished.SafeInvoke(this);
            }

            return samples;
        }

        /// <summary>
        /// Reinitializes the specified instrument.
        /// </summary>
        /// <param name="song">The song.</param>
        /// <param name="track">The track.</param>
        /// <param name="note">The note.</param>
        public void Reinitialize(Song song, Track track, PlayedNote note) {
            this._isActive = true;
            this._song = song;
            this._track = track;
            this._note = note;
            this._frequency = new Frequency(note.Note, track.Instrument.Pitch);
            this._beatsPlayed = 0;
            this._noteLengthInSamples = this._note.Length * this._song.SamplesPerBeat;
            this._preReleaseVolume = 0f;
            this._peakAmplitude = this.Envelope.Decay > 0 ? this.Envelope.PeakAmplitude : this.Envelope.SustainAmplitude;
            this._inverseSampleRate = 1f / this._song.SampleRate;
        }

        private float GetAttackAmplitude(int sampleNumber) {
            return this._peakAmplitude * (sampleNumber / (float)this.Envelope.Attack);
        }

        private float GetDecayAmplitude(int sampleNumber) {
            return this._peakAmplitude * (1f - ((sampleNumber - this.Envelope.Attack) / this.Envelope.Decay));
        }

        private float GetReleaseAmplitude(int sampleNumber) {
            return this._preReleaseVolume * (1f - ((sampleNumber - this._noteLengthInSamples) / this.Envelope.Release));
        }

        private float GetSampleAmplitude(int sampleNumber) {
            var result = 0f;
            if (sampleNumber < this._noteLengthInSamples) {
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
            else if ((sampleNumber - this._noteLengthInSamples) < this.Envelope.Release) {
                result = this.GetReleaseAmplitude(sampleNumber);
            }
            else {
                this._isActive = false;
            }

            return result;
        }
    }
}