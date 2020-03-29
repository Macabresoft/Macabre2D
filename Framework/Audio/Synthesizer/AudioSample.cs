namespace Macabre2D.Framework {

    using System.Runtime.Serialization;

    /// <summary>
    /// A single sample of audio.
    /// </summary>
    [DataContract]
    public struct AudioSample {

        /// <summary>
        /// The left channel.
        /// </summary>
        [DataMember]
        public readonly float LeftChannel;

        /// <summary>
        /// The right channel.
        /// </summary>
        [DataMember]
        public readonly float RightChannel;

        /// <summary>
        /// Initializes a new instance of the <see cref="AudioSample"/> class.
        /// </summary>
        /// <param name="leftChannel">The left channel.</param>
        /// <param name="rightChannel">The right channel.</param>
        public AudioSample(float leftChannel, float rightChannel) {
            this.LeftChannel = leftChannel.Clamp(-1f, 1f);
            this.RightChannel = rightChannel.Clamp(-1f, 1f);
        }
    }
}