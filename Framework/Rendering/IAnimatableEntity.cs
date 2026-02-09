namespace Macabre2D.Framework;

using System;
using Macabresoft.Core;

/// <summary>
/// Interface for an entity which animates with frames.
/// </summary>
public interface IAnimatableEntity : IEntity {
    /// <summary>
    /// Event for when <see cref="ShouldAnimate"/> changes.
    /// </summary>
    event EventHandler? ShouldAnimateChanged;
    
    /// <summary>
    /// Gets the frame rate override.
    /// </summary>
    ByteOverride FrameRateOverride { get; }

    /// <summary>
    /// Gets a value indicating whether this should animate.
    /// </summary>
    bool ShouldAnimate { get; }

    /// <summary>
    /// Increments the time of this animation forward. Only applicable when there exists an override.
    /// </summary>
    /// <param name="frameTime">The frame time.</param>
    void IncrementTime(FrameTime frameTime);

    /// <summary>
    /// Pushes the current animation to the next frame.
    /// </summary>
    void NextFrame();
}