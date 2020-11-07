namespace Macabresoft.Macabre2D.Editor.Library.MonoGame.Components {

    using Avalonia.Input;
    using Macabresoft.Macabre2D.Editor.AvaloniaInterop;
    using Macabresoft.Macabre2D.Framework;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;
    using System;

    public sealed class CameraControlComponent : GameUpdateableComponent {
        private CameraComponent _camera;

        public override void Initialize(IGameEntity entity) {
            base.Initialize(entity);

            if (!this.Entity.TryGetComponent(out this._camera)) {
                throw new ArgumentNullException(nameof(this._camera));
            }
        }

        public override void Update(FrameTime frameTime, InputState inputState) {
            if (this.Entity.Scene.Game is IAvaloniaGame game) {
                var mouseState = inputState.CurrentMouseState;
                var keyboardState = inputState.CurrentKeyboardState;
                var previousMouseState = inputState.PreviousMouseState;
                if (mouseState.ScrollWheelValue != previousMouseState.ScrollWheelValue) {
                    var scrollViewChange = (float)(frameTime.SecondsPassed * (previousMouseState.ScrollWheelValue - mouseState.ScrollWheelValue) * Math.Sqrt(this._camera.ViewHeight) * 2f);

                    var isZoomIn = scrollViewChange < 0;
                    if (isZoomIn) {
                        this._camera.ZoomTo(mouseState.Position, scrollViewChange * -1f);
                    }
                    else {
                        this._camera.ViewHeight += scrollViewChange;
                    }
                }

                ////if (this._game.IsMouseOver) {
                if (mouseState.MiddleButton == ButtonState.Pressed && previousMouseState.MiddleButton == ButtonState.Pressed) {
                    if (mouseState.Position != previousMouseState.Position) {
                        var oldPosition = this._camera.ConvertPointFromScreenSpaceToWorldSpace(previousMouseState.Position);
                        var mousePosition = this._camera.ConvertPointFromScreenSpaceToWorldSpace(mouseState.Position);

                        var distance = oldPosition - mousePosition;
                        this._camera.Entity.LocalPosition += distance;
                    }

                    if (game.CursorType == StandardCursorType.None) {
                        game.CursorType = StandardCursorType.SizeAll;
                    }
                }
                else if (game.CursorType == StandardCursorType.SizeAll) {
                    game.CursorType = StandardCursorType.None;
                }

                if (!keyboardState.IsModifierKeyDown()) {
                    var movementMultiplier = (float)frameTime.SecondsPassed * this._camera.ViewHeight;
                    if (keyboardState.IsKeyDown(Keys.W)) {
                        this.Entity.LocalPosition += new Vector2(0f, movementMultiplier);
                    }

                    if (keyboardState.IsKeyDown(Keys.A)) {
                        this.Entity.LocalPosition += new Vector2(movementMultiplier * -1f, 0f);
                    }

                    if (keyboardState.IsKeyDown(Keys.S)) {
                        this.Entity.LocalPosition += new Vector2(0f, movementMultiplier * -1f);
                    }

                    if (keyboardState.IsKeyDown(Keys.D)) {
                        this.Entity.LocalPosition += new Vector2(movementMultiplier, 0f);
                    }
                }
            }
        }
    }
}