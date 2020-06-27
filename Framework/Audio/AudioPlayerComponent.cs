namespace Macabre2D.Framework {

    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Audio;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.Serialization;

    /// <summary>
    /// Plays a <see cref="AudioClip"/>.
    /// </summary>
    public sealed class AudioPlayerComponent : BaseComponent, IAssetComponent<AudioClip> {
        private AudioClip _audioClip;
        private float _pan;
        private float _pitch;
        private bool _shouldLoop;
        private float _volume = 1f;

        /// <summary>
        /// Gets or sets the audio clip.
        /// </summary>
        /// <value>The audio clip.</value>
        [DataMember(Order = 0, Name = "Audio Clip")]
        public AudioClip AudioClip {
            get {
                return this._audioClip;
            }

            set {
                if (this.Set(ref this._audioClip, value) && !MacabreGame.Instance.IsDesignMode && this._audioClip?.SoundEffectInstance != null) {
                    this._audioClip.SoundEffectInstance.Stop(true);
                    this.LoadContent();
                }
            }
        }

        /// <summary>
        /// Gets or sets the pan.
        /// </summary>
        /// <value>The pan.</value>
        [DataMember(Order = 3, Name = "Pan")]
        public float Pan {
            get {
                return this._pan;
            }

            set {
                if (this.Set(ref this._pan, MathHelper.Clamp(value, -1f, 1f)) && this._audioClip?.SoundEffectInstance != null) {
                    this._audioClip.SoundEffectInstance.Pan = this._pan;
                }
            }
        }

        /// <summary>
        /// Gets or sets the pitch.
        /// </summary>
        /// <value>The pitch.</value>
        [DataMember(Order = 4, Name = "Pitch")]
        public float Pitch {
            get {
                return this._pitch;
            }

            set {
                if (this.Set(ref this._pitch, MathHelper.Clamp(value, -1f, 1f)) && this._audioClip?.SoundEffectInstance != null) {
                    this._audioClip.SoundEffectInstance.Pitch = this._pitch;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="AudioPlayerComponent"/> should loop.
        /// </summary>
        /// <value><c>true</c> if should loop; otherwise, <c>false</c>.</value>
        [DataMember(Order = 1, Name = "Should Loop")]
        public bool ShouldLoop {
            get {
                return this._shouldLoop;
            }

            set {
                if (this.Set(ref this._shouldLoop, value) && this._audioClip?.SoundEffectInstance != null) {
                    this._audioClip.SoundEffectInstance.IsLooped = this._shouldLoop;
                }
            }
        }

        /// <summary>
        /// Gets the state.
        /// </summary>
        /// <value>The state.</value>
        public SoundState State {
            get {
                return this._audioClip?.SoundEffectInstance?.State ?? SoundState.Stopped;
            }
        }

        /// <summary>
        /// Gets or sets the volume.
        /// </summary>
        /// <value>The volume.</value>
        [DataMember(Order = 2, Name = "Volume")]
        public float Volume {
            get {
                return this._volume;
            }

            set {
                if (this.Set(ref this._volume, MathHelper.Clamp(value, 0f, 1f)) && this._audioClip?.SoundEffectInstance != null) {
                    this._audioClip.SoundEffectInstance.Volume = this._volume;
                }
            }
        }

        /// <inheritdoc/>
        public IEnumerable<Guid> GetOwnedAssetIds() {
            return this.AudioClip != null ? new[] { this.AudioClip.Id } : new Guid[0];
        }

        /// <inheritdoc/>
        public bool HasAsset(Guid id) {
            return this.AudioClip?.Id == id;
        }

        /// <inheritdoc/>
        public override void LoadContent() {
            if (this._audioClip != null && this.Scene.IsInitialized) {
                this._audioClip.LoadSoundEffect(this.Volume, this.Pan, this.Pitch);

                if (this._shouldLoop && this.IsEnabled && this._audioClip.SoundEffectInstance != null) {
                    this._audioClip.SoundEffectInstance.IsLooped = true;
                    this.Play();
                }
            }

            base.LoadContent();
        }

        /// <summary>
        /// Pause this instance.
        /// </summary>
        public void Pause() {
            if (this._audioClip?.SoundEffectInstance != null && this._audioClip.SoundEffectInstance.State == SoundState.Playing) {
                this._audioClip.SoundEffectInstance.Pause();
            }
        }

        /// <summary>
        /// Play this instance.
        /// </summary>
        public void Play() {
            if (this._audioClip?.SoundEffectInstance != null && this._audioClip.SoundEffectInstance.State != SoundState.Playing) {
                this._audioClip.SoundEffectInstance.Volume = this.Volume;
                this._audioClip.SoundEffectInstance.Pan = this.Pan;
                this._audioClip.SoundEffectInstance.Pitch = this.Pitch;
                this._audioClip.SoundEffectInstance.Play();
            }
        }

        /// <inheritdoc/>
        public void RefreshAsset(AudioClip newInstance) {
            this.AudioClip = newInstance;
        }

        /// <inheritdoc/>
        public bool RemoveAsset(Guid id) {
            var result = this.HasAsset(id);
            if (result) {
                this._audioClip = null;
            }

            return result;
        }

        /// <summary>
        /// Stop this instance.
        /// </summary>
        public void Stop() {
            if (this._audioClip?.SoundEffectInstance != null && this._audioClip.SoundEffectInstance.State != SoundState.Stopped) {
                this._audioClip.SoundEffectInstance.Stop(true);
            }
        }

        /// <summary>
        /// Stop this instance.
        /// </summary>
        /// <param name="isImmediate">If set to <c>true</c> is immediate.</param>
        public void Stop(bool isImmediate) {
            if (this._audioClip?.SoundEffectInstance != null && this._audioClip.SoundEffectInstance.State != SoundState.Stopped) {
                this._audioClip.SoundEffectInstance.Stop(isImmediate);
            }
        }

        /// <inheritdoc/>
        public bool TryGetAsset(Guid id, out AudioClip asset) {
            var result = this.AudioClip?.Id == id;
            asset = result ? this.AudioClip : null;
            return result;
        }

        /// <inheritdoc/>
        protected override void Initialize() {
            this.PropertyChanged += this.Self_PropertyChanged;
        }

        private void Self_PropertyChanged(object sender, PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(this.IsEnabled)) {
                if (this.ShouldLoop && this.IsEnabled && this._audioClip.SoundEffectInstance != null) {
                    this._audioClip.SoundEffectInstance.IsLooped = true;
                    this.Play();
                }
                else if (!this.IsEnabled) {
                    this.Stop();
                }
            }
        }
    }
}