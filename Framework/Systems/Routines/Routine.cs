namespace Macabre2D.Framework;

/// <summary>
/// Represents a routine that can be run over multiple frames.
/// </summary>
public abstract class Routine {
    /// <summary>
    /// Gets a value indicating whether this routine has been canceled.
    /// </summary>
    public bool IsCanceled { get; private set; }

    /// <summary>
    /// Cancels this routine as of its next scheduled run.
    /// </summary>
    public void Cancel() {
        this.IsCanceled = true;
    }

    /// <summary>
    /// Runs the routine and returns a value indicating whether it is complete.
    /// </summary>
    /// <param name="frameTime">The frame time.</param>
    /// <returns>A value indicating whether the routine was finished.</returns>
    public bool Run(FrameTime frameTime) => this.IsCanceled || this.RunRoutine(frameTime);

    protected abstract bool RunRoutine(FrameTime frameTime);
}