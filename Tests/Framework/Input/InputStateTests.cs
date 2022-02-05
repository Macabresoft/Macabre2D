namespace Macabresoft.Macabre2D.Tests.Framework.Input;

using System.Linq;
using FluentAssertions;
using Macabresoft.Macabre2D.Framework;
using Microsoft.Xna.Framework.Input;
using NUnit.Framework;

[TestFixture]
public sealed class InputStateTests {
    private static MouseState GetMouseState(params MouseButton[] pressedButtons) {
        var left = pressedButtons.Contains(MouseButton.Left) ? ButtonState.Pressed : ButtonState.Released;
        var middle = pressedButtons.Contains(MouseButton.Middle) ? ButtonState.Pressed : ButtonState.Released;
        var right = pressedButtons.Contains(MouseButton.Right) ? ButtonState.Pressed : ButtonState.Released;
        var x1 = pressedButtons.Contains(MouseButton.XButton1) ? ButtonState.Pressed : ButtonState.Released;
        var x2 = pressedButtons.Contains(MouseButton.XButton2) ? ButtonState.Pressed : ButtonState.Released;
        return new MouseState(0, 0, 0, left, middle, right, x1, x2);
    }

    private static InputState GetKeyboardInputState(Keys keyHeldPreviously, Keys keyHeldCurrently) {
        var previousKeyboardState = new KeyboardState(keyHeldPreviously);
        var currentKeyboardState = new KeyboardState(keyHeldCurrently);
        var previousInputState = new InputState(new MouseState(), previousKeyboardState, new InputState());
        return new InputState(new MouseState(), currentKeyboardState, previousInputState);
    }

    private static InputState GetMouseInputState(MouseButton button, ButtonState previousState, ButtonState currentState) {
        var previousMouseState = previousState == ButtonState.Pressed ? GetMouseState(button) : new MouseState();
        var currentMouseState = currentState == ButtonState.Pressed ? GetMouseState(button) : new MouseState();
        var previousInputState = new InputState(previousMouseState, new KeyboardState(), new InputState());
        return new InputState(currentMouseState, new KeyboardState(), previousInputState);
    }

    [Category("Unit Tests")]
    [TestCase(Keys.Escape, Keys.Escape, Keys.Escape, true)]
    [TestCase(Keys.Space, Keys.Escape, Keys.Space, true)]
    [TestCase(Keys.A, Keys.A, Keys.Space, false)]
    public static void InputState_IsKeyHeld_Test(Keys keyToCheck, Keys keyHeldPreviously, Keys keyHeldCurrently, bool expectedValue) {
        var inputState = GetKeyboardInputState(keyHeldPreviously, keyHeldCurrently);
        var result = inputState.IsKeyHeld(keyToCheck);
        result.Should().Be(expectedValue);
    }

    [Category("Unit Tests")]
    [TestCase(Keys.Escape, Keys.Escape, Keys.Escape, false)]
    [TestCase(Keys.Space, Keys.Escape, Keys.Space, true)]
    [TestCase(Keys.A, Keys.A, Keys.Space, false)]
    public static void InputState_IsKeyNewlyPressed_Test(Keys keyToCheck, Keys keyHeldPreviously, Keys keyHeldCurrently, bool expectedValue) {
        var inputState = GetKeyboardInputState(keyHeldPreviously, keyHeldCurrently);
        var result = inputState.IsKeyNewlyPressed(keyToCheck);
        result.Should().Be(expectedValue);
    }

    [Category("Unit Tests")]
    [TestCase(Keys.Escape, Keys.Escape, Keys.Escape, false)]
    [TestCase(Keys.Space, Keys.Escape, Keys.Space, false)]
    [TestCase(Keys.A, Keys.A, Keys.Space, true)]
    public static void InputState_IsKeyNewlyReleased_Test(Keys keyToCheck, Keys keyHeldPreviously, Keys keyHeldCurrently, bool expectedValue) {
        var inputState = GetKeyboardInputState(keyHeldPreviously, keyHeldCurrently);
        var result = inputState.IsKeyNewlyReleased(keyToCheck);
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
    public static void InputState_IsMouseButtonHeld_Test(MouseButton button, ButtonState previousState, ButtonState currentState, bool expectedValue) {
        var inputState = GetMouseInputState(button, previousState, currentState);
        var result = inputState.IsMouseButtonHeld(button);
        result.Should().Be(expectedValue);
    }

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
    public static void InputState_IsMouseButtonNewlyPressed_Test(MouseButton button, ButtonState previousState, ButtonState currentState, bool expectedValue) {
        var inputState = GetMouseInputState(button, previousState, currentState);
        var result = inputState.IsMouseButtonNewlyPressed(button);
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
    public static void InputState_IsMouseButtonNewlyReleased_Test(MouseButton button, ButtonState previousState, ButtonState currentState, bool expectedValue) {
        var inputState = GetMouseInputState(button, previousState, currentState);
        var result = inputState.IsMouseButtonNewlyReleased(button);
        result.Should().Be(expectedValue);
    }
}