namespace Macabre2D.Framework {

    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class SampleHelper {
        public const ushort BytesPerSample = 2;
        private const ushort NumberOfChannels = 2;
        private static readonly IAudioEffect _clipper = new SoftClipperEffect();

        /// <summary>
        /// Gets the samples from the provided voices to be pushed into the audio buffer.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="voices">The voices.</param>
        /// <param name="numberOfSamples">The number of samples.</param>
        /// <param name="trackVolumeMultiplier">The track volume multiplier.</param>
        /// <returns>The samples for the audio buffer.</returns>
        public static byte[] GetBufferSamples<T>(ICollection<T> voices, ushort numberOfSamples, float trackVolumeMultiplier) where T : IVoice {
            var voiceSamples = voices.Select(x => x.GetBuffer(numberOfSamples)).ToList();
            var samples = new byte[numberOfSamples * NumberOfChannels * BytesPerSample];

            for (var i = 0; i < numberOfSamples; i++) {
                var relevantSamples = voiceSamples.Select(x => x[i]).ToList();
                var leftChannel = SampleHelper._clipper.ApplyEffect(trackVolumeMultiplier * relevantSamples.Sum(x => x.LeftChannel), 0f);
                var rightChannel = SampleHelper._clipper.ApplyEffect(trackVolumeMultiplier * relevantSamples.Sum(x => x.RightChannel), 0f);
                var leftShort = (short)(leftChannel >= 0.0f ? leftChannel * short.MaxValue : leftChannel * -short.MinValue);
                var rightShort = (short)(rightChannel >= 0.0f ? rightChannel * short.MaxValue : rightChannel * -short.MinValue);
                var index = i * NumberOfChannels * BytesPerSample;

                if (!BitConverter.IsLittleEndian) {
                    samples[index] = (byte)(leftShort >> 8);
                    samples[index + 1] = (byte)leftShort;
                    samples[index + 2] = (byte)(rightShort >> 8);
                    samples[index + 3] = (byte)rightShort;
                }
                else {
                    samples[index] = (byte)leftShort;
                    samples[index + 1] = (byte)(leftShort >> 8);
                    samples[index + 2] = (byte)rightShort;
                    samples[index + 3] = (byte)(rightShort >> 8);
                }
            }

            return samples;
        }
    }
}