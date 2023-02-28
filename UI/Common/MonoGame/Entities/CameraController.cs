namespace Macabresoft.Macabre2D.UI.Common;

using System;
using Avalonia.Input;
using Macabresoft.Macabre2D.Framework;
using Macabresoft.Macabre2D.UI.AvaloniaInterop;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

public sealed class CameraController : UpdateableEntity {
    private readonly IEditorService _editorService;
    private Camera _camera;

    public CameraController(IEditorService editorService) {
        this._editorService = editorService;
    }

    public override void Initialize(IScene scene, IEntity entity) {
        base.Initialize(scene, entity);

        if (!this.TryGetParentEntity(out this._camera)) {
            throw new NotSupportedException("Could not find a camera ancestor.");
        }
    }

    public override void Update(FrameTime frameTime, InputState inputState) {
        if (this.Game is IAvaloniaGame game && this._camera != null) {
            var mouseState = inputState.CurrentMouseState;
            var keyboardState = inputState.CurrentKeyboardState;
            var previousMouseState = inputState.PreviousMouseState;
            if (mouseState.ScrollWheelValue != previousMouseState.ScrollWheelValue) {
                var scrollViewChange = (float)(frameTime.SecondsPassed * (previousMouseState.ScrollWheelValue - mouseState.ScrollWheelValue) * this._camera.ActualViewHeight);

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

                if (this._editorService.CursorType == StandardCursorType.None) {
                    this._editorService.CursorType = StandardCursorType.SizeAll;
                }
            }
            else if (this._editorService.CursorType == StandardCursorType.SizeAll) {
                this._editorService.CursorType = StandardCursorType.None;
            }

            if (!keyboardState.IsModifierKeyDown()) {
                var movementMultiplier = (float)frameTime.SecondsPassed * this._camera.ActualViewHeight;
                if (keyboardState.IsKeyDown(Keys.W) || keyboardState.IsKeyDown(Keys.Up)) {
                    this._camera.LocalPosition += new Vector2(0f, movementMultiplier);
                }

                if (keyboardState.IsKeyDown(Keys.A) || keyboardState.IsKeyDown(Keys.Left)) {
                    this._camera.LocalPosition += new Vector2(movementMultiplier * -1f, 0f);
                }

                if (keyboardState.IsKeyDown(Keys.S) || keyboardState.IsKeyDown(Keys.Down)) {
                    this._camera.LocalPosition += new Vector2(0f, movementMultiplier * -1f);
                }

                if (keyboardState.IsKeyDown(Keys.D) || keyboardState.IsKeyDown(Keys.Right)) {
                    this._camera.LocalPosition += new Vector2(movementMultiplier, 0f);
                }

                if (keyboardState.IsKeyDown(Keys.OemPlus) || keyboardState.IsKeyDown(Keys.OemCloseBrackets)) {
                    var scrollViewChange = (float)(frameTime.SecondsPassed * this._camera.ViewHeight);
                    this._camera.ViewHeight -= scrollViewChange;
                }

                if (keyboardState.IsKeyDown(Keys.OemMinus) || keyboardState.IsKeyDown(Keys.OemOpenBrackets)) {
                    var scrollViewChange = (float)(frameTime.SecondsPassed * this._camera.ViewHeight);
                    this._camera.ViewHeight += scrollViewChange;
                }
            }
        }
    }
}