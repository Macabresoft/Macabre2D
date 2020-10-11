namespace Macabresoft.MonoGame.Samples.AvaloniaWindow.Components {

    using Macabresoft.MonoGame.Core2D;
    using Microsoft.Xna.Framework.Input;
    using System.Linq;

    public sealed class SampleInputComponent : GameUpdateableComponent {
        private SpriteRenderComponent _skullRenderer;

        public override void Initialize(IGameEntity entity) {
            base.Initialize(entity);

            this.Entity.TryGetComponent<SpriteRenderComponent>(out this._skullRenderer);
        }

        public override void Update(FrameTime frameTime, InputState inputState) {
            if (inputState.CurrentMouseState.LeftButton == ButtonState.Pressed && inputState.PreviousMouseState.LeftButton == ButtonState.Released) {
                this.Entity.Scene.BackgroundColor = DefinedColors.MacabresoftBlack;
            }
            else if (inputState.CurrentMouseState.LeftButton == ButtonState.Released && inputState.PreviousMouseState.LeftButton == ButtonState.Pressed) {
                this.Entity.Scene.BackgroundColor = DefinedColors.MacabresoftPurple;
            }

            if (inputState.CurrentKeyboardState.GetPressedKeys().Any()) {
                this._skullRenderer.Color = DefinedColors.MacabresoftYellow;
            }
            else {
                this._skullRenderer.Color = DefinedColors.MacabresoftBone;
            }
        }
    }
}