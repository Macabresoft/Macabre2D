namespace Macabre2D.Framework {

    using System;

    /// <summary>
    /// An oscillator that uses sine waves to generate noise.
    /// </summary>
    /// <seealso cref="Macabre2D.Framework.IOscillator"/>
    public sealed class SineWaveOscillator : IOscillator {

        /// <inheritdoc/>
        public double GetSignal(double time, double frequency, double volume) {
            return Math.Sin(frequency * time * 2D * Math.PI) * volume;
        }
    }
}