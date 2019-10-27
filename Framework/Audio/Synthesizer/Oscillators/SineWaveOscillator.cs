namespace Macabre2D.Framework {

    using System;

    /// <summary>
    /// An oscillator that uses sine waves to generate noise.
    /// </summary>
    /// <seealso cref="Macabre2D.Framework.IOscillator"/>
    public sealed class SineWaveOscillator : IOscillator {

        /// <inheritdoc/>
        public float GetSignal(float time, float frequency, float volume) {
            return (float)Math.Sin(frequency * time * 2 * Math.PI) * volume;
        }
    }
}