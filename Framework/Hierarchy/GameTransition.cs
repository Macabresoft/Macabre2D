namespace Macabresoft.Macabre2D.Framework;

using System;
using System.Threading.Tasks;

/// <summary>
/// Represents a transition that the game loop handles.
/// </summary>
public class GameTransition {
    private readonly Action _finishedCallBack;
    private readonly Task _task;
    private readonly GameTimer _timer;

    /// <summary>
    /// Initializes a new instance of the <see cref="GameTransition" /> class.
    /// </summary>
    /// <param name="transitionSeconds">The seconds for the transition to last.</param>
    public GameTransition(float transitionSeconds) : this(transitionSeconds, () => { }) {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GameTransition" /> class.
    /// </summary>
    /// <param name="transitionSeconds">The seconds for the transition to last.</param>
    /// <param name="finishedCallBack">A callback to be called when the timer finishes.</param>
    public GameTransition(float transitionSeconds, Action finishedCallBack) : this(Task.CompletedTask, transitionSeconds, finishedCallBack) {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GameTransition" /> class.
    /// </summary>
    /// <param name="task">The task to run.</param>
    /// <param name="minimumTransitionSeconds">The minimum transition time in seconds.</param>
    /// <param name="finishedCallBack">A callback to be called when the task is finished.</param>
    public GameTransition(Task task, float minimumTransitionSeconds, Action finishedCallBack) {
        this._task = task;
        this._finishedCallBack = finishedCallBack;
        this._timer = new GameTimer(minimumTransitionSeconds);
        this._timer.Restart();
    }

    /// <summary>
    /// Gets a value indicating whether this transition is complete.
    /// </summary>
    public bool IsComplete { get; private set; }

    /// <summary>
    /// Cancels this transition.
    /// </summary>
    public void Cancel() {
        this.IsComplete = true;
    }

    /// <summary>
    /// Completes this transition, invoking the callback.
    /// </summary>
    public void Complete() {
        this._finishedCallBack.Invoke();
        this.IsComplete = true;
    }

    /// <summary>
    /// Updates this instance.
    /// </summary>
    /// <param name="frameTime">The frame time.</param>
    public void Update(FrameTime frameTime) {
        if (!this.IsComplete) {
            if (this._timer.State == TimerState.Running) {
                this._timer.Increment(frameTime);
            }
            else if (this._task.IsCompleted || this._task.IsCanceled || this._task.IsFaulted) {
                this._finishedCallBack.Invoke();
                this.IsComplete = true;
            }
        }
    }
}