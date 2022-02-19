namespace Macabresoft.Macabre2D.Framework; 

/// <summary>
/// A system which does a sorted update loop over enabled updateable entities.
/// </summary>
public class UpdateSystem : LoopSystem {
    /// <inheritdoc />
    public override SystemKind Kind => SystemKind.Update;

    /// <inheritdoc />
    public override void Update(FrameTime frameTime, InputState inputState) {
        foreach (var entity in this.Scene.UpdateableEntities) {
            entity.Update(frameTime, inputState);
        }
    }
}