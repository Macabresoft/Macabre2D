namespace Macabresoft.Macabre2D.Framework;

/// <summary>
/// Interface for an entity which animates with frames.
/// </summary>
public interface IAnimatableEntity : IEntity {
    /// <summary>
    /// Gets the current animation.
    /// </summary>
    public SpriteAnimation? CurrentAnimation { get; }

    /// <summary>
    /// Gets the frame rate override.
    /// </summary>
    ByteOverride FrameRateOverride { get; }

    /// <summary>
    /// Gets a value indicating whether this is looping on the current animation.
    /// </summary>
    bool IsLooping { get; }

    /// <summary>
    /// Gets a value indicating whether this is playing.
    /// </summary>
    bool IsPlaying { get; }

    /// <summary>
    /// Gets the percentage complete for the current animation.
    /// </summary>
    /// <returns>The percentage complete.</returns>
    float GetPercentageComplete();

    /// <summary>
    /// Increments the time of this animation forward. Only applicable when there exists a <see cref="FrameRateOverride" />.
    /// </summary>
    /// <param name="frameTime">The frame time.</param>
    void IncrementTime(FrameTime frameTime);

    /// <summary>
    /// Pushes the current animation to the next frame.
    /// </summary>
    void NextFrame();
}