namespace Macabresoft.Macabre2D.Framework;

using System;

/// <summary>
/// A routine that runs for a length of time and then makes a callback at the end of the timer.
/// </summary>
public class TimedRoutine : Routine {
    private readonly Action _finishedCallback;
    private readonly GameTimer _timer;

    /// <summary>
    /// Initializes a new instance of the <see cref="TimedRoutine" /> class.
    /// </summary>
    /// <param name="finishedCallback"></param>
    /// <param name="lengthInSeconds"></param>
    public TimedRoutine(Action finishedCallback, float lengthInSeconds) : base() {
        this._finishedCallback = finishedCallback;
        this._timer = new GameTimer(lengthInSeconds);
        this._timer.Restart();
    }

    /// <inheritdoc />
    protected override bool RunRoutine(FrameTime frameTime) {
        this._timer.Increment(frameTime);

        if (this._timer.State == TimerState.Finished) {
            this._finishedCallback();
            return true;
        }

        return false;
    }
}