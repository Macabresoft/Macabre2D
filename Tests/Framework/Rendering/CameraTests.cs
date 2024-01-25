namespace Macabresoft.Macabre2D.Tests.Framework;

using FluentAssertions;
using FluentAssertions.Execution;
using Macabresoft.Macabre2D.Framework;
using Microsoft.Xna.Framework;
using NSubstitute;
using NUnit.Framework;

[TestFixture]
public sealed class CameraTests {
    [Test]
    [Category("Unit Tests")]
    [TestCase(10f, 2f, 2f, 1f, 9f, 0.2f, 0.2f, TestName = "Camera_ZoomTo_WorldPoint_1")]
    [TestCase(10f, 10f, 10f, 5f, 5f, 5f, 5f, TestName = "Camera_ZoomTo_WorldPoint_2")]
    public static void Camera_ZoomTo_WorldPointTest(
        float startingViewHeight,
        float zoomX,
        float zoomY,
        float zoomAmount,
        float expectedViewHeight,
        float expectedX,
        float expectedY) {
        var camera = new Camera {
            ViewHeight = startingViewHeight
        };

        var scene = Substitute.For<IScene>();
        camera.Initialize(scene, new Entity());

        camera.ZoomTo(new Vector2(zoomX, zoomY), zoomAmount);
        Assert.That(expectedViewHeight, Is.EqualTo(camera.ViewHeight).Within(0.001f));
        Assert.That(expectedX, Is.EqualTo(camera.LocalPosition.X).Within(0.001f));
        Assert.That(expectedY, Is.EqualTo(camera.LocalPosition.Y).Within(0.001f));
    }

    [Test]
    [Category("Unit Tests")]
    [TestCase(1, 1, 1f, 1f)]
    [TestCase(1, 2, 1f, 2f)]
    [TestCase(5, 3, 50f, 30f)]
    public void ViewWidth_Should_BeCorrect(int screenHeight, int screenWidth, float viewHeight, float expectedViewWidth) {
        var camera = this.CreateCamera(screenWidth, screenHeight, viewHeight);

        using (new AssertionScope()) {
            camera.ViewWidth.Should().Be(expectedViewWidth);
        }
    }

    private ICamera CreateCamera(int screenWidth, int screenHeight, float viewHeight) {
        var scene = Substitute.For<IScene>();
        var game = Substitute.For<IGame>();
        scene.Game.Returns(game);
        game.ViewportSize.Returns(new Point(screenWidth, screenHeight));

        var camera = new Camera {
            ViewHeight = viewHeight
        };

        camera.Initialize(scene, scene);
        return camera;
    }
}