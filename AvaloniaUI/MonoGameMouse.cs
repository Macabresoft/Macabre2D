namespace Macabresoft.Macabre2D.AvaloniaUI {

    using Avalonia.Input;
    using Microsoft.Xna.Framework.Input;
    using System;

    /// <summary>
    /// A wrapper for MonoGame's <see cref="MouseState" />.
    /// </summary>
    public sealed class MonoGameMouse {
        private readonly IInputElement _focusElement;

        /// <summary>
        /// Creates a new instance of the mouse helper.
        /// </summary>
        /// <param name="focusElement">The element that will be used as the focus point.</param>
        public MonoGameMouse(IInputElement focusElement) {
            this._focusElement = focusElement ?? throw new ArgumentNullException(nameof(focusElement));
            this._focusElement.PointerWheelChanged += this.HandlePointer;

            this._focusElement.PointerMoved += this.HandlePointer;
            this._focusElement.PointerEnter += this.HandlePointer;
            this._focusElement.PointerLeave += this.HandlePointer;

            this._focusElement.PointerPressed += this.HandlePointer;
            this._focusElement.PointerReleased += this.HandlePointer;
        }

        /// <summary>
        /// Gets the state.
        /// </summary>
        /// <value>The state.</value>
        public MouseState State { get; private set; }

        private void HandlePointer(object sender, PointerEventArgs e) {
            if (e.Handled) {
                return;
            }

            if (this._focusElement.IsPointerOver && WindowHelper.IsControlOnActiveWindow(this._focusElement)) {
                if (KeyboardDevice.Instance.FocusedElement != this._focusElement) {
                    this._focusElement.Focus();
                }

                var position = e.GetPosition(this._focusElement);
                e.Handled = true;

                var leftButtonState = ButtonState.Released;
                var rightButtonState = ButtonState.Released;
                var middleButtonState = ButtonState.Released;
                var scrollWheelValue = 0;

                if (e is PointerWheelEventArgs wheelArgs) {
                    scrollWheelValue = this.State.ScrollWheelValue + (int)wheelArgs.Delta.Y;
                }

                if (e.GetCurrentPoint(this._focusElement).Properties is PointerPointProperties properties) {
                    if (properties.IsLeftButtonPressed) {
                        leftButtonState = ButtonState.Pressed;
                    }
                    if (properties.IsRightButtonPressed) {
                        rightButtonState = ButtonState.Pressed;
                    }

                    if (properties.IsMiddleButtonPressed) {
                        middleButtonState = ButtonState.Pressed;
                    }
                }

                this.State = new MouseState(
                    (int)position.X,
                    (int)position.Y,
                    scrollWheelValue,
                    leftButtonState,
                    middleButtonState,
                    rightButtonState,
                    ButtonState.Released,
                    ButtonState.Released);
            }
        }
    }
}