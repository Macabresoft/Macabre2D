namespace Macabre2D.Framework {

    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// An oscillator that uses a pulse wave to create sound.
    /// </summary>
    /// <seealso cref="Macabre2D.Framework.IOscillator"/>
    [DataContract]
    public sealed class PulseWaveOscillator : IOscillator {
        private double _dutyCycle;

        /// <summary>
        /// Gets or sets the duty cycle. This value must be between 0 and 0.5.
        /// </summary>
        /// <value>The duty cycle.</value>
        [DataMember]
        public double DutyCycle {
            get {
                return this._dutyCycle;
            }

            set {
                this._dutyCycle = Math.Max(0D, Math.Min(value, 0.5D));
            }
        }

        /// <inheritdoc/>
        public double GetSignal(double time, double frequency, double volume) {
            var period = 1D / frequency; // frequency should be clamped to above 0 somewhere else
            var phase = (time - Math.Floor(time / period) * period) / period;
            return phase <= this.DutyCycle ? volume : -volume;
        }
    }
}