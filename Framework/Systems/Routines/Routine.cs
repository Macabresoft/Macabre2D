namespace Macabresoft.Macabre2D.Framework.Routines;

using System;

/// <summary>
/// Represents a routine that can be run over multiple frames.
/// </summary>
public sealed class Routine {
    private readonly Func<FrameTime, bool> _routine;

    /// <summary>
    /// Initializes a new instance of the <see cref="Routine" /> class.
    /// </summary>
    /// <param name="routine">The routine. This takes in a <see cref="FrameTime" /> and returns a <see cref="bool" /> indicating whether it is finished running.</param>
    public Routine(Func<FrameTime, bool> routine) {
        this._routine = routine;
    }

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
    public bool Run(FrameTime frameTime) => this._routine(frameTime);
}