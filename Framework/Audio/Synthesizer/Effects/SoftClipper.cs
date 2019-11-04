namespace Macabre2D.Framework {

    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Performs soft clipping on samples.
    /// </summary>
    /// <seealso cref="Macabre2D.Framework.Audio.Synthesizer.Effects.IAudioEffect"/>
    [DataContract]
    public sealed class SoftClipper : IAudioEffect {

        /// <summary>
        /// Gets or sets the pre gain.
        /// </summary>
        /// <value>The pre gain.</value>
        [DataMember]
        public float PreGain { get; set; } = 1f;

        /// <inheritdoc/>
        public float ApplyEffect(float sample) {
            return (float)(2f / Math.PI) * (float)Math.Atan(this.PreGain * sample);
        }
    }
}