namespace Macabresoft.Macabre2D.Framework;

using System;
using System.Collections.Generic;
using Macabresoft.Core;

/// <summary>
/// A system which runs routines over multiple frames.
/// </summary>
public class RoutineSystem : GameSystem, IUpdateSystem {
    private readonly List<Routine> _routines = [];

    /// <inheritdoc />
    public event EventHandler? ShouldUpdateChanged;

    /// <inheritdoc />
    public UpdateSystemKind Kind => UpdateSystemKind.Update;

    /// <inheritdoc />
    public bool ShouldUpdate {
        get;
        private set {
            if (value != field) {
                field = value;
                this.ShouldUpdateChanged.SafeInvoke(this);
            }
        }
    } = false;

    /// <summary>
    /// Runs the routine until it is finished.
    /// </summary>
    /// <param name="routine">The routine.</param>
    public void Run(Routine routine) {
        this._routines.Add(routine);
        this.ShouldUpdate = true;
    }

    /// <inheritdoc />
    public void Update(FrameTime frameTime, InputState inputState) {
        var removed = new List<Routine>();
        foreach (var routine in this._routines) {
            if (routine.IsCanceled || routine.Run(frameTime)) {
                removed.Add(routine);
            }
        }

        foreach (var routine in removed) {
            this._routines.Remove(routine);
        }

        this.ShouldUpdate = this._routines.Count != 0;
    }
}