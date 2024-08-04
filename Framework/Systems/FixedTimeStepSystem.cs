namespace Macabresoft.Macabre2D.Framework;

using System;
using System.ComponentModel;
using System.Runtime.Serialization;

/// <summary>
/// A system which updates at a fixed time step.
/// </summary>
/// <seealso cref="UpdateSystem" />
[Category(CommonCategories.Timing)]
public abstract class FixedTimeStepSystem : UpdateSystem {
    private float _timePassed;
    private float _timeStep = 1f / 30f;

    /// <summary>
    /// Gets the time step.
    /// </summary>
    /// <value>The time step.</value>
    /// <exception cref="NotSupportedException">Time step must be greater than 0.</exception>
    [DataMember]
    public float TimeStep {
        get => this._timeStep;
        set => this._timeStep = Math.Max(0f, value);
    }

    /// <inheritdoc />
    public override void Update(FrameTime frameTime, InputState inputState) {
        this._timePassed += (float)frameTime.SecondsPassed;

        while (this._timePassed >= this._timeStep) {
            this.FixedUpdate(frameTime, inputState);
            this._timePassed -= this._timeStep;
        }
    }

    /// <summary>
    /// An update that is called after a fixed amount of time.
    /// </summary>
    /// <param name="frameTime">The frame time.</param>
    /// <param name="inputState">State of the input.</param>
    protected abstract void FixedUpdate(FrameTime frameTime, InputState inputState);
}