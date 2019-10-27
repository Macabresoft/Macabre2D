namespace Macabre2D.Framework.Audio.Synthesizer {

    using Microsoft.Xna.Framework;
    using System.Runtime.Serialization;

    /// <summary>
    /// An envelope that allows control over the volume of sound being played over a note.
    /// </summary>
    [DataContract]
    public sealed class Envelope {
        private float _attack;
        private float _decay;
        private float _release;
        private float _sustain = 0.5f;

        /// <summary>
        /// Gets or sets the attack time.
        /// </summary>
        /// <value>The attack time.</value>
        [DataMember]
        public float Attack {
            get {
                return this._attack;
            }

            set {
                this._attack = MathHelper.Max(0f, value);
            }
        }

        /// <summary>
        /// Gets or sets the decay time.
        /// </summary>
        /// <value>The decay time.</value>
        [DataMember]
        public float Decay {
            get {
                return this._decay;
            }

            set {
                this._decay = MathHelper.Max(0f, value);
            }
        }

        /// <summary>
        /// Gets or sets the release time.
        /// </summary>
        /// <value>The release time.</value>
        [DataMember]
        public float Release {
            get {
                return this._release;
            }

            set {
                this._release = MathHelper.Max(0f, value);
            }
        }

        /// <summary>
        /// Gets or sets the sustain level. This is a value between 0.0 and 1.0 representing the
        /// volume, with 1.0 being max volume.
        /// </summary>
        /// <value>The sustain level.</value>
        [DataMember]
        public float Sustain {
            get {
                return this._sustain;
            }

            set {
                this._sustain = MathHelper.Clamp(value, 0f, 1f);
            }
        }
    }
}