namespace Macabre2D.Framework {

    using System;

    /// <summary>
    /// An oscillator that uses a saw tooth wave to create sound.
    /// </summary>
    /// <seealso cref="Macabre2D.Framework.IOscillator"/>
    public sealed class SawToothOscillator : IOscillator {

        /// <inheritdoc/>
        public float GetSignal(float time, float frequency, float volume) {
            return 2f * (time * frequency - (float)Math.Floor(time * frequency + 0.5f)) * volume;
        }
    }
}