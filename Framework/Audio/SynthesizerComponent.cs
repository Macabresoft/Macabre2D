namespace Macabre2D.Framework {

    using CosmicSynth.Framework;
    using Microsoft.Xna.Framework;
    using System.Runtime.Serialization;
    using System.Threading.Tasks;

    /// <summary>
    /// A component that acts as a synthesizer inside a scene.
    /// </summary>
    /// <seealso cref="Macabre2D.Framework.BaseComponent"/>
    /// <seealso cref="Macabre2D.Framework.IUpdateableComponentAsync"/>
    public sealed class SynthesizerComponent : BaseComponent, IUpdateableComponent, IUpdateableComponentAsync {
        private Song _song;
        private SongPlayer _songPlayer;
        private float _volume = 1f;

        /// <summary>
        /// Gets or sets the song.
        /// </summary>
        /// <value>The song.</value>
        [DataMember]
        public Song Song {
            get {
                return this._song;
            }

            set {
                this._song = value;

                if (this.IsInitialized) {
                    this._songPlayer?.Stop();
                    if (this._song != null) {
                        this._songPlayer = new SongPlayer(this._song);
                        this._songPlayer.Play();
                    }
                    else {
                        this._songPlayer = null;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the volume.
        /// </summary>
        /// <value>The volume.</value>
        [DataMember]
        public float Volume {
            get {
                return this._volume;
            }

            set {
                this._volume = value.Clamp(0f, 1f);
            }
        }

        public void Update(GameTime gameTime) {
            if (this._songPlayer != null) {
                this._songPlayer.Buffer(this.Volume);
            }
        }

        /// <inheritdoc/>
        public Task UpdateAsync(GameTime gameTime) {
            return /*this._songPlayer != null ? Task.Run(() => this._songPlayer.Buffer(this.Volume)) :*/ Task.CompletedTask;
        }

        /// <inheritdoc/>
        protected override void Initialize() {
            if (this.Scene != null) {
                if (this._song != null) {
                    this._songPlayer = new SongPlayer(this._song);
                    this._songPlayer.Play();
                }
            }
        }
    }
}