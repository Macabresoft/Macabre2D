namespace Macabresoft.Macabre2D.Framework {
    using System;
    using Microsoft.Xna.Framework.Audio;

    /// <summary>
    /// A single audio clip.
    /// </summary>
    public sealed class AudioClip : BaseContentAsset<SoundEffectInstance>, IDisposable {
        
        /// <summary>
        /// Initializes the <see cref="SoundEffectInstance"/> with volume, pan, and pitch.
        /// </summary>
        /// <param name="volume">The volume.</param>
        /// <param name="pan">The pan.</param>
        /// <param name="pitch">The pitch.</param>
        public void Initialize(float volume, float pan, float pitch) {
            if (this.Content is SoundEffectInstance instance) {
                instance.Volume = volume;
                instance.Pan = pan;
                instance.Pitch = pitch;
            }
        }
        
        
        /// <inheritdoc />
        public void Dispose() {
            this.Content?.Dispose();
            this.DisposePropertyChanged();
        }
    }
}