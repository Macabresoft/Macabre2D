namespace Macabresoft.Macabre2D.Tests.Framework.Settings;

using System;
using FluentAssertions;
using FluentAssertions.Execution;
using Macabresoft.Core;
using Macabresoft.Macabre2D.Framework;
using NUnit.Framework;

[TestFixture]
public class LayerSettingsTests {
    private const Layers AllButDefault = LayerSettings.AllLayers & ~Layers.Default;

    [Category("Unit Tests")]
    [Test]
    public static void DisableAll_ShouldNotDisableDefault() {
        var layerSettings = new LayerSettings();
        var initialValue = layerSettings.IsLayerEnabled(LayerSettings.AllLayers);
        layerSettings.DisableLayers(LayerSettings.AllLayers);
        var resultAll = layerSettings.IsLayerEnabled(LayerSettings.AllLayers);
        var resultButDefault = layerSettings.IsLayerEnabled(AllButDefault);

        using (new AssertionScope()) {
            initialValue.Should().BeTrue();
            resultAll.Should().BeTrue();
            resultButDefault.Should().BeFalse();
            layerSettings.EnabledLayers.Should().Be(Layers.Default);
        }
    }

    [Category("Unit Tests")]
    [Test]
    public static void DisableLayerWithSingleBit_ShouldDisable() {
        var layerSettings = new LayerSettings();
        var initialValue = layerSettings.IsLayerEnabled(Layers.Layer02);
        layerSettings.DisableLayers(Layers.Layer02);
        var result = layerSettings.IsLayerEnabled(Layers.Layer02);

        using (new AssertionScope()) {
            initialValue.Should().BeTrue();
            result.Should().BeFalse();
        }
    }

    [Category("Unit Tests")]
    [Test]
    public static void DisableManyLayers_ShouldDisable() {
        var layerSettings = new LayerSettings();
        var threeLayers = Layers.Layer01 | Layers.Layer05 | Layers.Layer13;
        var initialLayer01 = layerSettings.IsLayerEnabled(Layers.Layer01);
        var initialLayer05 = layerSettings.IsLayerEnabled(Layers.Layer05);
        var initialLayer13 = layerSettings.IsLayerEnabled(Layers.Layer13);
        var initialAll = layerSettings.IsLayerEnabled(threeLayers);
        layerSettings.DisableLayers(threeLayers);

        var resultLayer01 = layerSettings.IsLayerEnabled(Layers.Layer01);
        var resultLayer05 = layerSettings.IsLayerEnabled(Layers.Layer05);
        var resultLayer13 = layerSettings.IsLayerEnabled(Layers.Layer13);
        var resultAll = layerSettings.IsLayerEnabled(threeLayers);

        using (new AssertionScope()) {
            initialLayer01.Should().BeTrue();
            initialLayer05.Should().BeTrue();
            initialLayer13.Should().BeTrue();
            initialAll.Should().BeTrue();

            resultLayer01.Should().BeFalse();
            resultLayer05.Should().BeFalse();
            resultLayer13.Should().BeFalse();
            resultAll.Should().BeFalse();
        }
    }

    [Category("Unit Tests")]
    [Test]
    public static void DisableOnlyDefaultLayer_ShouldNotDisable() {
        var layerSettings = new LayerSettings();
        var initialValue = layerSettings.IsLayerEnabled(Layers.Default);
        layerSettings.DisableLayers(Layers.Default);
        var result = layerSettings.IsLayerEnabled(Layers.Default);

        using (new AssertionScope()) {
            initialValue.Should().BeTrue();
            result.Should().BeTrue();
        }
    }

    [Category("Unit Tests")]
    [Test]
    public static void EnableAll_WhenSomeDisabled_ShouldEnableAll() {
        var layerSettings = new LayerSettings(Layers.Layer03 | Layers.Layer06);
        layerSettings.EnableLayers(LayerSettings.AllLayers);

        using (new AssertionScope()) {
            layerSettings.IsLayerEnabled(AllButDefault).Should().BeTrue();
            layerSettings.IsLayerEnabled(LayerSettings.AllLayers).Should().BeTrue();
        }
    }

    [Category("Unit Tests")]
    [Test]
    public static void EnableSingleLayer__ShouldEnableSingleLayer() {
        var layerSettings = new LayerSettings(Layers.None);
        layerSettings.EnableLayers(Layers.Layer12);

        using (new AssertionScope()) {
            layerSettings.IsLayerEnabled(Layers.Layer12).Should().BeTrue();
        }
    }

    [Category("Unit Tests")]
    [Test]
    public static void GetLayerWithUnknownName_ShouldReturnNone() {
        var layerSettings = new LayerSettings();
        var layers = layerSettings.GetLayer(Guid.NewGuid().ToString());

        using (new AssertionScope()) {
            layers.Should().Be(Layers.None);
        }
    }

    [Category("Unit Tests")]
    [Test]
    public static void GetNameWithMultiple_ShouldReturnEnumDisplayName() {
        var layerSettings = new LayerSettings();
        var layers = Layers.Layer01 | Layers.Layer03 | Layers.Layer07;
        var newName = layerSettings.GetName(layers);

        using (new AssertionScope()) {
            newName.Should().Be(layers.GetEnumDisplayName());
        }
    }

    [Category("Unit Tests")]
    [Test]
    public static void NoLayers_ShouldStillHaveDefault() {
        var layerSettings = new LayerSettings(Layers.None);

        using (new AssertionScope()) {
            layerSettings.IsLayerEnabled(AllButDefault).Should().BeFalse();
            layerSettings.IsLayerEnabled(Layers.Default).Should().BeTrue();
        }
    }

    [Category("Unit Tests")]
    [TestCase(Layers.Layer01, "Hammer", true)]
    [TestCase(Layers.Layer13 | Layers.Layer04, "Sickle", false)]
    [TestCase(Layers.Layer02, "Layer 1", false)]
    public static void SetName_Test(Layers layer, string name, bool expectedValue) {
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