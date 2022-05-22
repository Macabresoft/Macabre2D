namespace Macabresoft.Macabre2D.Framework;

using System.Runtime.Serialization;

/// <summary>
/// Gets the state of the timer.
/// </summary>
public enum TimerState {
    Disabled,
    Running,
    Finished
}

/// <summary>
/// A timer that can be disabled and has a limit
/// </summary>
[DataContract]
public class GameTimer : NotifyPropertyChanged {
    private float _timeLimit;

    /// <summary>
    /// Gets the state of this timer.
    /// </summary>
    public TimerState State { get; private set; } = TimerState.Disabled;

    /// <summary>
    /// Gets or sets the time limit of this timer in seconds.
    /// </summary>
    [DataMember]
    public float TimeLimit {
        get => this._timeLimit;
        set => this.Set(ref this._timeLimit, value);
    }

    /// <summary>
    /// Gets the number of seconds this has been running.
    /// </summary>
    public float TimeRunning { get; private set; }

    /// <summary>
    /// Completes this timer prematurely.
    /// </summary>
    public void Complete() {
        this.State = TimerState.Finished;
        this.TimeRunning = this.TimeLimit;
    }

    /// <summary>
    /// Decrements the timer.
    /// </summary>
    /// <param name="frameTime">The frame time.</param>
    public void Decrement(FrameTime frameTime) {
        if (this.State != TimerState.Disabled) {
            this.TimeRunning -= (float)frameTime.SecondsPassed;

            if (this.State == TimerState.Finished && this.TimeRunning < this.TimeLimit) {
                this.State = TimerState.Running;
            }

            if (this.TimeRunning <= 0f) {
                this.TimeRunning = 0f;
            }
        }
    }

    /// <summary>
    /// Increments the timer.
    /// </summary>
    /// <param name="frameTime">The frame time.</param>
    public void Increment(FrameTime frameTime) {
        if (this.State == TimerState.Running) {
            this.TimeRunning += (float)frameTime.SecondsPassed;

            if (this.TimeRunning >= this.TimeLimit) {
                this.TimeRunning = this.TimeLimit;
                this.State = TimerState.Finished;
            }
        }
    }

    /// <summary>
    /// Pauses this timer.
    /// </summary>
    public void Pause() {
        this.State = TimerState.Disabled;
    }

    /// <summary>
    /// Resets this timer.
    /// </summary>
    public void Reset() {
        this.TimeRunning = 0f;

        if (this.State == TimerState.Finished) {
            this.State = TimerState.Running;
        }
    }

    /// <summary>
    /// Restarts this timer.
    /// </summary>
    /// <param name="frameTime">The frame time.</param>
    public void Restart(FrameTime frameTime) {
        this.Restart();
        this.Increment(frameTime);
    }

    /// <summary>
    /// Restarts the timer with zero time running.
    /// </summary>
    public void Restart() {
        this.State = TimerState.Running;
        this.TimeRunning = 0f;
    }

    /// <summary>
    /// Starts the timer form wherever it left off.
    /// </summary>
    /// <param name="frameTime">The frame time.</param>
    public void Start(FrameTime frameTime) {
        if (this.State == TimerState.Disabled) {
            this.State = TimerState.Running;
        }

        this.Increment(frameTime);
    }

    /// <summary>
    /// Stops this timer.
    /// </summary>
    public void Stop() {
        this.Pause();
        this.TimeRunning = 0f;
    }
}