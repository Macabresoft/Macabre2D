namespace Macabresoft.Macabre2D.UI.Common.MonoGame.Entities {
    using System;
    using Avalonia.Input;
    using Macabresoft.Macabre2D.UI.AvaloniaInterop;
    using Macabresoft.Macabre2D.Framework;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;

    internal sealed class CameraController : UpdateableEntity {
        private Camera _camera;

        public override void Initialize(IScene scene, IEntity entity) {
            base.Initialize(scene, entity);

            if (!this.TryGetParentEntity(out this._camera)) {
                throw new NotSupportedException("Could not find a camera ancestor.");
            }
        }

        public override void Update(FrameTime frameTime, InputState inputState) {
            if (this.Scene.Game is IAvaloniaGame game && this._camera != null) {
                var mouseState = inputState.CurrentMouseState;
                var keyboardState = inputState.CurrentKeyboardState;
                var previousMouseState = inputState.PreviousMouseState;
                if (mouseState.ScrollWheelValue != previousMouseState.ScrollWheelValue) {
                    var scrollViewChange = (float)(frameTime.SecondsPassed * (previousMouseState.ScrollWheelValue - mouseState.ScrollWheelValue) * this._camera.ViewHeight);

                    var isZoomIn = scrollViewChange < 0;
                    if (isZoomIn) {
                        this._camera.ZoomTo(mouseState.Position, scrollViewChange * -1f);
                    }
                    else {
                        this._camera.ViewHeight += scrollViewChange;
                    }
                }

                if (mouseState.MiddleButton == ButtonState.Pressed && previousMouseState.MiddleButton == ButtonState.Pressed) {
                    if (mouseState.Position != previousMouseState.Position) {
                        var oldPosition = this._camera.ConvertPointFromScreenSpaceToWorldSpace(previousMouseState.Position);
                        var mousePosition = this._camera.ConvertPointFromScreenSpaceToWorldSpace(mouseState.Position);

                        var distance = oldPosition - mousePosition;
                        this._camera.LocalPosition += distance;
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
                        this._camera.LocalPosition += new Vector2(0f, movementMultiplier);
                    }

                    if (keyboardState.IsKeyDown(Keys.A)) {
                        this._camera.LocalPosition += new Vector2(movementMultiplier * -1f, 0f);
                    }

                    if (keyboardState.IsKeyDown(Keys.S)) {
                        this._camera.LocalPosition += new Vector2(0f, movementMultiplier * -1f);
                    }

                    if (keyboardState.IsKeyDown(Keys.D)) {
                        this._camera.LocalPosition += new Vector2(movementMultiplier, 0f);
                    }
                }
            }
        }
    }
}