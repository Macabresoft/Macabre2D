namespace Macabresoft.Macabre2D.Tests.Framework.Input;

using AwesomeAssertions;
using AwesomeAssertions.Execution;
using Macabresoft.Macabre2D.Framework;
using Macabresoft.Macabre2D.Project.Common;
using Microsoft.Xna.Framework.Input;
using NUnit.Framework;

[TestFixture]
public class InputSettingsTests {
    [Category("Unit Tests")]
    [Test]
    public void TryGetBinding_ShouldReturnFalse_WhenNoBindings() {
        var bindings = new InputSettings();

        var result = bindings.TryGetBindings(InputAction.Confirm, out var controllerButton, out _, out var key, out var mouseButton);

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
        var bindings = new InputSettings();
        bindings.SetPrimaryGamePadBinding(InputAction.Confirm, Buttons.A);
        bindings.SetKeyBinding(InputAction.Confirm, Keys.A);
        bindings.SetMouseBinding(InputAction.Confirm, MouseButton.Left);

        var result = bindings.TryGetBindings(InputAction.Confirm, out var controllerButton, out _, out var key, out var mouseButton);

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
        var bindings = new InputSettings();
        bindings.SetPrimaryGamePadBinding(InputAction.Confirm, Buttons.A);

        var result = bindings.TryGetBindings(InputAction.Confirm, out var controllerButton, out _, out var key, out var mouseButton);

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
        var bindings = new InputSettings();
        bindings.SetKeyBinding(InputAction.Confirm, Keys.A);

        var result = bindings.TryGetBindings(InputAction.Confirm, out var controllerButton, out _, out var key, out var mouseButton);

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
        var bindings = new InputSettings();
        bindings.SetMouseBinding(InputAction.Confirm, MouseButton.Left);

        var result = bindings.TryGetBindings(InputAction.Confirm, out var controllerButton, out _, out var key, out var mouseButton);

        using (new AssertionScope()) {
            result.Should().BeTrue();
            controllerButton.Should().Be(Buttons.None);
            key.Should().Be(Keys.None);
            mouseButton.Should().Be(MouseButton.Left);
        }
    }
}