namespace Macabre2D.Framework;

/// <summary>
/// An update system for <see cref="IFixedUpdateableEntity" /> updates on a fixed time step.
/// </summary>
public class FixedUpdateSystem : FixedTimeStepSystem {
    /// <inheritdoc />
    protected override void FixedUpdate(FrameTime frameTime, InputState inputState) {
        foreach (var entity in this.Scene.FixedUpdateableEntities) {
            entity.FixedUpdate(this.TimeStep);
        }
    }
}