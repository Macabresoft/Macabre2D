namespace Macabre2D.UI.CosmicSynthLibrary.Controls.SongEditing {

    using Macabre2D.Framework;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Audio;
    using System.Collections.Generic;
    using System.Linq;

    public interface IPianoRoll {
        ushort SamplesPerBuffer { get; }

        Song Song { get; }

        Track Track { get; }

        void Buffer(float volume);

        float GetRowPosition(Note note, Pitch pitch);

        void PlayNote(Frequency frequency);

        void StopNote(Frequency frequency);
    }

    public sealed class PianoRoll : IPianoRoll {
        public const string SpriteSheetPath = "PianoRollSpriteSheet";
        private readonly Dictionary<Frequency, LiveVoice> _activeVoices = new Dictionary<Frequency, LiveVoice>();
        private readonly VoicePool<LiveVoice> _voicePool = new VoicePool<LiveVoice>();
        private Song _song;
        private DynamicSoundEffectInstance _soundEffectInstance;
        private Track _track;

        public PianoRoll() {
            FrameworkDispatcher.Update();
            this._soundEffectInstance = new DynamicSoundEffectInstance(Song.MinimumSampleRate, AudioChannels.Mono);
        }

        public ushort SamplesPerBuffer { get; set; } = 500;

        public Song Song {
            get {
                if (this._song == null) {
                    this.Song = new Song();
                }

                return this._song;
            }

            set {
                if (this._song != value) {
                    this._song = value;
                    this.SetSoundEffectInstance(new DynamicSoundEffectInstance(this._song.SampleRate, AudioChannels.Stereo));
                }
            }
        }

        public Track Track {
            get {
                if (this._track == null) {
                    this._track = this.Song.Tracks.First();
                }

                return this._track;
            }

            set {
                this._track = value;
            }
        }

        public void Buffer(float volume) {
            if (this._activeVoices.Any() && this._soundEffectInstance.PendingBufferCount < 2) {
                var samples = SampleHelper.GetBufferSamples(this._activeVoices.Values, this.SamplesPerBuffer, volume);
                this._soundEffectInstance.SubmitBuffer(samples);
            }
        }

        public float GetRowPosition(Note note, Pitch pitch) {
            return 12f * GetPitchMultiplier(pitch) + (byte)note;
        }

        public void PlayNote(Frequency frequency) {
            if (!this._activeVoices.ContainsKey(frequency)) {
                var voice = this._voicePool.GetNext();
                voice.SamplesPerBuffer = this.SamplesPerBuffer;
                voice.Reinitialize(this.Song, this.Track, new NoteInstance(0, 1, 1f, frequency));
                this._activeVoices.Add(frequency, voice);

                if (this._soundEffectInstance.State != SoundState.Playing) {
                    this._soundEffectInstance.Play();
                }
            }
        }

        public void StopNote(Frequency frequency) {
            if (this._activeVoices.TryGetValue(frequency, out var voice)) {
                voice.Stop();
                this._activeVoices.Remove(frequency);
                this._voicePool.Return(voice);
            }
        }

        private static float GetPitchMultiplier(Pitch pitch) {
            var result = 0f;
            if (pitch == Pitch.Normal) {
                result = 1f;
            }
            else if (pitch == Pitch.High) {
                result = 2f;
            }

            return result;
        }

        private void SetSoundEffectInstance(DynamicSoundEffectInstance value) {
            this._soundEffectInstance?.Dispose();
            this._soundEffectInstance = value;
        }
    }
}