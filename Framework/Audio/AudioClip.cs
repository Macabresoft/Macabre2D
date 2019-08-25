namespace Macabre2D.Framework {

    using Microsoft.Xna.Framework.Audio;
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// A single audio clip.
    /// </summary>
    public sealed class AudioClip : BaseIdentifiable, IAsset, IDisposable {
        private bool _disposedValue = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="AudioClip"/> class.
        /// </summary>
        public AudioClip() {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AudioClip"/> class.
        /// </summary>
        /// <param name="assetId">The asset identifier.</param>
        /// <param name="contentManager">The content manager.</param>
        /// <param name="volume">The volume.</param>
        /// <param name="pan">The pan.</param>
        /// <param name="pitch">The pitch.</param>
        public AudioClip(Guid assetId, float volume, float pan, float pitch) {
            this.AssetId = assetId;
            this.LoadSoundEffect(volume, pan, pitch);
        }

        /// <inheritdoc/>
        [DataMember]
        public Guid AssetId { get; set; }

        internal SoundEffectInstance SoundEffectInstance { get; set; }

        /// <inheritdoc/>
        public void Dispose() {
            this.Dispose(true);
        }

        /// <summary>
        /// Loads the sound effect.
        /// </summary>
        /// <param name="contentManager">The content manager.</param>
        /// <param name="volume">The volume.</param>
        /// <param name="pan">The pan.</param>
        /// <param name="pitch">The pitch.</param>
        public void LoadSoundEffect(float volume, float pan, float pitch) {
            var soundEffect = AssetManager.Instance.Load<SoundEffect>(this.AssetId);

            if (soundEffect != null) {
                this.SoundEffectInstance = soundEffect.CreateInstance();
                this.SoundEffectInstance.Volume = volume;
                this.SoundEffectInstance.Pan = pan;
                this.SoundEffectInstance.Pitch = pitch;
            }
        }

        private void Dispose(bool disposing) {
            if (!this._disposedValue) {
                if (disposing) {
                    this.SoundEffectInstance.Dispose();
                }

                this._disposedValue = true;
            }
        }
    }
}