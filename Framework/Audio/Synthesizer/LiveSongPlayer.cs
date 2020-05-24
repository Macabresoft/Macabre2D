namespace Macabre2D.Framework {

    using Macabre2D.Framework;
    using Microsoft.Xna.Framework.Audio;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// The piano roll.
    /// </summary>
    public sealed class LiveSongPlayer : ISongPlayer {
        public const string SpriteSheetPath = "PianoRollSpriteSheet";
        private readonly Dictionary<Frequency, LiveVoice> _activeVoices = new Dictionary<Frequency, LiveVoice>();
        private readonly HashSet<LiveVoice> _inactiveVoices = new HashSet<LiveVoice>();
        private readonly DynamicSoundEffectInstance _soundEffectInstance;
        private readonly VoicePool<LiveVoice> _voicePool = new VoicePool<LiveVoice>();
        private Track _track;

        /// <summary>
        /// Initializes a new instance of the <see cref="LiveSongPlayer"/> class.
        /// </summary>
        /// <param name="song">The song.</param>
        public LiveSongPlayer(Song song) {
            this.Song = song;
            this._track = this.Song.Tracks.First();
            this._soundEffectInstance = new DynamicSoundEffectInstance(this.Song.SampleRate, AudioChannels.Stereo);
        }

        /// <summary>
        /// Gets or sets the samples per buffer.
        /// </summary>
        /// <value>The samples per buffer.</value>
        public ushort SamplesPerBuffer { get; set; } = 500;

        /// inheritdoc />
        public Song Song { get; }

        /// <summary>
        /// Gets or sets the track.
        /// </summary>
        /// <value>The track.</value>
        public Track Track {
            get {
                if (this._track == null) {
                    this._track = this.Song.Tracks.First();
                }

                return this._track;
            }

            set {
                if (this.Song.Tracks.Contains(value)) {
                    this._track = value;
                }
            }
        }

        /// inheritdoc />
        public void Buffer(float volume) {
            foreach (var inactiveVoice in this._inactiveVoices) {
                this._activeVoices.Remove(inactiveVoice.Frequency);
                this._voicePool.Return(inactiveVoice);
            }

            this._inactiveVoices.Clear();

            if (this._activeVoices.Any() && this._soundEffectInstance.PendingBufferCount < 2) {
                var samples = SampleHelper.GetBufferSamples(this._activeVoices.Values, this.SamplesPerBuffer, volume);
                this._soundEffectInstance.SubmitBuffer(samples);
            }
        }

        /// <summary>
        /// Plays the note at the specified frequency.
        /// </summary>
        /// <param name="frequency">The frequency.</param>
        public void PlayNote(Frequency frequency) {
            if (!this._activeVoices.ContainsKey(frequency)) {
                var voice = this._voicePool.GetNext();
                voice.SamplesPerBuffer = this.SamplesPerBuffer;
                voice.Reinitialize(this.Song, this.Track, new NoteInstance(0, 1, 1f, frequency));
                voice.OnFinished += this.Voice_OnFinished;
                this._activeVoices.Add(frequency, voice);

                if (this._soundEffectInstance.State != SoundState.Playing) {
                    this._soundEffectInstance.Play();
                }
            }
        }

        /// <summary>
        /// Stops the note playing at the specified frequency.
        /// </summary>
        /// <param name="frequency">The frequency.</param>
        public void StopNote(Frequency frequency) {
            if (this._activeVoices.TryGetValue(frequency, out var voice)) {
                voice.Stop();
            }
        }

        private void Voice_OnFinished(object sender, System.EventArgs e) {
            if (sender is LiveVoice voice) {
                voice.OnFinished -= this.Voice_OnFinished;
                this._inactiveVoices.Add(voice);
            }
        }
    }
}