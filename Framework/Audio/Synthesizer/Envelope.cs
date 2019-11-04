namespace Macabre2D.Framework {

    using Microsoft.Xna.Framework;
    using System.Runtime.Serialization;

    /// <summary>
    /// An envelope that allows control over the volume of sound being played over a note.
    /// </summary>
    [DataContract]
    public sealed class Envelope {
        private float _sustainVolume = 0.5f;

        /// <summary>
        /// Gets or sets attack length in samples.
        /// </summary>
        /// <value>The attack time.</value>
        [DataMember]
        public ushort Attack { get; set; }

        /// <summary>
        /// Gets or sets decay length in samples.
        /// </summary>
        /// <value>The decay time.</value>
        [DataMember]
        public ushort Decay { get; set; }

        /// <summary>
        /// Gets or sets release length in samples.
        /// </summary>
        /// <value>The release time.</value>
        [DataMember]
        public ushort Release { get; set; }

        /// <summary>
        /// Gets or sets the sustain level. This is a value between 0.0 and 1.0 representing the
        /// volume, with 1.0 being max volume.
        /// </summary>
        /// <value>The sustain level.</value>
        [DataMember(Name = "Sustain Volume")]
        public float SustainVolume {
            get {
                return this._sustainVolume;
            }

            set {
                this._sustainVolume = MathHelper.Clamp(value, 0f, 1f);
            }
        }
    }
}