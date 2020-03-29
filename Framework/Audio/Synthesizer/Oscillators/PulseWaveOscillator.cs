namespace Macabre2D.Framework {

    using Microsoft.Xna.Framework;
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// An oscillator that uses a pulse wave to create sound.
    /// </summary>
    /// <seealso cref="Macabre2D.Framework.IOscillator"/>
    [DataContract]
    public sealed class PulseWaveOscillator : IOscillator {
        private float _dutyCycle;

        /// <summary>
        /// Gets or sets the duty cycle. This value must be between 0 and 0.5.
        /// </summary>
        /// <value>The duty cycle.</value>
        [DataMember]
        public float DutyCycle {
            get {
                return this._dutyCycle;
            }

            set {
                this._dutyCycle = MathHelper.Clamp(value, 0f, 0.5f);
            }
        }

        /// <inheritdoc/>
        public float GetSignal(float time, float frequency, float amplitude) {
            var period = 1D / frequency; // frequency should be clamped to above 0 somewhere else
            var phase = (time - Math.Floor(time / period) * period) / period;
            return phase <= this.DutyCycle ? amplitude : -amplitude;
        }
    }
}