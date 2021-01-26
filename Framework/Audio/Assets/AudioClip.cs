namespace Macabresoft.Macabre2D.Framework {
    using System;
    using Microsoft.Xna.Framework.Audio;

    /// <summary>
    /// A single audio clip.
    /// </summary>
    public sealed class AudioClip : BaseContentAsset<SoundEffect>, IDisposable {
        /// <summary>
        /// Gets a sound effect instance.
        /// </summary>
        /// <param name="volume">The volume.</param>
        /// <param name="pan">The panning.</param>
        /// <param name="pitch">The pitch.</param>
        /// <returns>A sound effect instance.</returns>
        public SoundEffectInstance? GetSoundEffectInstance(float volume, float pan, float pitch) {
            SoundEffectInstance? instance;
            if (this.Content is SoundEffect soundEffect) {
                instance = soundEffect.CreateInstance();
                instance.Volume = volume;
                instance.Pan = pan;
                instance.Pitch = pitch;
            }
            else {
                instance = null;
            }

            return instance;
        }
        
        
        /// <inheritdoc />
        public void Dispose() {
            this.Content?.Dispose();
            this.DisposePropertyChanged();
        }
    }
}