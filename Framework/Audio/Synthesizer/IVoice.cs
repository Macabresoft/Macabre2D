namespace Macabre2D.Framework {

    using System;

    /// <summary>
    /// An interface for voices to be used by a <see cref="VoicePool"/>.
    /// </summary>
    public interface IVoice {

        /// <summary>
        /// Occurs when the note is finished playing.
        /// </summary>
        event EventHandler OnFinished;

        /// <summary>
        /// Gets the next samples.
        /// </summary>
        /// <returns>The next samples.</returns>
        AudioSample[] GetNextSamples();

        /// <summary>
        /// Reinitializes the specified instrument.
        /// </summary>
        /// <param name="song">The song.</param>
        /// <param name="track">The track.</param>
        /// <param name="note">The note.</param>
        void Reinitialize(Song song, Track track, NoteInstance note);
    }
}