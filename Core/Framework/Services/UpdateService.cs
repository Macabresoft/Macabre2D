namespace Macabresoft.MonoGame.Core {

    /// <summary>
    /// A service which does a sorted update loop over enabled updateable components.
    /// </summary>
    public sealed class UpdateService : GameService {

        /// <inheritdoc />
        public override void Update(FrameTime frameTime, InputState inputState) {
            if (this.Scene != null) {
                foreach (var component in this.Scene.UpdateableComponents) {
                    component.Update(frameTime, inputState);
                }
            }
        }
    }
}