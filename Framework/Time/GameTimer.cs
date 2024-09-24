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
public class GameTimer {
    /// <summary>
    /// Initializes a new instance of the <see cref="GameTimer" /> class.
    /// </summary>
    public GameTimer() {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GameTimer" /> class.
    /// </summary>
    /// <param name="timeLimit">The time limit.</param>
    public GameTimer(float timeLimit) {
        this.TimeLimit = timeLimit;
    }

    /// <summary>
    /// Gets the percentage complete.
    /// </summary>
    public float PercentComplete {
        get {
            if (this.TimeLimit > 0f) {
                return this.TimeRunning / this.TimeLimit;
            }

            return 0f;
        }
    }

    /// <summary>
    /// Gets the number of seconds remaining.
    /// </summary>
    public float TimeRemaining => this.TimeLimit - this.TimeRunning;

    /// <summary>
    /// Gets the state of this timer.
    /// </summary>
    public TimerState State { get; private set; } = TimerState.Disabled;

    /// <summary>
    /// Gets or sets the time limit of this timer in seconds.
    /// </summary>
    [DataMember]
    public float TimeLimit { get; set; }

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
        this.Decrement((float)frameTime.SecondsPassed);
    }

    /// <summary>
    /// Decrements the timer by a multiplied frame time.
    /// </summary>
    /// <param name="frameTime">The frame time.</param>
    /// <param name="multiplier">
    /// The multiplier by which to apply to the seconds passed. For instance, if the multiplier is 0.5
    /// and the seconds passed is 10, this timer will be decremented by 5 seconds.
    /// </param>
    public void Decrement(FrameTime frameTime, float multiplier) {
        this.Decrement((float)(frameTime.SecondsPassed * multiplier));
    }

    /// <summary>
    /// Increments the timer.
    /// </summary>
    /// <param name="frameTime">The frame time.</param>
    public void Increment(FrameTime frameTime) {
        this.Increment((float)frameTime.SecondsPassed);
    }

    /// <summary>
    /// Increments the timer by a multiplied frame time.
    /// </summary>
    /// <param name="frameTime">The frame time.</param>
    /// <param name="multiplier">
    /// The multiplier by which to apply to the seconds passed. For instance, if the multiplier is 0.5
    /// and the seconds passed is 10, this timer will be incremented by 5 seconds.
    /// </param>
    public void Increment(FrameTime frameTime, float multiplier) {
        this.Increment((float)(frameTime.SecondsPassed * multiplier));
    }

    /// <summary>
    /// Increments the timer.
    /// </summary>
    /// <param name="seconds">The seconds.</param>
    public void Increment(float seconds) {
        if (this.State == TimerState.Running) {
            this.TimeRunning += seconds;

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

    private void Decrement(float seconds) {
        if (this.State != TimerState.Disabled) {
            this.TimeRunning -= seconds;

            if (this.State == TimerState.Finished && this.TimeRunning < this.TimeLimit) {
                this.State = TimerState.Running;
            }

            if (this.TimeRunning <= 0f) {
                this.TimeRunning = 0f;
            }
        }
    }
}