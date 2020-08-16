namespace Macabresoft.MonoGame.Core {

    using System;

    /// <summary>
    /// Interface for a component that is identifiable during the current session with a <see
    /// cref="int"/> and across sessions with a <see cref="Guid"/>
    /// </summary>
    public interface IIdentifiableComponent : IIdentifiable {

        /// <summary>
        /// Gets the session identifier. This identifier is unique per session. It is not guaranteed
        /// to be different across sessions, but it will remain the same for the whole time a scene
        /// is running.
        /// </summary>
        /// <value>The session identifier.</value>
        int SessionId { get; }
    }
}