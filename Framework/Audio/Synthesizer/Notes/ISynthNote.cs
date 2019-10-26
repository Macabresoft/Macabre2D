namespace Macabre2D.Framework {

    /// <summary>
    /// A note to be placed on a track and played by an <see cref="Instrument"/>.
    /// </summary>
    public interface ISynthNote {

        /// <summary>
        /// Gets the length in beats.
        /// </summary>
        /// <value>The length.</value>
        ulong Length { get; }

        /// <summary>
        /// Gets the frequency.
        /// </summary>
        /// <param name="percentage">
        /// The percentage of the note that has been played. A number between 0 and 1.
        /// </param>
        /// <returns>The frequency</returns>
        float GetFrequency(float percentage);
    }
}