namespace Macabresoft.Macabre2D.Tests.Framework.Settings;

using System;
using FluentAssertions;
using FluentAssertions.Execution;
using Macabresoft.Core;
using Macabresoft.Macabre2D.Framework;
using NUnit.Framework;

[TestFixture]
public class LayerSettingsTests {
    [Category("Unit Tests")]
    [Test]
    public static void LayerSettings_GetLayerWithUnknownName_ShouldReturnNone() {
        var layerSettings = new LayerSettings();
        var layers = layerSettings.GetLayer(Guid.NewGuid().ToString());

        using (new AssertionScope()) {
            layers.Should().Be(Layers.None);
        }
    }

    [Category("Unit Tests")]
    [Test]
    public static void LayerSettings_GetNameWithMultiple_ShouldReturnEnumDisplayName() {
        var layerSettings = new LayerSettings();
        var layers = Layers.Layer01 | Layers.Layer03 | Layers.Layer07;
        var newName = layerSettings.GetName(layers);

        using (new AssertionScope()) {
            newName.Should().Be(layers.GetEnumDisplayName());
        }
    }

    [Category("Unit Tests")]
    [TestCase(Layers.Layer01, "Hammer", true)]
    [TestCase(Layers.Layer13 | Layers.Layer04, "Sickle", false)]
    [TestCase(Layers.Layer02, "Layer 1", false)]
    public static void LayerSettings_SetName_Test(Layers layer, string name, bool expectedValue) {
        var layerSettings = new LayerSettings();
        var originalName = layerSettings.GetName(layer);
        var result = layerSettings.SetName(layer, name);
        var newName = layerSettings.GetName(layer);

        using (new AssertionScope()) {
            result.Should().Be(expectedValue);
            newName.Should().Be(expectedValue ? name : originalName);
        }
    }
}