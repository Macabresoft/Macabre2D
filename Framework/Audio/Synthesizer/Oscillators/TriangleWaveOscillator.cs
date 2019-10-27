namespace Macabre2D.Framework {

    using System;

    /// <summary>
    /// An oscillator that uses triangle waves to create sound.
    /// </summary>
    /// <seealso cref="Macabre2D.Framework.IOscillator"/>
    public sealed class TriangleWaveOscillator : IOscillator {

        /// <inheritdoc/>
        public double GetSignal(double time, double frequency, double volume) {
            return 2D * volume * Math.Abs(2f * (time * frequency - Math.Floor(time * frequency + 0.5D))) - volume;
        }
    }
}