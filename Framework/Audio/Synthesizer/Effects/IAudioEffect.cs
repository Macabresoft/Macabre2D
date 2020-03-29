namespace Macabre2D.Framework {

    /// <summary>
    /// An audio effect that can be applied to samples.
    /// </summary>
    public interface IAudioEffect {

        /// <summary>
        /// Applies the effect to the specified sample.
        /// </summary>
        /// <param name="sample">The sample.</param>
        /// <param name="time">The time into playing the effected audio.</param>
        /// <returns>A new sample with the effect applied.</returns>
        float ApplyEffect(float sample, float time);
    }
}