namespace Macabresoft.Macabre2D.UI.AvaloniaInterop {
    using System;
    using Avalonia.Input;
    using Microsoft.Xna.Framework.Input;

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
            this._focusElement.PointerWheelChanged += this.HandleMouseScrollWheel;

            this._focusElement.PointerMoved += this.HandlePointerMoved;
            this._focusElement.PointerEnter += this.HandlePointerMoved;
            this._focusElement.PointerLeave += this.HandlePointerMoved;

            this._focusElement.PointerPressed += this.HandlePointer;
            this._focusElement.PointerReleased += this.HandlePointer;
        }

        /// <summary>
        /// Gets the state.
        /// </summary>
        /// <value>The state.</value>
        public MouseState State { get; private set; }

        private void HandleMouseScrollWheel(object sender, PointerWheelEventArgs e) {
            if (e.Handled) {
                return;
            }

            if (this._focusElement.IsPointerOver && ActiveWindowHelper.IsControlOnActiveWindow(this._focusElement)) {
                if (KeyboardDevice.Instance.FocusedElement != this._focusElement) {
                    this._focusElement.Focus();
                }

                var scrollWheelValue = this.State.ScrollWheelValue + (int)e.Delta.Y;

                this.State = new MouseState(
                    this.State.X,
                    this.State.Y,
                    scrollWheelValue,
                    this.State.LeftButton,
                    this.State.MiddleButton,
                    this.State.RightButton,
                    ButtonState.Released,
                    ButtonState.Released);
            }
        }

        private void HandlePointerMoved(object sender, PointerEventArgs e) {
            if (e.Handled) {
                return;
            }

            if (this._focusElement.IsPointerOver && ActiveWindowHelper.IsControlOnActiveWindow(this._focusElement)) {
                if (KeyboardDevice.Instance.FocusedElement != this._focusElement) {
                    this._focusElement.Focus();
                }

                var position = e.GetPosition(this._focusElement);

                this.State = new MouseState(
                    (int)position.X,
                    (int)position.Y,
                    this.State.ScrollWheelValue,
                    this.State.LeftButton,
                    this.State.MiddleButton,
                    this.State.RightButton,
                    ButtonState.Released,
                    ButtonState.Released);
            }
        }

        private void HandlePointer(object sender, PointerEventArgs e) {
            if (e.Handled) {
                return;
            }

            if (this._focusElement.IsPointerOver && ActiveWindowHelper.IsControlOnActiveWindow(this._focusElement)) {
                if (KeyboardDevice.Instance.FocusedElement != this._focusElement) {
                    this._focusElement.Focus();
                }

                var position = e.GetPosition(this._focusElement);

                var leftButtonState = ButtonState.Released;
                var rightButtonState = ButtonState.Released;
                var middleButtonState = ButtonState.Released;

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
                    this.State.ScrollWheelValue,
                    leftButtonState,
                    middleButtonState,
                    rightButtonState,
                    ButtonState.Released,
                    ButtonState.Released);
            }
        }
    }
}