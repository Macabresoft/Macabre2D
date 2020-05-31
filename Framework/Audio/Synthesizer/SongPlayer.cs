namespace Macabre2D.Framework {

    using Microsoft.Xna.Framework.Audio;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Interface for an object that can play songs.
    /// </summary>
    public interface ISongPlayer {

        /// <summary>
        /// Gets the song.
        /// </summary>
        /// <value>The song.</value>
        Song Song { get; }

        /// <summary>
        /// Buffers the <see cref="DynamicSoundEffectInstance"/> owned by this instance at the
        /// specified volume.
        /// </summary>
        /// <param name="volume">The volume.</param>
        void Buffer(float volume, ushort numberOfSamples);
    }

    /// <summary>
    /// Plays a song.
    /// </summary>
    public sealed class SongPlayer : ISongPlayer {
        private readonly HashSet<Voice> _activeVoices = new HashSet<Voice>();
        private readonly HashSet<Voice> _inactiveVoices = new HashSet<Voice>();
        private readonly DynamicSoundEffectInstance _soundEffectInstance;
        private readonly VoicePool<Voice> _voicePool;
        private float _currentBeat = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="SongPlayer"/> class.
        /// </summary>
        /// <param name="song">The song.</param>
        /// <param name="voicePool">The voice pool.</param>
        public SongPlayer(Song song, VoicePool<Voice> voicePool) {
            this.Song = song ?? throw new ArgumentNullException(nameof(song));
            this._voicePool = voicePool;
            this._soundEffectInstance = new DynamicSoundEffectInstance(song.SampleRate, AudioChannels.Stereo);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SongPlayer"/> class.
        /// </summary>
        /// <param name="song">The song.</param>
        public SongPlayer(Song song) : this(song, new VoicePool<Voice>()) {
        }

        /// <inheritdoc/>
        public Song Song { get; }

        /// <inheritdoc/>
        public void Buffer(float volume, ushort numberOfSamples) {
            if (this._currentBeat >= this.Song.Length) {
                this._currentBeat = 0f;
            }

            foreach (var voice in this._inactiveVoices) {
                this._activeVoices.Remove(voice);
                this._voicePool.Return(voice);
            }

            this._inactiveVoices.Clear();

            if (this._soundEffectInstance.PendingBufferCount < 3) {
                var beatLengthOfBuffer = this.Song.ConvertSamplesToBeats(numberOfSamples);
                var endOfBuffer = this._currentBeat + beatLengthOfBuffer;
                if (endOfBuffer > this.Song.Length) {
                    endOfBuffer = this.Song.Length;
                    beatLengthOfBuffer = endOfBuffer - this._currentBeat;
                    numberOfSamples = (ushort)this.Song.ConvertBeatsToSamples(beatLengthOfBuffer);
                }

                var range = new RangeVector(this._currentBeat, endOfBuffer);
                foreach (var track in this.Song.Tracks) {
                    var notes = track.GetNotes(range);

                    foreach (var note in notes) {
                        var voice = this._voicePool.GetNext(this.Song, track, note, this._currentBeat);
                        voice.OnFinished += this.Voice_OnFinished;
                        this._activeVoices.Add(voice);
                    }
                }

                var trackMultiplier = volume / (float)Math.Sqrt(this.Song.Tracks.Count);
                var samples = SampleHelper.GetBufferSamples(this._activeVoices, numberOfSamples, trackMultiplier);
                this._soundEffectInstance.SubmitBuffer(samples);
                this._currentBeat += beatLengthOfBuffer;
            }
        }

        /// <summary>
        /// Pauses this instance.
        /// </summary>
        public void Pause() {
            this._soundEffectInstance.Pause();
        }

        /// <summary>
        /// Plays this instance.
        /// </summary>
        public void Play() {
            this._soundEffectInstance.Play();
        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        public void Stop() {
            this._soundEffectInstance.Stop();
        }

        private void Voice_OnFinished(object sender, EventArgs e) {
            if (sender is Voice voice) {
                voice.OnFinished -= this.Voice_OnFinished;
                this._inactiveVoices.Add(voice);
            }
        }
    }
}