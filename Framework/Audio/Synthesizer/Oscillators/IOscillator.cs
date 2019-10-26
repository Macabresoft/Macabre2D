namespace Macabre2D.Framework {

    /// <summary>
    /// An oscillator that can be used by an <see cref="Instrument"/> to make sounds.
    /// </summary>
    public interface IOscillator {

        /// <summary>
        /// Gets an audio signal given the current time, frequency, and volume.
        /// </summary>
        /// <param name="time">The time.</param>
        /// <param name="frequency">The frequency.</param>
        /// <param name="volume">The volume. This is a value between -1 and 1.</param>
        /// <returns>The audio signal.</returns>
        float GetSignal(float time, float frequency, float volume);
    }
}