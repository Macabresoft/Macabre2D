namespace Macabre2D.Framework {

    using System.Runtime.Serialization;

    /// <summary>
    /// A tremolo effect.
    /// </summary>
    /// <seealso cref="Macabre2D.Framework.IAudioEffect"/>
    [DataContract]
    public sealed class TremoloEffect : IAudioEffect {
        private float _depth = 0.5f;
        private float _depthModifier = 1f;
        private float _halfDepth = 0.25f;
        private IOscillator _oscillator = new SineWaveOscillator();
        private float _rate = 20f;

        /// <summary>
        /// Gets or sets the depth. This is a value between 0 and 1 which determines how low this
        /// tremolo can affect the volume.
        /// </summary>
        /// <value>The depth.</value>
        [DataMember]
        public float Depth {
            get {
                return this._depth;
            }

            set {
                this._depth = value.Clamp(0f, 1f);
                this._halfDepth = this._depth * 0.5f;
                this._depthModifier = 1.0f - this._depth + 0.5f;
            }
        }

        /// <summary>
        /// Gets or sets the oscillator.
        /// </summary>
        /// <value>The oscillator.</value>
        [DataMember]
        public IOscillator Oscillator {
            get {
                return this._oscillator;
            }

            set {
                if (value != null) {
                    this._oscillator = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the rate. This is a value measured in Hz that determines how quickly the
        /// tremolo effect modulates.
        /// </summary>
        /// <value>The rate.</value>
        [DataMember]
        public float Rate {
            get {
                return this._rate;
            }

            set {
                this._rate = value.Clamp(0.01f, 20f);
            }
        }

        /// <inheritdoc/>
        public float ApplyEffect(float sample, float time) {
            return sample * (this._oscillator.GetSignal(time, this.Rate, this._halfDepth) + this._depthModifier);
        }
    }
}