namespace Macabresoft.Macabre2D.Tests.Framework.Rendering;

using AwesomeAssertions;
using AwesomeAssertions.Execution;
using Macabresoft.Macabre2D.Framework;
using Microsoft.Xna.Framework;
using NUnit.Framework;

[TestFixture]
public class ScreenShaderTests {
    [Category("Unit Tests")]
    [Test]
    [TestCase(ScreenShaderSizing.FullScreen, 100, 100, 200, 200, 1, 100, 100)]
    [TestCase(ScreenShaderSizing.PixelSize, 100, 100, 200, 200, 1, 200, 200)]
    [TestCase(ScreenShaderSizing.LimitedPixelSize, 100, 100, 200, 200, 1, 100, 100)]
    [TestCase(ScreenShaderSizing.FullScreen, 100, 100, 200, 200, 3, 300, 300)]
    [TestCase(ScreenShaderSizing.PixelSize, 100, 100, 200, 200, 2, 400, 400)]
    [TestCase(ScreenShaderSizing.LimitedPixelSize, 100, 100, 200, 200, 2, 100, 100)]
    public static void GetRenderSize_ShouldGetCorrectValue(
        ScreenShaderSizing sizing,
        int viewPortX,
        int viewPortY,
        int pixelX,
        int pixelY,
        byte multiplier,
        int expectedX,
        int expectedY) {
        var shader = new ScreenShader {
            Multiplier = multiplier,
            Sizing = sizing
        };

        var result = shader.GetRenderSize(new Point(viewPortX, viewPortY), new Point(pixelX, pixelY));

        using (new AssertionScope()) {
            result.X.Should().Be(expectedX);
            result.Y.Should().Be(expectedY);
        }
    }
}