namespace Macabre2D.Framework {

    using Microsoft.Xna.Framework.Audio;
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// A single audio clip.
    /// </summary>
    [DataContract]
    public sealed class AudioClip : IDisposable, IIdentifiable {
        private bool _disposedValue = false;

        [DataMember]
        private Guid _id = Guid.NewGuid();

        /// <summary>
        /// Initializes a new instance of the <see cref="AudioClip"/> class.
        /// </summary>
        public AudioClip() {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AudioClip"/> class.
        /// </summary>
        /// <param name="contentId">The content identifier.</param>
        /// <param name="contentManager">The content manager.</param>
        /// <param name="volume">The volume.</param>
        /// <param name="pan">The pan.</param>
        /// <param name="pitch">The pitch.</param>
        public AudioClip(Guid contentId, float volume, float pan, float pitch) {
            this.ContentId = contentId;
            this.LoadSoundEffect(volume, pan, pitch);
        }

        /// <summary>
        /// Gets or sets the content identifier.
        /// </summary>
        /// <value>The content identifier.</value>
        [DataMember]
        public Guid ContentId { get; set; }

        /// <inheritdoc/>
        public Guid Id {
            get {
                return this._id;
            }
        }

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
            var soundEffect = AssetManager.Instance.Load<SoundEffect>(this.ContentId);

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