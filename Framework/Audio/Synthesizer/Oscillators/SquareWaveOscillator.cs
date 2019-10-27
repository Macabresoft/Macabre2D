namespace Macabre2D.Framework {

    using System;

    /// <summary>
    /// An oscillator that uses a square wave to create sound.
    /// </summary>
    /// <seealso cref="Macabre2D.Framework.IOscillator"/>
    public sealed class SquareWaveOscillator : IOscillator {

        /// <inheritdoc/>
        public double GetSignal(double time, double frequency, double volume) {
            return Math.Sin(frequency * time * 2D * Math.PI) >= 0D ? volume : -volume;
        }
    }
}