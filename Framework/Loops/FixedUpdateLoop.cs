namespace Macabresoft.Macabre2D.Framework;

/// <summary>
/// An update loop for <see cref="IFixedUpdateableEntity"/> updates on a fixed time step.
/// </summary>
public class FixedUpdateLoop : FixedTimeStepLoop {
    /// <inheritdoc />
    protected override void FixedUpdate(FrameTime frameTime, InputState inputState) {
        foreach (var entity in this.Scene.FixedUpdateableEntities) {
            entity.FixedUpdate(this.TimeStep);
        }
    }
}