namespace Macabresoft.Macabre2D.Tests.Framework.Input {
    using System.Linq;
    using FluentAssertions;
    using Macabresoft.Macabre2D.Framework;
    using Microsoft.Xna.Framework.Input;
    using NUnit.Framework;

    [TestFixture]
    public sealed class InputStateTests {
        [Test]
        [Category("Unit Tests")]
        [TestCase(MouseButton.Left, ButtonState.Released, ButtonState.Pressed, true)]
        [TestCase(MouseButton.Left, ButtonState.Pressed, ButtonState.Pressed, false)]
        [TestCase(MouseButton.Left, ButtonState.Released, ButtonState.Released, false)]
        [TestCase(MouseButton.Left, ButtonState.Pressed, ButtonState.Released, false)]
        [TestCase(MouseButton.Middle, ButtonState.Released, ButtonState.Pressed, true)]
        [TestCase(MouseButton.Middle, ButtonState.Pressed, ButtonState.Pressed, false)]
        [TestCase(MouseButton.Middle, ButtonState.Released, ButtonState.Released, false)]
        [TestCase(MouseButton.Middle, ButtonState.Pressed, ButtonState.Released, false)]
        [TestCase(MouseButton.Right, ButtonState.Released, ButtonState.Pressed, true)]
        [TestCase(MouseButton.Right, ButtonState.Pressed, ButtonState.Pressed, false)]
        [TestCase(MouseButton.Right, ButtonState.Released, ButtonState.Released, false)]
        [TestCase(MouseButton.Right, ButtonState.Pressed, ButtonState.Released, false)]
        [TestCase(MouseButton.XButton1, ButtonState.Released, ButtonState.Pressed, true)]
        [TestCase(MouseButton.XButton1, ButtonState.Pressed, ButtonState.Pressed, false)]
        [TestCase(MouseButton.XButton1, ButtonState.Released, ButtonState.Released, false)]
        [TestCase(MouseButton.XButton1, ButtonState.Pressed, ButtonState.Released, false)]
        [TestCase(MouseButton.XButton2, ButtonState.Released, ButtonState.Pressed, true)]
        [TestCase(MouseButton.XButton2, ButtonState.Pressed, ButtonState.Pressed, false)]
        [TestCase(MouseButton.XButton2, ButtonState.Released, ButtonState.Released, false)]
        [TestCase(MouseButton.XButton2, ButtonState.Pressed, ButtonState.Released, false)]
        public static void InputState_IsButtonNewlyPressed_Test(MouseButton button, ButtonState previousState, ButtonState currentState, bool expectedValue) {
            var previousMouseState = previousState == ButtonState.Pressed ? GetMouseState(button) : new MouseState();
            var currentMouseState = currentState == ButtonState.Pressed ? GetMouseState(button) : new MouseState();
            var previousInputState = new InputState(previousMouseState, new KeyboardState(), new InputState());
            var inputState = new InputState(currentMouseState, new KeyboardState(), previousInputState);

            var result = inputState.IsButtonNewlyPressed(button);

            result.Should().Be(expectedValue);
        }

        [Category("Unit Tests")]
        [TestCase(MouseButton.Left, ButtonState.Released, ButtonState.Pressed, false)]
        [TestCase(MouseButton.Left, ButtonState.Pressed, ButtonState.Pressed, false)]
        [TestCase(MouseButton.Left, ButtonState.Released, ButtonState.Released, false)]
        [TestCase(MouseButton.Left, ButtonState.Pressed, ButtonState.Released, true)]
        [TestCase(MouseButton.Middle, ButtonState.Released, ButtonState.Pressed, false)]
        [TestCase(MouseButton.Middle, ButtonState.Pressed, ButtonState.Pressed, false)]
        [TestCase(MouseButton.Middle, ButtonState.Released, ButtonState.Released, false)]
        [TestCase(MouseButton.Middle, ButtonState.Pressed, ButtonState.Released, true)]
        [TestCase(MouseButton.Right, ButtonState.Released, ButtonState.Pressed, false)]
        [TestCase(MouseButton.Right, ButtonState.Pressed, ButtonState.Pressed, false)]
        [TestCase(MouseButton.Right, ButtonState.Released, ButtonState.Released, false)]
        [TestCase(MouseButton.Right, ButtonState.Pressed, ButtonState.Released, true)]
        [TestCase(MouseButton.XButton1, ButtonState.Released, ButtonState.Pressed, false)]
        [TestCase(MouseButton.XButton1, ButtonState.Pressed, ButtonState.Pressed, false)]
        [TestCase(MouseButton.XButton1, ButtonState.Released, ButtonState.Released, false)]
        [TestCase(MouseButton.XButton1, ButtonState.Pressed, ButtonState.Released, true)]
        [TestCase(MouseButton.XButton2, ButtonState.Released, ButtonState.Pressed, false)]
        [TestCase(MouseButton.XButton2, ButtonState.Pressed, ButtonState.Pressed, false)]
        [TestCase(MouseButton.XButton2, ButtonState.Released, ButtonState.Released, false)]
        [TestCase(MouseButton.XButton2, ButtonState.Pressed, ButtonState.Released, true)]
        public static void InputState_IsButtonNewlyReleased_Test(MouseButton button, ButtonState previousState, ButtonState currentState, bool expectedValue) {
            var previousMouseState = previousState == ButtonState.Pressed ? GetMouseState(button) : new MouseState();
            var currentMouseState = currentState == ButtonState.Pressed ? GetMouseState(button) : new MouseState();
            var previousInputState = new InputState(previousMouseState, new KeyboardState(), new InputState());
            var inputState = new InputState(currentMouseState, new KeyboardState(), previousInputState);

            var result = inputState.IsButtonNewlyReleased(button);

            result.Should().Be(expectedValue);
        }

        [Category("Unit Tests")]
        [TestCase(MouseButton.Left, ButtonState.Released, ButtonState.Pressed, true)]
        [TestCase(MouseButton.Left, ButtonState.Pressed, ButtonState.Pressed, true)]
        [TestCase(MouseButton.Left, ButtonState.Released, ButtonState.Released, false)]
        [TestCase(MouseButton.Left, ButtonState.Pressed, ButtonState.Released, false)]
        [TestCase(MouseButton.Middle, ButtonState.Released, ButtonState.Pressed, true)]
        [TestCase(MouseButton.Middle, ButtonState.Pressed, ButtonState.Pressed, true)]
        [TestCase(MouseButton.Middle, ButtonState.Released, ButtonState.Released, false)]
        [TestCase(MouseButton.Middle, ButtonState.Pressed, ButtonState.Released, false)]
        [TestCase(MouseButton.Right, ButtonState.Released, ButtonState.Pressed, true)]
        [TestCase(MouseButton.Right, ButtonState.Pressed, ButtonState.Pressed, true)]
        [TestCase(MouseButton.Right, ButtonState.Released, ButtonState.Released, false)]
        [TestCase(MouseButton.Right, ButtonState.Pressed, ButtonState.Released, false)]
        [TestCase(MouseButton.XButton1, ButtonState.Released, ButtonState.Pressed, true)]
        [TestCase(MouseButton.XButton1, ButtonState.Pressed, ButtonState.Pressed, true)]
        [TestCase(MouseButton.XButton1, ButtonState.Released, ButtonState.Released, false)]
        [TestCase(MouseButton.XButton1, ButtonState.Pressed, ButtonState.Released, false)]
        [TestCase(MouseButton.XButton2, ButtonState.Released, ButtonState.Pressed, true)]
        [TestCase(MouseButton.XButton2, ButtonState.Pressed, ButtonState.Pressed, true)]
        [TestCase(MouseButton.XButton2, ButtonState.Released, ButtonState.Released, false)]
        [TestCase(MouseButton.XButton2, ButtonState.Pressed, ButtonState.Released, false)]
        public static void InputState_IsButtonHeld_Test(MouseButton button, ButtonState previousState, ButtonState currentState, bool expectedValue) {
            var previousMouseState = previousState == ButtonState.Pressed ? GetMouseState(button) : new MouseState();
            var currentMouseState = currentState == ButtonState.Pressed ? GetMouseState(button) : new MouseState();
            var previousInputState = new InputState(previousMouseState, new KeyboardState(), new InputState());
            var inputState = new InputState(currentMouseState, new KeyboardState(), previousInputState);

            var result = inputState.IsButtonHeld(button);

            result.Should().Be(expectedValue);
        }

        private static MouseState GetMouseState(params MouseButton[] pressedButtons) {
            var left = pressedButtons.Contains(MouseButton.Left) ? ButtonState.Pressed : ButtonState.Released;
            var middle = pressedButtons.Contains(MouseButton.Middle) ? ButtonState.Pressed : ButtonState.Released;
            var right = pressedButtons.Contains(MouseButton.Right) ? ButtonState.Pressed : ButtonState.Released;
            var x1 = pressedButtons.Contains(MouseButton.XButton1) ? ButtonState.Pressed : ButtonState.Released;
            var x2 = pressedButtons.Contains(MouseButton.XButton2) ? ButtonState.Pressed : ButtonState.Released;
            return new MouseState(0, 0, 0, left, middle, right, x1, x2);
        }
    }
}