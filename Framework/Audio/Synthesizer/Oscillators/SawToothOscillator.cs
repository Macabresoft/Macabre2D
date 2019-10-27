namespace Macabre2D.Framework {

    using System;

    /// <summary>
    /// An oscillator that uses a saw tooth wave to create sound.
    /// </summary>
    /// <seealso cref="Macabre2D.Framework.IOscillator"/>
    public sealed class SawToothOscillator : IOscillator {

        /// <inheritdoc/>
        public double GetSignal(double time, double frequency, double volume) {
            return 2D * (time * frequency - Math.Floor(time * frequency + 0.5D)) * volume;
        }
    }
}