namespace Macabresoft.Macabre2D.Samples.AvaloniaWindow.Entities {
    using System.Linq;
    using Macabresoft.Macabre2D.Framework;
    using Microsoft.Xna.Framework.Input;

    public sealed class SampleInputEntity : UpdateableEntity {
        private SpriteRenderer _skullRenderer;

        public override void Initialize(IScene scene, IEntity entity) {
            base.Initialize(scene, entity);
            this.TryGetParentEntity(out this._skullRenderer);
        }

        public override void Update(FrameTime frameTime, InputState inputState) {
            if (inputState.CurrentMouseState.LeftButton == ButtonState.Pressed && inputState.PreviousMouseState.LeftButton == ButtonState.Released) {
                this.Scene.BackgroundColor = DefinedColors.MacabresoftBlack;
            }
            else if (inputState.CurrentMouseState.LeftButton == ButtonState.Released && inputState.PreviousMouseState.LeftButton == ButtonState.Pressed) {
                this.Scene.BackgroundColor = DefinedColors.MacabresoftPurple;
            }

            if (this._skullRenderer != null) {
                this._skullRenderer.Color = inputState.CurrentKeyboardState.GetPressedKeys().Any() ? DefinedColors.MacabresoftYellow : DefinedColors.MacabresoftBone;
            }
        }
    }
}