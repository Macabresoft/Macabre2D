namespace Macabresoft.Macabre2D.Tests.Framework;

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
        Assert.AreEqual(expectedViewHeight, camera.ViewHeight);
        Assert.AreEqual(expectedX, camera.LocalPosition.X, 0.001f);
        Assert.AreEqual(expectedY, camera.LocalPosition.Y, 0.001f);
    }
}