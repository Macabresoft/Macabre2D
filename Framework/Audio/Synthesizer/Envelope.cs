namespace Macabre2D.Framework {

    using Microsoft.Xna.Framework;
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// An envelope that allows control over the volume of sound being played over a note.
    /// </summary>
    [DataContract]
    public sealed class Envelope {
        private const ushort MaximumAttackTime = 5000;
        private const ushort MaximumDecayTime = 5000;
        private const ushort MaximumReleaseTime = 10000;
        private ushort _attack = 50;
        private ushort _decay;
        private float _peakAmplitude = 0.8f;
        private ushort _release = 50;
        private float _sustainAmplitude = 0.5f;

        /// <summary>
        /// Gets or sets attack length in milliseconds.
        /// </summary>
        /// <value>The attack time.</value>
        [DataMember]
        public ushort Attack {
            get {
                return this._attack;
            }

            set {
                this._attack = Math.Min(value, MaximumAttackTime);
            }
        }

        /// <summary>
        /// Gets or sets decay length in milliseconds.
        /// </summary>
        /// <value>The decay time.</value>
        [DataMember]
        public ushort Decay {
            get {
                return this._decay;
            }

            set {
                this._decay = Math.Min(value, MaximumDecayTime);
            }
        }

        /// <summary>
        /// Gets or sets the peak amplitude. This value is only relevant if <see cref="Decay"/> is
        /// present. Cannot be less than <see cref="SustainAmplitude"/> and cannot be more than 1.0.
        /// </summary>
        /// <value>The sustain level.</value>
        [DataMember(Name = "Peak Amplitude")]
        public float PeakAmplitude {
            get {
                return this._peakAmplitude;
            }

            set {
                this._peakAmplitude = MathHelper.Clamp(value, this.SustainAmplitude, 1f);
            }
        }

        /// <summary>
        /// Gets or sets release length in samples.
        /// </summary>
        /// <value>The release time.</value>
        [DataMember]
        public ushort Release {
            get {
                return this._release;
            }

            set {
                this._release = Math.Min(value, MaximumReleaseTime);
            }
        }

        /// <summary>
        /// Gets or sets the sustain amplitude. This is a value between 0.0 and 1.0 representing the
        /// volume, with 1.0 being max volume.
        /// </summary>
        /// <value>The sustain level.</value>
        [DataMember(Name = "Sustain Amplitude")]
        public float SustainAmplitude {
            get {
                return this._sustainAmplitude;
            }

            set {
                this._sustainAmplitude = MathHelper.Clamp(value, 0f, 1f);
                this.PeakAmplitude = this._peakAmplitude;
            }
        }
    }
}