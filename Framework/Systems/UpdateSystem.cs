namespace Macabresoft.Macabre2D.Framework {

    /// <summary>
    /// A system which does a sorted update loop over enabled updateable entities.
    /// </summary>
    public class UpdateSystem : UpdateableSystem {

        /// <inheritdoc />
        public override SystemLoop Loop => SystemLoop.Update;

        /// <inheritdoc />
        public override void Update(FrameTime frameTime, InputState inputState) {
            foreach (var entity in this.Scene.UpdateableEntities) {
                entity.Update(frameTime, inputState);
            }
        }
    }
}