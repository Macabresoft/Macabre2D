namespace Macabre2D.Framework {

    using System;

    /// <summary>
    /// An oscillator that uses random numbers to create noise (white, brown, pink; you choose!).
    /// </summary>
    /// <seealso cref="Macabre2D.Framework.IOscillator"/>
    public sealed class WhiteNoiseOscillator : IOscillator {
        private Random _random = new Random();

        /// <inheritdoc/>
        public float GetSignal(float time, float frequency, float volume) {
            return (float)(this._random.NextDouble() - this._random.NextDouble()) * volume;
        }
    }
}