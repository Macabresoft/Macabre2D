namespace Macabresoft.Macabre2D.Tests.Framework.Input;

using System;
using FluentAssertions;
using FluentAssertions.Execution;
using Macabresoft.Macabre2D.Framework;
using NUnit.Framework;
using System.Linq;

[TestFixture]
public class InputSettingsTests {
    [Category("Unit Tests")]
    [Test]
    public static void GetName_ShouldGetName_WhenSet() {
        var inputSettings = new InputSettings();
        var enumValues = Enum.GetValues<InputAction>();
        foreach (var value in enumValues) {
            inputSettings.SetName(value, value.ToString());
        }

        using (new AssertionScope()) {
            foreach (var value in enumValues) {
                inputSettings.GetName(value).Should().Be(value.ToString());
            }
        }
    }
    
    [Category("Unit Tests")]
    [Test]
    public static void GetName_ShouldGetEmpty_WhenNoValues() {
        var inputSettings = new InputSettings();
        var enumValues = Enum.GetValues<InputAction>().ToList();

        using (new AssertionScope()) {
            foreach (var value in enumValues) {
                inputSettings.GetName(value).Should().Be(string.Empty);
            }
        }
    }

    [Category("Unit Tests")]
    [Test]
    public static void IsActionEnabled_ShouldReturnTrue_WhenExists() {
        var inputSettings = new InputSettings();
        var enumValues = Enum.GetValues<InputAction>();
        foreach (var value in enumValues) {
            inputSettings.SetName(value, value.ToString());
        }

        using (new AssertionScope()) {
            foreach (var value in enumValues) {
                inputSettings.IsActionEnabled(value).Should().BeTrue();
            }
        }
    }
    
    [Category("Unit Tests")]
    [Test]
    public static void IsActionEnabled_ShouldReturnFalse_WhenNoValues() {
        var inputSettings = new InputSettings();
        var enumValues = Enum.GetValues<InputAction>();
        using (new AssertionScope()) {
            foreach (var value in enumValues) {
                inputSettings.IsActionEnabled(value).Should().BeFalse();
            }
        }
    }
    
    [Category("Unit Tests")]
    [Test]
    public static void SetName_ShouldDisableAction_WhenSetEmpty() {
        var inputSettings = new InputSettings();
        var enumValues = Enum.GetValues<InputAction>();
        foreach (var value in enumValues) {
            inputSettings.SetName(value, value.ToString());
        }
        
        foreach (var value in enumValues) {
            inputSettings.SetName(value, string.Empty);
        }

        using (new AssertionScope()) {
            foreach (var value in enumValues) {
                inputSettings.IsActionEnabled(value).Should().BeFalse();
            }
        }
    }
    
    [Category("Unit Tests")]
    [Test]
    public static void TryGetAction_ShouldReturnTrue_WhenExists() {
        var inputSettings = new InputSettings();
        var enumValues = Enum.GetValues<InputAction>();
        
        foreach (var value in enumValues) {
            inputSettings.SetName(value, value.ToString());
        }
        
        using (new AssertionScope()) {
            foreach (var value in enumValues) {
                inputSettings.TryGetAction(value.ToString(), out var action).Should().BeTrue();
                action.Should().Be(value);
            }
        }
    }
    
    [Category("Unit Tests")]
    [Test]
    public static void TryGetAction_ShouldReturnFalse_WhenNoValues() {
        var inputSettings = new InputSettings();
        var enumValues = Enum.GetValues<InputAction>();

        using (new AssertionScope()) {
            foreach (var value in enumValues) {
                inputSettings.TryGetAction(value.ToString(), out _).Should().BeFalse();
            }
        }
    }
}