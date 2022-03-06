namespace Macabresoft.Macabre2D.Framework; 

/// <summary>
/// A loop that calls updates on entities.
/// </summary>
public class UpdateLoop : Loop {
    /// <inheritdoc />
    public override LoopKind Kind => LoopKind.Update;

    /// <inheritdoc />
    public override void Update(FrameTime frameTime, InputState inputState) {
        foreach (var entity in this.Scene.UpdateableEntities) {
            entity.Update(frameTime, inputState);
        }
    }
}