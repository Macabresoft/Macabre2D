namespace Macabresoft.Macabre2D.Tests.Framework.Input;

using FluentAssertions;
using FluentAssertions.Execution;
using Macabresoft.Macabre2D.Framework;
using Microsoft.Xna.Framework.Input;
using NUnit.Framework;

[TestFixture]
public class InputBindingsTests {
    [Category("Unit Tests")]
    [Test]
    public void TryGetBinding_ShouldReturnFalse_WhenNoBindings() {
        var bindings = new InputBindings();

        var result = bindings.TryGetBindings(InputAction.Action01, out var controllerButton, out var key, out var mouseButton);

        using (new AssertionScope()) {
            result.Should().BeFalse();
            controllerButton.Should().Be(Buttons.None);
            key.Should().Be(Keys.None);
            mouseButton.Should().Be(MouseButton.None);
        }
    }

    [Category("Unit Tests")]
    [Test]
    public void TryGetBinding_ShouldReturnTrue_WhenAllBindings() {
        var bindings = new InputBindings();
        bindings.SetControllerBinding(InputAction.Action01, Buttons.A);
        bindings.SetKeyBinding(InputAction.Action01, Keys.A);
        bindings.SetMouseBinding(InputAction.Action01, MouseButton.Left);

        var result = bindings.TryGetBindings(InputAction.Action01, out var controllerButton, out var key, out var mouseButton);

        using (new AssertionScope()) {
            result.Should().BeTrue();
            controllerButton.Should().Be(Buttons.A);
            key.Should().Be(Keys.A);
            mouseButton.Should().Be(MouseButton.Left);
        }
    }

    [Category("Unit Tests")]
    [Test]
    public void TryGetBinding_ShouldReturnTrue_WhenControllerBinding() {
        var bindings = new InputBindings();
        bindings.SetControllerBinding(InputAction.Action01, Buttons.A);

        var result = bindings.TryGetBindings(InputAction.Action01, out var controllerButton, out var key, out var mouseButton);

        using (new AssertionScope()) {
            result.Should().BeTrue();
            controllerButton.Should().Be(Buttons.A);
            key.Should().Be(Keys.None);
            mouseButton.Should().Be(MouseButton.None);
        }
    }

    [Category("Unit Tests")]
    [Test]
    public void TryGetBinding_ShouldReturnTrue_WhenKeyBinding() {
        var bindings = new InputBindings();
        bindings.SetKeyBinding(InputAction.Action01, Keys.A);

        var result = bindings.TryGetBindings(InputAction.Action01, out var controllerButton, out var key, out var mouseButton);

        using (new AssertionScope()) {
            result.Should().BeTrue();
            controllerButton.Should().Be(Buttons.None);
            key.Should().Be(Keys.A);
            mouseButton.Should().Be(MouseButton.None);
        }
    }

    [Category("Unit Tests")]
    [Test]
    public void TryGetBinding_ShouldReturnTrue_WhenMouseBinding() {
        var bindings = new InputBindings();
        bindings.SetMouseBinding(InputAction.Action01, MouseButton.Left);

        var result = bindings.TryGetBindings(InputAction.Action01, out var controllerButton, out var key, out var mouseButton);

        using (new AssertionScope()) {
            result.Should().BeTrue();
            controllerButton.Should().Be(Buttons.None);
            key.Should().Be(Keys.None);
            mouseButton.Should().Be(MouseButton.Left);
        }
    }
}