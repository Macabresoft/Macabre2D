namespace Macabre2D.Framework {

    using System.Collections.Concurrent;

    /// <summary>
    /// A pool of voices.
    /// </summary>
    public sealed class VoicePool<T> where T : IVoice, new() {
        private const int StartingNumberOfPooledVoices = 4;
        private readonly ConcurrentStack<T> _voices = new ConcurrentStack<T>();

        /// <summary>
        /// Initializes a new instance of the <see cref="VoicePool"/> class.
        /// </summary>
        public VoicePool() {
            for (var i = 0; i < StartingNumberOfPooledVoices; i++) {
                this._voices.Push(new T());
            }
        }

        /// <summary>
        /// Gets the next available <see cref="Voice"/>.
        /// </summary>
        /// <returns>The next voice.</returns>
        public T GetNext(Song song, Track track, NoteInstance note, float offset) {
            if (!this._voices.TryPop(out var voice)) {
                voice = new T();
            }

            voice.Reinitialize(song, track, note, offset);
            return voice;
        }

        /// <summary>
        /// Returns the specified pooled voice.
        /// </summary>
        /// <param name="pooledVoice">The pooled voice.</param>
        public void Return(T pooledVoice) {
            this._voices.Push(pooledVoice);
        }
    }
}