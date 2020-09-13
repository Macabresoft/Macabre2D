namespace Macabresoft.MonoGame.Core {

    /// <summary>
    /// A system which does a sorted update loop over enabled updateable components.
    /// </summary>
    public class UpdateSystem : GameSystem {

        /// <inheritdoc />
        public override SystemLoop Loop => SystemLoop.Update;

        /// <inheritdoc />
        public override void Update(FrameTime frameTime, InputState inputState) {
            foreach (var component in this.Scene.UpdateableComponents) {
                component.Update(frameTime, inputState);
            }
        }
    }
}