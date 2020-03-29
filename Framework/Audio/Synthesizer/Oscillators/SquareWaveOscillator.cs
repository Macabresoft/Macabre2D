namespace Macabre2D.Framework {

    using System;

    /// <summary>
    /// An oscillator that uses a square wave to create sound.
    /// </summary>
    /// <seealso cref="Macabre2D.Framework.IOscillator"/>
    public sealed class SquareWaveOscillator : IOscillator {

        /// <inheritdoc/>
        public float GetSignal(float time, float frequency, float amplitude) {
            return Math.Sin(frequency * time * 2D * Math.PI) >= 0D ? amplitude : -amplitude;
        }
    }
}