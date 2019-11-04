namespace Macabre2D.Framework {

    using System.Collections.Concurrent;

    /// <summary>
    /// A pool of voices.
    /// </summary>
    /// <seealso cref="Macabre2D.Framework.IObjectPool{Macabre2D.Framework.Voice}"/>
    public sealed class VoicePool : IObjectPool<Voice> {
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

        /// <inheritdoc/>
        public Voice GetNext() {
            if (!this._voices.TryPop(out var voice)) {
                voice = new Voice();
            }

            return voice;
        }

        /// <inheritdoc/>
        public void ReturnObject(Voice pooledObject) {
            this._voices.Push(pooledObject);
        }
    }
}