namespace Macabre2D.Framework {

    using System.Collections.Concurrent;

    /// <summary>
    /// A pool of voices.
    /// </summary>
    public sealed class VoicePool {
        private const int StartingNumberOfPooledVoices = 4;
        private readonly ConcurrentStack<Voice> _voices = new ConcurrentStack<Voice>();

        /// <summary>
        /// Initializes a new instance of the <see cref="VoicePool"/> class.
        /// </summary>
        public VoicePool() {
            for (var i = 0; i < StartingNumberOfPooledVoices; i++) {
                this._voices.Push(new Voice());
            }
        }

        /// <summary>
        /// Gets the next available <see cref="Voice"/>.
        /// </summary>
        /// <returns>The next voice.</returns>
        public Voice GetNext() {
            if (!this._voices.TryPop(out var voice)) {
                voice = new Voice();
            }

            return voice;
        }

        /// <summary>
        /// Returns the specified pooled voice.
        /// </summary>
        /// <param name="pooledVoice">The pooled voice.</param>
        public void Return(Voice pooledVoice) {
            this._voices.Push(pooledVoice);
        }
    }
}